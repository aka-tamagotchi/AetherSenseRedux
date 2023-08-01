using System;
using System.Collections.Generic;
using AetherSenseRedux.Pattern;
using Microsoft.CSharp.RuntimeBinder;

namespace AetherSenseRedux.Trigger;

internal class TriggerFactory
{
    public static ITrigger GetTriggerFromConfig(TriggerConfig config, ref List<Device> devices)
    {
        return config.Type switch
        {
            "Chat" => new ChatTrigger((ChatTriggerConfig)config, ref devices),
            _      => throw new ArgumentException(String.Format("Invalid trigger {0} specified", config.Type))
        };
    }

    public static TriggerConfig GetTriggerConfigFromObject(dynamic o)
    {
        var devices = new List<string>();
        foreach (var device in o.EnabledDevices) {
            devices.Add((string)device);
        }

        switch ((string)o.Type) {
            case "Chat":
                var size1 = 0;
                var size2 = 0;

                try {
                    try {
                        size1 = o.FilterTable.Count;
                    } catch (Exception) {
                        size1 = o.FilterTable.Length;
                    }

                    bool[][] filters = new bool[size1][];
                    for (var i = 0; i < filters.Length; i++) {
                        try {
                            size2 = o.FilterTable[i].Count;
                        } catch (Exception) {
                            size2 = o.FilterTable[i].Length;
                        }

                        filters[i] = new bool[size2];
                        for (var j = 0; j < filters[i].Length; j++) {
                            filters[i][j] = (bool)o.FilterTable[i][j];
                        }
                    }

                    return new ChatTriggerConfig()
                           {
                               Name            = (string)o.Name,
                               Regex           = (string)o.Regex,
                               RetriggerDelay  = (long)o.RetriggerDelay,
                               EnabledDevices  = devices,
                               PatternSettings = PatternFactory.GetPatternConfigFromObject(o.PatternSettings),
                               FilterTable     = filters,
                               UseFilter       = o.UseFilter
                           };
                } catch (RuntimeBinderException) {
                    // Handle version 1 configurations without filter tables that would otherwise crash the plugin
                    return new ChatTriggerConfig()
                           {
                               Name            = (string)o.Name,
                               Regex           = (string)o.Regex,
                               RetriggerDelay  = (long)o.RetriggerDelay,
                               EnabledDevices  = devices,
                               PatternSettings = PatternFactory.GetPatternConfigFromObject(o.PatternSettings)
                           };
                }
            default:
                throw new ArgumentException($"Invalid trigger {o.Type} specified");
        }
    }
}