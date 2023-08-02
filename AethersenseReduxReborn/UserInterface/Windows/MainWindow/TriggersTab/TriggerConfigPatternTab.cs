using AethersenseReduxReborn.Pattern;
using AethersenseReduxReborn.Trigger;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggerConfigPatternTab:ITab
{
    private readonly Plugin              _plugin;
    private readonly TriggerConfigTabBar _triggerConfigTabBar;
    
    public string Name => "Pattern";

    public TriggerConfigPatternTab(TriggerConfigTabBar triggerConfigTabBar, Plugin plugin)
    {
        _triggerConfigTabBar = triggerConfigTabBar;
        _plugin              = plugin;
    }

    public void Draw()
    {
        using var patternTab = ImRaii.TabItem("Pattern");
        if (!patternTab)
            return;
        if(_triggerConfigTabBar.SelectedTrigger is null) 
            return;
        
        var t = (TriggerConfig)_triggerConfigTabBar.SelectedTrigger;

        string[] patterns = { "Constant", "Ramp", "Random", "Square", "Saw" };

        //begin pattern selection
        if (ImGui.BeginCombo("##combo", t.PatternSettings!.Type)) {
            foreach (var pattern in patterns) {
                bool isSelected = t.PatternSettings.Type == pattern;
                if (ImGui.Selectable(pattern, isSelected)) {
                    if (t.PatternSettings.Type != pattern) {
                        t.PatternSettings = PatternFactory.GetDefaultsFromString(pattern);
                    }
                }

                if (isSelected) {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }
        //end pattern selection

        ImGui.SameLine();

        //begin test button
        if (ImGui.ArrowButton("test", ImGuiDir.Right)) {
            _plugin.DoPatternTest(t.PatternSettings);
        }

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip("Preview pattern on all devices.");
        }
        //end test button

        ImGui.Indent();

        //begin pattern settings
        switch ((string)t.PatternSettings.Type) {
            case "Constant":
                DrawConstantPatternSettings(t.PatternSettings);
                break;
            case "Ramp":
                DrawRampPatternSettings(t.PatternSettings);
                break;
            case "Saw":
                DrawSawPatternSettings(t.PatternSettings);
                break;
            case "Random":
                DrawRandomPatternSettings(t.PatternSettings);
                break;
            case "Square":
                DrawSquarePatternSettings(t.PatternSettings);
                break;
            default:
                //we should never get here but just in case
                ImGui.Text("Select a valid pattern.");
                break;
        }
        //end pattern settings

        ImGui.Unindent();
    }
    
    /// <summary>
    /// Draws the configuration interface for constant patterns
    /// </summary>
    /// <param name="pattern">A ConstantPatternConfig object containing the current configuration for the pattern.</param>
    private static void DrawConstantPatternSettings(dynamic pattern)
    {
        var duration = (int)pattern.Duration;
        if (ImGui.InputInt("Duration (ms)", ref duration)) {
            pattern.Duration = (long)duration;
        }

        var level = (double)pattern.Level;
        if (ImGui.InputDouble("Level", ref level)) {
            pattern.Level = level;
        }
    }

    /// <summary>
    /// Draws the configuration interface for ramp patterns
    /// </summary>
    /// <param name="pattern">A RampPatternConfig object containing the current configuration for the pattern.</param>
    private static void DrawRampPatternSettings(dynamic pattern)
    {
        var duration = (int)pattern.Duration;
        if (ImGui.InputInt("Duration (ms)", ref duration)) {
            pattern.Duration = (long)duration;
        }

        var start = (double)pattern.Start;
        if (ImGui.InputDouble("Start", ref start)) {
            pattern.Start = start;
        }

        var end = (double)pattern.End;
        if (ImGui.InputDouble("End", ref end)) {
            pattern.End = end;
        }
    }

    /// <summary>
    /// Draws the configuration interface for saw patterns
    /// </summary>
    /// <param name="pattern">A SawPatternConfig object containing the current configuration for the pattern.</param>
    private static void DrawSawPatternSettings(dynamic pattern)
    {
        var duration = (int)pattern.Duration;
        if (ImGui.InputInt("Duration (ms)", ref duration)) {
            pattern.Duration = (long)duration;
        }

        var start = (double)pattern.Start;
        if (ImGui.InputDouble("Start", ref start)) {
            pattern.Start = start;
        }

        var end = (double)pattern.End;
        if (ImGui.InputDouble("End", ref end)) {
            pattern.End = end;
        }

        var duration1 = (int)pattern.Duration1;
        if (ImGui.InputInt("Saw Duration (ms)", ref duration1)) {
            pattern.Duration1 = (long)duration1;
        }
    }

    /// <summary>
    /// Draws the configuration interface for random patterns
    /// </summary>
    /// <param name="pattern">A RandomPatternConfig object containing the current configuration for the pattern.</param>
    private static void DrawRandomPatternSettings(dynamic pattern)
    {
        var duration = (int)pattern.Duration;
        if (ImGui.InputInt("Duration (ms)", ref duration)) {
            pattern.Duration = (long)duration;
        }

        var min = (double)pattern.Minimum;
        if (ImGui.InputDouble("Minimum", ref min)) {
            pattern.Minimum = min;
        }

        var max = (double)pattern.Maximum;
        if (ImGui.InputDouble("Maximum", ref max)) {
            pattern.Maximum = max;
        }
    }

    /// <summary>
    /// Draws the configuration interface for square patterns
    /// </summary>
    /// <param name="pattern">A SquarePatternConfig object containing the current configuration for the pattern.</param>
    private static void DrawSquarePatternSettings(dynamic pattern)
    {
        var duration = (int)pattern.Duration;
        if (ImGui.InputInt("Duration (ms)", ref duration)) {
            pattern.Duration = (long)duration;
        }

        var level1 = (double)pattern.Level1;
        if (ImGui.InputDouble("Level 1", ref level1)) {
            pattern.Level1 = level1;
        }

        var duration1 = (int)pattern.Duration1;
        if (ImGui.InputInt("Level 1 Duration (ms)", ref duration1)) {
            pattern.Duration1 = (long)duration1;
        }

        var level2 = (double)pattern.Level2;
        if (ImGui.InputDouble("Level 2", ref level2)) {
            pattern.Level2 = level2;
        }

        var duration2 = (int)pattern.Duration2;
        if (ImGui.InputInt("Level 2 Duration (ms)", ref duration2)) {
            pattern.Duration2 = (long)duration2;
        }

        var offset = (int)pattern.Offset;
        if (ImGui.InputInt("Offset (ms)", ref offset)) {
            pattern.Offset = (long)offset;
        }
    }
}
