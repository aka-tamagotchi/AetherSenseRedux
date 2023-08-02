using System;

namespace AethersenseReduxReborn.Pattern;

public interface IPattern
{
    DateTime Expires { get; set; }
    double   GetIntensityAtTime(DateTime currTime);

    static PatternConfig GetDefaultConfiguration() { throw new NotImplementedException(); }
}

[Serializable]
public abstract class PatternConfig
{
    public abstract string Type     { get; }
    public          long   Duration { get; set; } = 1000;
}