using System.Collections.Generic;
using System.Linq;

namespace AethersenseReduxReborn.SignalSources;

public class SignalGroup
{
    private bool   _enabled = true;
    private string _deviceName;
    private uint   _actuatorIndex;
    
    public string      Name        { get; set; }
    public CombineType CombineType { get; set; }
    public string DeviceName {
        get => _deviceName;
        set {
            Service.PluginLog.Verbose("Setting DeviceName of SignalGroup {0} to {1}", Name, value);
            _deviceName = value;
            Enabled     = true;
        }
    }
    public uint ActuatorIndex {
        get => _actuatorIndex;
        set {
            Service.PluginLog.Verbose("Setting ActuatorIndex of SignalGroup {0} to {1}", Name, value);
            _actuatorIndex = value;
            Enabled        = true;
        }
    }
    public double              Signal        { get; set; }
    public List<ISignalSource> SignalSources { get; } = new();
    public bool Enabled {
        get => _enabled;
        set {
            Service.PluginLog.Verbose(value ? "Enabling SignalGroup {0}" : "Disabling SignalGroup {0}", Name);
            _enabled = value;
        }
    }

    public SignalGroup(string name, string deviceName = "")
    {
        Name        = name;
        _deviceName = deviceName;
    }

    public void UpdateSources(double elapsedMilliseconds)
    {
        foreach (var signalSource in SignalSources){
            signalSource.Update(elapsedMilliseconds);
        }
        var activeSources = SignalSources.Where(source => source.Value > 0).ToList();
        if(activeSources.Count == 0)
        {
            Signal = 0;
            return;
        }
        var intensity = CombineType switch {
            CombineType.Average => activeSources.Sum(source => source.Value) / activeSources.Count,
            CombineType.Max     => activeSources.Max(source => source.Value),
            CombineType.Minimum => activeSources.Min(source => source.Value),
            _                   => 0,
        };
        if(double.IsNaN(intensity))
        {
            intensity = 0;
        }
        Signal = double.Clamp(intensity, 0, 1);
    }

    public void AddSignalSource(ISignalSource source)
    {
        Service.PluginLog.Verbose("Adding new SignalSource to SignalGroup {0}", Name);
        SignalSources.Add(source);
    }

    public void RemoveSignalSource(ISignalSource signalSource)
    {
        Service.PluginLog.Verbose("Removing SignalSource from SignalGroup {0}", Name);
        SignalSources.Remove(signalSource);
    }
}

public enum CombineType
{
    Average,
    Max,
    Minimum,
}
