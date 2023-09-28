using System.Collections.Generic;
using System.Linq;
using AethersenseReduxReborn.IntensitySource.Interfaces;
using Dalamud.Logging;

namespace AethersenseReduxReborn.IntensitySource;

public class ScalarCollection
{
    private readonly List<ITimeBasedIntensitySource> _timeBasedIntensitySources = new();
    private readonly List<IDataIntensitySource>      _dataIntensitySources      = new();

    public string  Name        { get; set; }
    public Combine CombineType { get; set; }

    public List<IIntensitySource> IntensitySources
    {
        get
        {
            var list = new List<IIntensitySource>(_timeBasedIntensitySources);
            list.AddRange(_dataIntensitySources);
            return list;
        }
    }


    public ScalarCollection(string name) { Name = name; }

    public double UpdateSources(long elapsedMilliseconds)
    {
        var intensity = 0.0d;
        switch (CombineType) {
            case Combine.Average:
                intensity += _timeBasedIntensitySources.Sum(intensitySource => intensitySource.Update(elapsedMilliseconds))
                           + _dataIntensitySources.Sum(intensitySource => intensitySource.Update());
                intensity /= _timeBasedIntensitySources.Count + _dataIntensitySources.Count;
                break;
            case Combine.Max:
                intensity = double.Max(_timeBasedIntensitySources.Max(intensitySource => intensitySource.Update(elapsedMilliseconds)),
                                       _dataIntensitySources.Max(intensitySource => intensitySource.Update()));
                break;
            case Combine.Minimum:
                intensity = double.Min(_timeBasedIntensitySources.Min(intensitySource => intensitySource.Update(elapsedMilliseconds)),
                                       _dataIntensitySources.Min(intensitySource => intensitySource.Update()));
                break;
            default:
                intensity = 0;
                break;
        }

        return double.Clamp(intensity, 0, 1);
    }

    public void AddIntensitySource(IIntensitySource source)
    {
        PluginLog.Verbose("Adding new IntensitySource to ScalarCollection {0}", Name);
        switch (source) {
            case ITimeBasedIntensitySource intensitySource:
                _timeBasedIntensitySources.Add(intensitySource);
                break;
            case IDataIntensitySource intensitySource:
                _dataIntensitySources.Add(intensitySource);
                break;
        }
    }
}

public enum Combine
{
    Average,
    Max,
    Minimum,
}
