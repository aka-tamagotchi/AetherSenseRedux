using System.Collections.Generic;
using System.Linq;

namespace AethersenseReduxReborn.SignalSources;

public class SignalGroup
{
    public string              Name          { get; set; }
    public CombineType         CombineType   { get; set; }
    public string              DeviceName    { get; set; }
    public uint                ActuatorIndex { get; set; }
    public double              Signal        { get; set; }
    public List<ISignalSource> SignalSources { get; } = new();

    public SignalGroup(string name, string deviceName = "")
    {
        Name       = name;
        DeviceName = deviceName;
    }

    public void UpdateSources(double elapsedMilliseconds)
    {
        foreach (var signalSource in SignalSources){
            signalSource.Update(elapsedMilliseconds);
        }
        var activeSources = SignalSources.Where(source => source.Value > 0).ToList();
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
}

public enum CombineType
{
    Average,
    Max,
    Minimum,
}
