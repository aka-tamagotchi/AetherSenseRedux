using System;

namespace AethersenseReduxReborn.Pattern;

internal class SawPattern : IPattern
{
    public           DateTime Expires { get; set; }
    private readonly double   _startLevel;
    private readonly double   _endLevel;
    private readonly long     _duration1;


    public SawPattern(SawPatternConfig config)
    {
        _startLevel = config.Start;
        _endLevel   = config.End;
        var duration = config.Duration;
        _duration1 = config.Duration1;
        Expires    = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
    }

    public double GetIntensityAtTime(DateTime time)
    {
        if (Expires < time) {
            throw new PatternExpiredException();
        }

        var progress =
            1.0 - ((Expires.Ticks - time.Ticks) / ((double)_duration1 * 10000) %
                   1.0); // we only want the floating point remainder here
        return (_endLevel - _startLevel) * progress + _startLevel;
    }

    public static PatternConfig GetDefaultConfiguration() { return new SawPatternConfig(); }
}

[Serializable]
public class SawPatternConfig : PatternConfig
{
    public override string Type      { get; }      = "Saw";
    public          double Start     { get; set; } = 0;
    public          double End       { get; set; } = 1;
    public          long   Duration1 { get; set; } = 500;
}