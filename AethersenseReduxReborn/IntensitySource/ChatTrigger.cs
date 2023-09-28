using AethersenseReduxReborn.IntensitySource.Interfaces;

namespace AethersenseReduxReborn.IntensitySource;

public class ChatTrigger : ITimeBasedIntensitySource
{
    private SimplePattern? _currentPattern;

    public SimplePatternConfig PatternConfig { get; set; } = SimplePatternConfig.DefaultConstantPattern();

    public required string Name                 { get; set; }
    public required string ScalarCollectionName { get; set; }

    public double Update(long elapsedMilliseconds)
    {
        var output = 0.0d;
        if (_currentPattern is not null)
            output = _currentPattern.Update(elapsedMilliseconds);
        if (_currentPattern is { IsCompleted: true })
            _currentPattern = null;
        return output;
    }

    public void TriggerPattern() { _currentPattern = SimplePattern.CreatePatternFromConfig(PatternConfig); }
}
