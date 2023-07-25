using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Buttplug;
using AetherSenseRedux.Trigger;
using AetherSenseRedux.Pattern;
using System.Diagnostics;
using System.Threading;

namespace AetherSenseRedux
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "AetherSense Redux";

        private const string commandName = "/asr";

        private ButtplugStatus _status;

        public ButtplugStatus Status { get
            {
                try
                {
                    if (Buttplug == null)
                    {
                        return ButtplugStatus.Uninitialized;
                    }
                    else if (Buttplug.Connected && _status == ButtplugStatus.Connected)
                    {
                        return ButtplugStatus.Connected;
                    }
                    else if (_status == ButtplugStatus.Connecting)
                    {
                        return ButtplugStatus.Connecting;
                    }
                    else if (!Buttplug.Connected && _status == ButtplugStatus.Connected)
                    {
                        return ButtplugStatus.Error;
                    } else if (_status == ButtplugStatus.Disconnecting)
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
                    PluginLog.Error(ex, "error when getting status");
                    return ButtplugStatus.Error;
                }

                
            }
        }

        public bool Scanning { get
            {
                if (Buttplug == null)
                {
                    return false;
                }
                return Buttplug.IsScanning;
            }
        }

        private bool Connected { 
            get {
                if (Buttplug != null)
                {
                    return Buttplug.Connected;
                }
                return false;
            } 
        }

        public Dictionary<string,double> ConnectedDevices { 
            get
            {
                Dictionary<string,double> result = new();
                foreach (Device device in DevicePool)
                {
                    result[device.Name] = device.UPS;
                }
                return result;
            } 
        }

        public Exception? LastException { get; set; }

        public WaitType WaitType { get; set; }
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; set; }
        [PluginService] private ChatGui ChatGui { get; init; } = null!;
        private PluginUi PluginUi { get; init; }

        private ButtplugClient? Buttplug;

        private List<Device> DevicePool;

        private readonly List<ChatTrigger> ChatTriggerPool;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginInterface"></param>
        /// <param name="commandManager"></param>
        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            var t = DoBenchmark();

            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            PluginInterface.Inject(this);

            this.DevicePool = new List<Device>();
            this.ChatTriggerPool = new List<ChatTrigger>();

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

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

            PluginUi = new PluginUi(Configuration, this);

            CommandManager.AddHandler(commandName, new CommandInfo(OnShowUI)
            {
                HelpMessage = "Opens the Aether Sense Redux configuration window"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

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
            CommandManager.RemoveHandler(commandName);
        }

        // EVENT HANDLERS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceAdded(object? sender, DeviceAddedEventArgs e)
        {

            PluginLog.Information("Device {0} added", e.Device.Name);
            Device newDevice = new(e.Device, WaitType);
            this.DevicePool.Add(newDevice);
            if (!Configuration.SeenDevices.Contains(newDevice.Name)){
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
            if(Status != ButtplugStatus.Connected)
            {
                return;
            }
            PluginLog.Information("Device {0} removed", e.Device.Name);
            var toRemove = new List<Device>();
            lock (this.DevicePool)
            {
                foreach (Device device in this.DevicePool)
                {
                    if (device.ClientDevice == e.Device)
                    {
                        try
                        {
                            device.Stop();
                        }
                        catch (Exception ex)
                        {
                            PluginLog.Error(ex, "Could not stop device {0}, device disconnected?", device.Name);
                        }
                        toRemove.Add(device);
                        device.Dispose();
                    }
                }
            }
            foreach (Device device in toRemove)
            {
                lock (this.DevicePool)
                {
                    this.DevicePool.Remove(device);
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
            PluginLog.Error("Unexpected disconnect.");
        }

        private void OnChatReceived(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            ChatMessage chatMessage = new(type, senderId, ref sender, ref message, ref isHandled);
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                t.Queue(chatMessage);
            }
            if (Configuration.LogChat)
            {
                PluginLog.Debug(chatMessage.ToString());
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

            lock (DevicePool) {
                foreach (var device in this.DevicePool)
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
        public async Task DoScan()
        {
            try
            {
                await Buttplug!.StartScanningAsync();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Asynchronous scanning failed.");
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

            if (Buttplug == null)
            {
                Buttplug = new ButtplugClient("AetherSense Redux");
                Buttplug.DeviceAdded += OnDeviceAdded;
                Buttplug.DeviceRemoved += OnDeviceRemoved;
                Buttplug.ScanningFinished += OnScanComplete;
                Buttplug.ServerDisconnect += OnServerDisconnect;
            }

            if (!Connected)
            {
                try
                {
                    ButtplugWebsocketConnectorOptions wsOptions = new(new Uri(Configuration.Address));
                    await Buttplug.ConnectAsync(wsOptions);
                    var t = DoScan();
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Buttplug failed to connect.");
                    LastException = ex;
                    Stop(false);
                }
            }

            if (Connected)
            {
                PluginLog.Information("Buttplug connected.");
                _status = ButtplugStatus.Connected;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void DisconnectButtplug()
        {
            if (Status == ButtplugStatus.Connected)
            {
                _status = ButtplugStatus.Disconnecting;
                try 
                {
                    var t = Buttplug!.DisconnectAsync();
                    t.Wait();
                    PluginLog.Information("Buttplug disconnected.");
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Buttplug failed to disconnect.");
                }
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanDevices()
        {
            lock (DevicePool)
            {
                foreach (Device device in DevicePool)
                {
                    PluginLog.Debug("Stopping device {0}", device.Name);
                    device.Stop();
                    device.Dispose();
                }
                DevicePool.Clear();
            }
            PluginLog.Debug("Devices destroyed.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanButtplug()
        {
            if (Buttplug == null)
            {
                _status = ButtplugStatus.Disconnected;
                return;
            }

            _status = ButtplugStatus.Disconnected;
            Buttplug.Dispose();
            Buttplug = null;
            PluginLog.Debug("Buttplug destroyed.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CleanTriggers()
        {
            foreach (ChatTrigger t in ChatTriggerPool)
            {
                PluginLog.Debug("Stopping chat trigger {0}", t.Name);
                t.Stop();
            }
            ChatGui.ChatMessage -= OnChatReceived;
            ChatTriggerPool.Clear();
            PluginLog.Debug("Triggers destroyed.");
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
                // happen but it's possible it may happen in the future.
                var Trigger = TriggerFactory.GetTriggerFromConfig(d, ref DevicePool);
                if (Trigger.Type == "ChatTrigger")
                {
                    ChatTriggerPool.Add((ChatTrigger)Trigger);
                } 
                else
                {
                    PluginLog.Error("Invalid trigger type {0} created.", Trigger.Type);
                }
            }

            foreach (ChatTrigger t in ChatTriggerPool)
            {
                PluginLog.Debug("Starting chat trigger {0}",t.Name);
                t.Start();
            }

            ChatGui.ChatMessage += OnChatReceived;
            PluginLog.Debug("Triggers created");
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
            if (Connected)
            {
                CleanTriggers();
                InitTriggers();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop(bool expected)
        {
            CleanTriggers();
            CleanDevices();
            if (expected)
            {
                DisconnectButtplug();
            }
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
        public static async Task<WaitType> DoBenchmark()
        {
            var result = WaitType.Slow_Timer;
            long[] times = new long[10];
            long sum = 0;
            double[] averages = new double[2];
            Stopwatch timer = new();
            PluginLog.Information("Starting benchmark");


            PluginLog.Debug("Testing Task.Delay");
            
            for (int i = 0; i < times.Length; i++)
            {
                timer.Restart();
                await Task.Delay(1);
                times[i] = timer.Elapsed.Ticks;
            }
            foreach (long t in times)
            {
                PluginLog.Debug("{0}", t);
                sum += t;
            }
            averages[0] = (double)sum / times.Length / 10000;
            PluginLog.Debug("Average: {0}", averages[0]);

            PluginLog.Debug("Testing Thread.Sleep");
            times = new long[10];
            for (int i = 0; i < times.Length; i++)
            {
                timer.Restart();
                Thread.Sleep(1);
                times[i] = timer.Elapsed.Ticks;
            }
            sum = 0;
            foreach (long t in times)
            {
                PluginLog.Debug("{0}", t);
                sum += t;
            }
            averages[1] = (double)sum / times.Length / 10000;
            PluginLog.Debug("Average: {0}", averages[1]);
            
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
                    PluginLog.Information("High resolution Task.Delay found, using delay in timing loops.");
                    break;
                case WaitType.Use_Sleep:
                    PluginLog.Information("High resolution Thread.Sleep found, using sleep in timing loops.");
                    break;
                default:
                    PluginLog.Information("No high resolution, CPU-friendly waits available, timing loops will be inaccurate.");
                    break;
            }

            return result;

        }
    }

    
}
