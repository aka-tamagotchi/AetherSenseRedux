﻿using System;

namespace AetherSenseRedux.Pattern;

internal class RandomPattern : IPattern
{
    public           DateTime Expires { get; set; }
    private readonly Random   _rand = new();
    private readonly double   _min;
    private readonly double   _max;

    public RandomPattern(RandomPatternConfig config)
    {
        Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
        _min    = config.Minimum;
        _max    = config.Maximum;
    }

    public double GetIntensityAtTime(DateTime time)
    {
        if (Expires < time) {
            throw new PatternExpiredException();
        }

        return Scale(_rand.NextDouble(), _min, _max);
    }

    private static double Scale(double value, double min, double max) { return value * (max - min) + min; }

    public static PatternConfig GetDefaultConfiguration() { return new RandomPatternConfig(); }
}

[Serializable]
public class RandomPatternConfig : PatternConfig
{
    public override string Type    { get; }      = "Random";
    public          double Minimum { get; set; } = 0;
    public          double Maximum { get; set; } = 1;
}