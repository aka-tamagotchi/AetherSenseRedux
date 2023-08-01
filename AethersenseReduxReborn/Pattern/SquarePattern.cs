using System;

namespace AethersenseReduxReborn.Pattern;

internal class SquarePattern : IPattern
{
    public           DateTime Expires { get; set; }
    private readonly double   _level1;
    private readonly double   _level2;
    private readonly long     _duration1;
    private readonly long     _offset;
    private readonly long     _totalDuration;


    public SquarePattern(SquarePatternConfig config)
    {
        _level1        = config.Level1;
        _level2        = config.Level2;
        _duration1     = config.Duration1;
        _offset        = config.Offset;
        Expires        = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
        _totalDuration = _duration1      + config.Duration2;
    }

    public double GetIntensityAtTime(DateTime time)
    {
        if (Expires < time) {
            throw new PatternExpiredException();
        }

        var patternTime = DateTime.UtcNow.Ticks / 10000 + _offset;

        var progress = patternTime % _totalDuration;

        return (progress < _duration1) ? _level1 : _level2;
    }

    public static PatternConfig GetDefaultConfiguration() { return new SquarePatternConfig(); }
}

[Serializable]
public class SquarePatternConfig : PatternConfig
{
    public override string Type      { get; }      = "Square";
    public          double Level1    { get; set; } = 0;
    public          double Level2    { get; set; } = 1;
    public          long   Duration1 { get; set; } = 200;
    public          long   Duration2 { get; set; } = 200;
    public          long   Offset    { get; set; } = 0;
}