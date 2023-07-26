
using System;
using System.Collections.Generic;

namespace AetherSenseRedux.Pattern;

internal class ConstantPattern : IPattern
{
    public           DateTime Expires { get; set; }
    private readonly double   _level;

    public ConstantPattern(ConstantPatternConfig config)
    {
        _level  = config.Level;
        Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
    }

    public double GetIntensityAtTime(DateTime time)
    {
        if (Expires < time)
        {
            throw new PatternExpiredException();
        }
        return _level;
    }
    public static PatternConfig GetDefaultConfiguration()
    {
        return new ConstantPatternConfig();
    }
}
[Serializable]
public class ConstantPatternConfig : PatternConfig
{
    public override string Type  { get; }      = "Constant";
    public          double Level { get; set; } = 1;
}