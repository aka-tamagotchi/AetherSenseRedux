using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.SignalSources;

[Serializable]
public class SimplePattern
{
    private double _elapsedTime;

    private readonly SimplePatternType _patternType;

    private readonly long   _totalDuration;
    private readonly long   _duration1;
    private readonly long   _duration2;
    private readonly double _intensity1;
    private readonly double _intensity2;

    public bool IsCompleted { get; set; }

    public SimplePattern(SimplePatternType patternType, long totalDuration, long duration1, long duration2, double intensity1, double intensity2)
    {
        _patternType   = patternType;
        _totalDuration = totalDuration;
        _duration1     = duration1;
        _duration2     = duration2;
        _intensity1    = intensity1;
        _intensity2    = intensity2;
    }

    public double Update(double elapsedMilliseconds)
    {
        _elapsedTime += elapsedMilliseconds;
        var weight = double.Clamp(_elapsedTime / _totalDuration, 0, 1);

        if (_elapsedTime >= _totalDuration)
            IsCompleted = true;
        
        var value = _patternType switch {
            SimplePatternType.Constant => _intensity1,
            SimplePatternType.Ramp     => Lerp(_intensity1, _intensity2, weight),
            SimplePatternType.Saw      => Lerp(_intensity1, _intensity2, weight % 1),
            SimplePatternType.Square   => _elapsedTime                          % _duration1 + _duration2 < _duration1 ? _intensity1 : _intensity2,
            SimplePatternType.Random   => Lerp(_intensity1, _intensity2, Random.Shared.NextDouble()),
            _                          => 0,
        };
        Service.PluginLog.Verbose("time:{0}, weight:{1}, value:{2}", _elapsedTime, weight, value);
        return value;
    }

    private static T Lerp<T>(T start, T end, T weight)
        where T: INumber<T>, INumberBase<T> =>
        start * (T.One - weight) + end * weight;

    public static SimplePattern CreatePatternFromConfig(SimplePatternConfig patternConfig) =>
        new(patternConfig.PatternType,
            patternConfig.TotalDuration,
            patternConfig.Duration1,
            patternConfig.Duration2,
            patternConfig.Intensity1,
            patternConfig.Intensity2);
}

public class SimplePatternConfig
{
    private int    _totalDuration;
    private int    _duration1;
    private int    _duration2;
    private double _intensity1;
    private double _intensity2;

    public SimplePatternType PatternType { get; set; }

    public int TotalDuration {
        get => _totalDuration;
        init => _totalDuration = value;
    }

    public int Duration1 {
        get => _duration1;
        init => _duration1 = value;
    }

    public int Duration2 {
        get => _duration2;
        init => _duration2 = value;
    }

    public double Intensity1 {
        get => _intensity1;
        init => _intensity1 = value;
    }

    public double Intensity2 {
        get => _intensity2;
        init => _intensity2 = value;
    }

    public void DrawConfigOptions()
    {
        const ImGuiComboFlags patternTypeSelectorFlags = ImGuiComboFlags.PopupAlignLeft;
        using (var patternTypeSelector = ImRaii.Combo("Pattern Type", PatternType.ToString(), patternTypeSelectorFlags)){
            if (patternTypeSelector)
                foreach (var patternType in Enum.GetValues<SimplePatternType>()){
                    var isSelected = patternType == PatternType;

                    if (ImGui.Selectable(patternType.ToString(), isSelected))
                        PatternType = patternType;
                }
        }

        InputDuration("Total Duration (ms)", ref _totalDuration);

        switch (PatternType){
            case SimplePatternType.Constant:
                InputIntensity("Intensity", ref _intensity1);
                break;
            case SimplePatternType.Ramp:
                InputIntensity("Start Intensity", ref _intensity1);
                InputIntensity("End Intensity",   ref _intensity2);
                break;
            case SimplePatternType.Saw:
                InputIntensity("Low Intensity",  ref _intensity1);
                InputIntensity("High Intensity", ref _intensity2);
                break;
            case SimplePatternType.Square:
                InputIntensity("Low Intensity", ref _intensity1);
                InputDuration("Low Duration (ms)", ref _duration1);
                InputIntensity("High Intensity", ref _intensity2);
                InputDuration("High Duration (ms)", ref _duration2);
                break;
            case SimplePatternType.Random:
                InputIntensity("Lowest Intensity",  ref _intensity1);
                InputIntensity("Highest Intensity", ref _intensity2);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return;

        void InputIntensity(string label, ref double intensity)
        {
            var newValue                                                                                       = (float)intensity;
            if (ImGui.SliderFloat(label, ref newValue, 0, 1, "%1.2f", ImGuiSliderFlags.AlwaysClamp)) intensity = newValue;
        }

        void InputDuration(string label, ref int duration)
        {
            var newValue                                          = duration;
            if (ImGui.InputInt(label, ref newValue, 50)) duration = int.Clamp(newValue, 0, int.MaxValue);
        }
    }

    public static SimplePatternConfig DefaultConstantPattern() =>
        new() {
                  TotalDuration = 250,
                  Intensity1    = 1,
              };
}

public enum SimplePatternType
{
    Constant,
    Ramp,
    Saw,
    Square,
    Random,
}
