using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using AethersenseReduxReborn.Pattern;
using AethersenseReduxReborn.Trigger;

namespace AethersenseReduxReborn;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int           Version { get; set; } = 2;
    public bool          FirstRun = true;
    public bool          LogChat     { get; set; } = false;
    public string        Address     { get; set; } = "ws://127.0.0.1:12345";
    public List<string>  SeenDevices { get; set; } = new();
    public List<dynamic> Triggers    { get; set; } = new List<dynamic>();

    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    /// <summary>
    /// Stores a reference to the plugin interface to allow us to save this configuration and reload it from disk.
    /// </summary>
    /// <param name="pluginInterface">The DalamudPluginInterface instance in this plugin</param>
    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    /// <summary>
    /// Deep copies the trigger list while ensuring that everything has the correct type.
    /// </summary>
    public void FixDeserialization()
    {
        List<TriggerConfig> triggers = new();
        foreach (dynamic t in Triggers)
        {
            triggers.Add(TriggerFactory.GetTriggerConfigFromObject(t));
        }
        Triggers = new List<dynamic>();

        foreach (TriggerConfig t in triggers)
        {
            Triggers.Add(t);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadDefaults()
    {
        Version  = 2;
        FirstRun = false;
        Triggers = new List<dynamic>() {
                                           new ChatTriggerConfig()
                                           {
                                               Name           = "Casted",
                                               EnabledDevices = new List<string>(),
                                               PatternSettings = new ConstantPatternConfig()
                                                                 {
                                                                     Level    = 1,
                                                                     Duration = 200
                                                                 },
                                               Regex          = "You cast",
                                               RetriggerDelay = 0
                                           },
                                           new ChatTriggerConfig()
                                           {

                                               Name           = "Casting",
                                               EnabledDevices = new List<string>(),
                                               PatternSettings = new RampPatternConfig()
                                                                 {
                                                                     Start    = 0,
                                                                     End      = 0.75,
                                                                     Duration = 2500
                                                                 },
                                               Regex          = "You begin casting",
                                               RetriggerDelay = 250
                                           }
                                       };
    }

    /// <summary>
    /// 
    /// </summary>
    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    public void Import(dynamic o)
    {
        try
        {
            if (o.Version != 2)
            {
                return;
            }
            FirstRun    = o.FirstRun;
            LogChat     = o.LogChat;
            Address     = o.Address;
            SeenDevices = new List<string>(o.SeenDevices);
            Triggers    = o.Triggers;
            FixDeserialization();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Attempted to import a bad configuration.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public Configuration CloneConfigurationFromDisk()
    {
        if (pluginInterface == null)
        {
            throw new NullReferenceException("Attempted to load the plugin configuration from a clone.");
        }
        var config = pluginInterface!.GetPluginConfig() as Configuration ?? throw new NullReferenceException("No configuration exists on disk.");
        config.FixDeserialization();
        return config;
    }
}