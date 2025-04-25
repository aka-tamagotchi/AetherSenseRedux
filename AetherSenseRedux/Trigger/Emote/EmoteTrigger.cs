using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AetherSenseRedux.Pattern;
using XIVChatTypes;

namespace AetherSenseRedux.Trigger.Emote;

internal class EmoteTrigger : ITrigger
{
    // ITrigger properties
    public bool Enabled { get; set; }
    public string Type { get; } = "EmoteTrigger";
    public string Name { get; init; }
    public List<Device> Devices { get; init; }
    public List<string> EnabledDevices { get; init; }
    public PatternConfig PatternSettings { get; init; }
    private EmoteTriggerConfig Configuration { get; init; }

    // ChatTrigger properties
    private ConcurrentQueue<EmoteLogItem> _emoteLog;
    public long RetriggerDelay { get; init; }
    private DateTime RetriggerTime { get; set; }
    private Guid Guid { get; set; }

    public EmoteTrigger(EmoteTriggerConfig configuration, ref List<Device> devices)
    {
        // ITrigger properties
        Enabled = true;
        Configuration = configuration;
        Name = configuration.Name;
        Devices = devices;
        EnabledDevices = configuration.EnabledDevices;
        PatternSettings = PatternFactory.GetPatternConfigFromObject(configuration.PatternSettings);

        RetriggerDelay = configuration.RetriggerDelay;

        _emoteLog = new ConcurrentQueue<EmoteLogItem>();
        RetriggerTime = DateTime.MinValue;
        Guid = Guid.NewGuid();
    }

    /// <summary>
    /// Adds a chat message to the trigger's processing queue.
    /// </summary>
    /// <param name="message">The chat message.</param>
    public void Queue(EmoteLogItem emote)
    {
        if (!Enabled) return;

        Service.PluginLog.Verbose("{0} ({1}): Received emote to queue", Name, Guid.ToString());
        _emoteLog.Enqueue(emote);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnTrigger()
    {
        if (RetriggerDelay > 0)
        {
            if (DateTime.UtcNow < RetriggerTime)
            {
                Service.PluginLog.Debug("Trigger discarded, too soon");
                return;
            }
            else
            {
                RetriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(RetriggerDelay);
            }
        }
        lock (Devices)
        {
            foreach (var device in Devices.Where(device => EnabledDevices.Contains(device.Name)))
            {
                lock (device.Patterns)
                {
                    device.Patterns.Add(PatternFactory.GetPatternFromObject(PatternSettings));
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        Task.Run(MainLoop).ConfigureAwait(false); ;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
        Enabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task MainLoop()
    {
        while (Enabled)
        {
            if (_emoteLog.TryDequeue(out var emoteItem))
            {
                Service.PluginLog.Verbose($"{Guid}: Processing emote log item: {emoteItem.Instigator.Name} performed {emoteItem.EmoteId} on {emoteItem.Target?.Name ?? "[no target]"}");
                if (!Configuration.EmoteIds.Contains(emoteItem.EmoteId)) continue;

                if (Configuration.TriggerOnPerform && emoteItem.PlayerIsPerformer)
                {
                    Service.PluginLog.Debug($"{Guid}: Triggered on emote: {emoteItem.EmoteId} performed by self ({emoteItem.Instigator.Name})");
                    OnTrigger();
                }
                else if (Configuration.TriggerOnTarget && emoteItem.PlayerIsTarget)
                {
                    Service.PluginLog.Debug($"{Guid}: Triggered on emote: {emoteItem.EmoteId} performed by player {emoteItem.Instigator.Name}");
                    OnTrigger();
                }
            }
            else
            {
                await Task.Delay(50);
            }
        }
    }
    static TriggerConfig GetDefaultConfiguration()
    {
        return new EmoteTriggerConfig();
    }
}