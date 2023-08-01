using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AethersenseReduxReborn.Pattern;

namespace AethersenseReduxReborn.Trigger;

internal interface ITrigger
{
    bool   Enabled { get; set; }
    string Name    { get; init; }
    string Type    { get; }

    Task MainLoop();

}

[Serializable]
public abstract class TriggerConfig
{
    public abstract string       Type            { get; }
    public abstract string       Name            { get; set; }
    public          List<string> EnabledDevices  { get; set; } = new List<string>();
    public          dynamic?     PatternSettings { get; set; }
}