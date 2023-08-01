using System;

namespace AethersenseReduxReborn.Pattern;

internal class RampPattern : IPattern
{
    public           DateTime Expires { get; set; }
    private readonly double   _startLevel;
    private readonly double   _endLevel;
    private readonly long     _duration;


    public RampPattern(RampPatternConfig config)
    {
        _startLevel = config.Start;
        _endLevel   = config.End;
        _duration   = config.Duration;
        Expires     = DateTime.UtcNow + TimeSpan.FromMilliseconds(_duration);
    }

    public double GetIntensityAtTime(DateTime time)
    {
        if (Expires < time) {
            throw new PatternExpiredException();
        }

        var progress = 1.0 - ((Expires.Ticks - time.Ticks) / ((double)_duration * 10000));
        return (_endLevel  - _startLevel) * progress + _startLevel;
    }

    public static PatternConfig GetDefaultConfiguration() { return new RampPatternConfig(); }
}

[Serializable]
public class RampPatternConfig : PatternConfig
{
    public override string Type  { get; }      = "Ramp";
    public          double Start { get; set; } = 0;
    public          double End   { get; set; } = 1;
}