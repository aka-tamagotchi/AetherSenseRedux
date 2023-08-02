using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AethersenseReduxReborn.Pattern;
using AethersenseReduxReborn.Trigger;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggersTab : ITab
{
    private          int           _selectedTrigger;
    private readonly List<dynamic> _triggers;


    private readonly TriggerConfigTabBar _triggerConfigTabBar;

    public string Name => "Triggers";

    public TriggersTab(Plugin plugin, Configuration configuration)
    {
        _triggers            = configuration.Triggers;
        _triggerConfigTabBar = new TriggerConfigTabBar(plugin, configuration);
    }

    public void Draw()
    {
        using var triggersTab = ImRaii.TabItem("Triggers");
        if (!triggersTab)
            return;

        using (var leftOuterChild = ImRaii.Child("leftouter", new Vector2(155, 0))) {
            ImGui.Indent(1);
            using (var leftChild = ImRaii.Child("left", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), true)) {
                foreach (var (t, i) in _triggers.Select((value, i) => (value, i))) {
                    using var triggerId = ImRaii.PushId(i);
                    if (ImGui.Selectable(string.Format("{0} ({1})", t.Name, t.Type), _selectedTrigger == i)) {
                        _selectedTrigger = i;
                    }
                }
            }

            if (ImGui.Button("Add New")) {
                _triggers.Add(new ChatTriggerConfig()
                              {
                                  PatternSettings = new ConstantPatternConfig()
                              });
            }

            if (_triggers.Count != 0) {
                ImGui.SameLine();
                if (ImGui.Button("Remove")) {
                    _triggers.RemoveAt(_selectedTrigger);
                    if (_selectedTrigger >= _triggers.Count) {
                        _selectedTrigger = (_selectedTrigger > 0) ? _triggers.Count - 1 : 0;
                    }
                }
            }
        }

        ImGui.SameLine();

        using (var rightChild = ImRaii.Child("right", new Vector2(0, 0), false)) {
            ImGui.Indent(1);

            if (_triggers.Count == 0) {
                ImGui.Text("Use the Add New button to add a trigger.");
            } else {
                _triggerConfigTabBar.SelectedTrigger = _triggers[_selectedTrigger];
                _triggerConfigTabBar.Draw();
            }

            ImGui.Unindent();
        }
    }
}
