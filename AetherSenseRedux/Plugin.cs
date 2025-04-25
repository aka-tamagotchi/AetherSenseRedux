using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Buttplug.Client;
using AetherSenseRedux.Trigger;
using AetherSenseRedux.Pattern;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AetherSenseRedux.Hooks;
using AetherSenseRedux.Trigger.Emote;
using Lumina.Excel.Sheets;

namespace AetherSenseRedux
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;

        private const string CommandName = "/asr";

        private Configuration Configuration { get; set; }

        private ButtplugStatus _status;

        private EmoteReaderHooks _emoteReaderHooks;

        public ButtplugStatus Status
        {
            get
            {
                try
                {
                    if (_buttplug == null)
                    {
                        return ButtplugStatus.Uninitialized;
                    }
                    else if (_buttplug.Connected && _status == ButtplugStatus.Connected)
                    {
                        return ButtplugStatus.Connected;
                    }
                    else if (_status == ButtplugStatus.Connecting)
                    {
                        return ButtplugStatus.Connecting;
                    }
                    else if (!_buttplug.Connected && _status == ButtplugStatus.Connected)
                    {
                        return ButtplugStatus.Error;
                    }
                    else if (_status == ButtplugStatus.Disconnecting)
                    {
                        return ButtplugStatus.Disconnecting;
                    }
                    else if (LastException != null)
                    {
                        return ButtplugStatus.Error;
                    }
                    else
                    {
                        return ButtplugStatus.Disconnected;
                    }
                }
                catch (Exception ex)
                {
                    Service.PluginLog.Error(ex, "error when getting status");
                    return ButtplugStatus.Error;
                }


            }
        }

        public bool Scanning
        {
            get
            {
                if (_buttplug == null)
                {
                    return false;
                }
                // Buttplug.IsScanning no longer exists?
                // return Buttplug.IsScanning;
                return false;
            }
        }

        private bool Connected => _buttplug?.Connected ?? false;

        public Dictionary<string, DeviceStatus> ConnectedDevices
        {
            get
            {
                Dictionary<string, DeviceStatus> result = new();
                foreach (var device in _devicePool)
                {
                    result[device.Name] = device.Status;
                }

                return result;
            }
        }

        public Exception? LastException { get; set; }

        public WaitType WaitType { get; set; }

        private PluginUI PluginUi { get; init; }

        private ButtplugClient? _buttplug;
        private ButtplugWebsocketConnector? _buttplugWebsocketConnector;

        private List<Device> _devicePool;

        private readonly List<ChatTrigger> _chatTriggerPool;
        private readonly List<EmoteTrigger> _emoteTriggerPool;

        /// <summary>
        /// 
        /// </summary>
        public Plugin()
        {
            PluginInterface.Inject(this);
            PluginInterface.Create<Service>();

            Service.Plugin = this;
            Service.PluginInterface = PluginInterface;

            var t = DoBenchmark();

            this._devicePool = [];
            this._chatTriggerPool = [];
            this._emoteTriggerPool = [];

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.FixDeserialization();

            _status = ButtplugStatus.Disconnected;

            // Update the configuration if it's an older version
            if (Configuration.Version == 1)
            {
                Configuration.Version = 2;
                Configuration.FirstRun = false;
                Configuration.Save();
            }

            if (Configuration.FirstRun)
            {
                Configuration.LoadDefaults();
            }

            PluginUi = new PluginUI(Configuration);

            Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnShowUI)
            {
                HelpMessage = "Opens the Aether Sense Redux configuration window"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            this._emoteReaderHooks = new EmoteReaderHooks();
            this._emoteReaderHooks.OnEmote += OnEmoteReceived;

            t.Wait();
            WaitType = t.Result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Stop(true);
            PluginUi.Dispose();
            Service.CommandManager.RemoveHandler(CommandName);
            _emoteReaderHooks.Dispose();
        }

        // EVENT HANDLERS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceAdded(object? sender, DeviceAddedEventArgs e)
        {

            Service.PluginLog.Information("Device {0} added", e.Device.Name);
            Device newDevice = new(e.Device, WaitType);
            lock (this._devicePool)
            {
                this._devicePool.Add(newDevice);
            }
            if (!Configuration.SeenDevices.Contains(newDevice.Name))
            {
                Configuration.SeenDevices.Add(newDevice.Name);
            }
            newDevice.Start();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceRemoved(object? sender, DeviceRemovedEventArgs e)
        {
            if (Status != ButtplugStatus.Connected)
            {
                return;
            }
            Service.PluginLog.Information("Device {0} removed", e.Device.Name);
            var toRemove = new List<Device>();
            lock (this._devicePool)
            {
                foreach (var device in this._devicePool.Where(device => device.ClientDevice == e.Device))
                {
                    try
                    {
                        device.Stop();
                    }
                    catch (Exception ex)
                    {
                        Service.PluginLog.Error(ex, "Could not stop device {0}, device disconnected?", device.Name);
                    }
                    toRemove.Add(device);
                    device.Dispose();
                }
            }
            foreach (var device in toRemove)
            {
                lock (this._devicePool)
                {
                    this._devicePool.Remove(device);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnScanComplete(object? sender, EventArgs e)
        {
            // Do nothing, since Buttplug still keeps scanning for BLE devices even after scanning is "complete"
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnServerDisconnect(object? sender, EventArgs e)
        {
            if (Status == ButtplugStatus.Disconnecting)
            {
                return;
            }

            Stop(false);
            Service.PluginLog.Error("Unexpected disconnect.");
        }

        private void OnChatReceived(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            ChatMessage chatMessage = new(type, timestamp, ref sender, ref message, ref isHandled);
            foreach (var t in _chatTriggerPool)
            {
                t.Queue(chatMessage);
            }
            if (Configuration.LogChat)
            {
                Service.PluginLog.Debug(chatMessage.ToString());
            }
        }

        private void OnEmoteReceived(EmoteEvent e)
        {
            var isPerformer = e.Instigator.GameObjectId == Service.ClientState.LocalPlayer?.GameObjectId;
            var isTarget = e.Target?.GameObjectId == Service.ClientState.LocalPlayer?.GameObjectId;

            var emote = Service.DataManager.GetExcelSheet<Emote>().GetRowOrDefault(e.EmoteId);

            Service.PluginLog.Debug($"{e.Instigator.Name} performed emote {emote?.Name.ExtractText() ?? "UNKNOWN"} ({e.EmoteId})" + (e.Target != null ? $" on target {e.Target.Name}" : string.Empty));

            var emoteLogItem = new EmoteLogItem
            {
                Instigator = e.Instigator,
                Target = e.Target,
                EmoteId = e.EmoteId,
                PlayerIsPerformer = isPerformer,
                PlayerIsTarget = isTarget,
                Timestamp = DateTime.Now,
            };

            foreach (var trigger in _emoteTriggerPool)
            {
                trigger.Queue(emoteLogItem);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private void OnShowUI(string command, string args)
        {
            // in response to the slash command, just display our main ui
            PluginUi.SettingsVisible = true;
        }
        // END EVENT HANDLERS

        // SOME FUNCTIONS THAT DO THINGS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternConfig">A pattern configuration.</param>
        public void DoPatternTest(dynamic patternConfig)
        {
            if (Status != ButtplugStatus.Connected)
            {
                return;
            }

            lock (_devicePool)
            {
                foreach (var device in this._devicePool)
                {
                    lock (device.Patterns)
                    {
                        device.Patterns.Add(PatternFactory.GetPatternFromObject(patternConfig));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The task associated with this method.</returns>
        private async Task DoScan()
        {
            try
            {
                await _buttplug!.StartScanningAsync();
            }
            catch (Exception ex)
            {
                Service.PluginLog.Error(ex, "Asynchronous scanning failed.");
            }
        }
        // END SOME FUNCTIONS THAT DO THINGS

        // START AND STOP FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        private async Task InitButtplug()
        {
            LastException = null;
            _status = ButtplugStatus.Connecting;

            if (_buttplug == null)
            {
                _buttplug = new ButtplugClient("AetherSense Redux");
                _buttplug.DeviceAdded += OnDeviceAdded;
                _buttplug.DeviceRemoved += OnDeviceRemoved;
                _buttplug.ScanningFinished += OnScanComplete;
                _buttplug.ServerDisconnect += OnServerDisconnect;
            }

            if (!Connected)
            {
                try
                {
                    _buttplugWebsocketConnector = new ButtplugWebsocketConnector(new Uri(Configuration.Address));
                    await _buttplug.ConnectAsync(_buttplugWebsocketConnector);
                    _ = DoScan();
                }
                catch (Exception ex)
                {
                    Service.PluginLog.Error(ex, "Buttplug failed to connect.");
                    LastException = ex;
                    Stop(false);
                }
            }

            if (Connected)
            {
                Service.PluginLog.Information("Buttplug connected.");
                _status = ButtplugStatus.Connected;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private async Task DisconnectButtplugAsync()
        {
            if (_buttplug?.Connected == true)
            {
                try
                {
                    if (_buttplugWebsocketConnector?.Connected ?? false)
                    {
                        Service.PluginLog.Debug("Websocket is still connected. Disconnecting...");
                        await _buttplugWebsocketConnector.DisconnectAsync();
                        _buttplugWebsocketConnector = null;
                        Service.PluginLog.Debug("Websocket disconnected");
                    }
                }
                catch (Exception ex)
                {
                    Service.PluginLog.Error(ex, "Failed to disconnect Buttplug websocket.");
                }

                try
                {
                    await _buttplug!.DisconnectAsync();
                    Service.PluginLog.Information("Buttplug disconnected.");
                }
                catch (Exception ex)
                {
                    Service.PluginLog.Error(ex, "Failed to disconnect Buttplug.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanDevices()
        {
            lock (_devicePool)
            {
                foreach (var device in _devicePool)
                {
                    Service.PluginLog.Debug("Stopping device {0}", device.Name);
                    device.Stop();
                    device.Dispose();
                }
                _devicePool.Clear();
            }
            Service.PluginLog.Debug("Devices destroyed.");
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task CleanButtplugAsync()
        {
            if (_buttplug == null)
            {
                _status = ButtplugStatus.Disconnected;
                return;
            }

            try
            {
                _status = ButtplugStatus.Disconnecting;
                await DisconnectButtplugAsync();
            }
            catch (Exception ex)
            {
                Service.PluginLog.Error(ex, $"Error thrown while trying to disconnect: {ex.Message}");
            }
            finally
            {
                _buttplug = null;
                _status = ButtplugStatus.Disconnected;
            }

            Service.PluginLog.Debug("Buttplug destroyed.");
        }

        private void CleanButtplug()
        {
            CleanButtplugAsync().Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanTriggers()
        {
            foreach (var t in _chatTriggerPool)
            {
                Service.PluginLog.Debug("Stopping chat trigger {0}", t.Name);
                t.Stop();
            }

            foreach (var t in _emoteTriggerPool)
            {
                Service.PluginLog.Debug("Stopping emote trigger {0}", t.Name);
                t.Stop();
            }
            Service.ChatGui.ChatMessage -= OnChatReceived;
            _chatTriggerPool.Clear();
            _emoteTriggerPool.Clear();
            Service.PluginLog.Debug("Triggers destroyed.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitTriggers()
        {
            foreach (var d in Configuration.Triggers)
            {
                // We pass DevicePool by reference so that triggers don't get stuck with outdated copies
                // of the device pool, should it be replaced with a new List<Device> - currently this doesn't
                // happen, but it's possible it may happen in the future.
                var trigger = TriggerFactory.GetTriggerFromConfig(d, ref _devicePool);
                if (trigger.Type == "ChatTrigger")
                {
                    _chatTriggerPool.Add((ChatTrigger)trigger);
                }
                else if (trigger.Type == "EmoteTrigger")
                {
                    _emoteTriggerPool.Add((EmoteTrigger)trigger);
                }
                else
                {
                    Service.PluginLog.Error("Invalid trigger type {0} created.", trigger.Type);
                }
            }

            foreach (var t in _chatTriggerPool)
            {
                Service.PluginLog.Debug("Starting chat trigger {0}", t.Name);
                t.Start();
            }

            foreach (var t in _emoteTriggerPool)
            {
                Service.PluginLog.Debug("Starting emote trigger {0}", t.Name);
                t.Start();
            }

            Service.ChatGui.ChatMessage += OnChatReceived;
            Service.PluginLog.Debug("Triggers created");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            InitTriggers();
            Task.Run(InitButtplug);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reload()
        {
            if (!Connected) return;
            CleanTriggers();
            InitTriggers();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop(bool expected)
        {
            CleanTriggers();
            CleanDevices();
            CleanButtplug();
        }
        // END START AND STOP FUNCTIONS

        // UI FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        private void DrawUI()
        {
            this.PluginUi.Draw();
        }
        private void DrawConfigUI()
        {
            PluginUi.SettingsVisible = !PluginUi.SettingsVisible;
        }
        // END UI FUNCTIONS

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<WaitType> DoBenchmark()
        {
            var result = WaitType.Slow_Timer;
            var times = new long[10];
            long sum = 0;
            var averages = new double[2];
            Stopwatch timer = new();
            Service.PluginLog.Information("Starting benchmark");


            Service.PluginLog.Debug("Testing Task.Delay");

            for (var i = 0; i < times.Length; i++)
            {
                timer.Restart();
                await Task.Delay(1);
                times[i] = timer.Elapsed.Ticks;
            }
            foreach (var t in times)
            {
                Service.PluginLog.Debug("{0}", t);
                sum += t;
            }
            averages[0] = (double)sum / times.Length / 10000;
            Service.PluginLog.Debug("Average: {0}", averages[0]);

            Service.PluginLog.Debug("Testing Thread.Sleep");
            times = new long[10];
            for (var i = 0; i < times.Length; i++)
            {
                timer.Restart();
                Thread.Sleep(1);
                times[i] = timer.Elapsed.Ticks;
            }
            sum = 0;
            foreach (var t in times)
            {
                Service.PluginLog.Debug("{0}", t);
                sum += t;
            }
            averages[1] = (double)sum / times.Length / 10000;
            Service.PluginLog.Debug("Average: {0}", averages[1]);

            if (averages[0] < 3)
            {
                result = WaitType.Use_Delay;

            }
            else if (averages[1] < 3)
            {
                result = WaitType.Use_Sleep;

            }

            switch (result)
            {
                case WaitType.Use_Delay:
                    Service.PluginLog.Information("High resolution Task.Delay found, using delay in timing loops.");
                    break;
                case WaitType.Use_Sleep:
                    Service.PluginLog.Information("High resolution Thread.Sleep found, using sleep in timing loops.");
                    break;
                case WaitType.Slow_Timer:
                default:
                    Service.PluginLog.Information("No high resolution, CPU-friendly waits available, timing loops will be inaccurate.");
                    break;
            }

            return result;

        }

    }
}
