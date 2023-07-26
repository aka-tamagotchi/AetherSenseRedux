using System;

namespace AetherSenseRedux.Pattern;

internal class PatternFactory
{
    public static IPattern GetPatternFromObject(PatternConfig settings)
    {
        return settings.Type switch
        {
            "Constant" => new ConstantPattern((ConstantPatternConfig)settings),
            "Ramp"     => new RampPattern((RampPatternConfig)settings),
            "Saw"      => new SawPattern((SawPatternConfig)settings),
            "Random"   => new RandomPattern((RandomPatternConfig)settings),
            "Square"   => new SquarePattern((SquarePatternConfig)settings),
            _          => throw new ArgumentException($"Invalid pattern {settings.Type} specified")
        };
    }

    public static PatternConfig GetDefaultsFromString(string name)
    {
        return name switch
        {
            "Constant" => ConstantPattern.GetDefaultConfiguration(),
            "Ramp"     => RampPattern.GetDefaultConfiguration(),
            "Saw"      => SawPattern.GetDefaultConfiguration(),
            "Random"   => RandomPattern.GetDefaultConfiguration(),
            "Square"   => SquarePattern.GetDefaultConfiguration(),
            _          => throw new ArgumentException($"Invalid pattern {name} specified")
        };
    }

    public static PatternConfig GetPatternConfigFromObject(dynamic o)
    {
        return (string)o.Type switch
        {
            "Constant" => new ConstantPatternConfig { Duration = (long)o.Duration, Level = (double)o.Level },
            "Ramp" => new RampPatternConfig
                      {
                          Duration = (long)o.Duration, Start = (double)o.Start, End = (double)o.End
                      },
            "Saw" => new SawPatternConfig
                     {
                         Duration  = (long)o.Duration,
                         Start     = (double)o.Start,
                         End       = (double)o.End,
                         Duration1 = (long)o.Duration1,
                     },
            "Random" => new RandomPatternConfig
                        {
                            Duration = (long)o.Duration, Minimum = (double)o.Minimum, Maximum = (double)o.Maximum
                        },
            "Square" => new SquarePatternConfig
                        {
                            Duration  = (long)o.Duration,
                            Duration1 = (long)o.Duration1,
                            Duration2 = (long)o.Duration2,
                            Level1    = (double)o.Level1,
                            Level2    = (double)o.Level2,
                            Offset    = (long)o.Offset,
                        },
            _ => throw new ArgumentException(string.Format("Invalid pattern {0} specified", o.Type))
        };
    }
}