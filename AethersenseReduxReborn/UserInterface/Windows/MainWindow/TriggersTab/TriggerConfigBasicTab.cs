using AethersenseReduxReborn.Trigger;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggerConfigBasicTab : ITab
{
    private readonly TriggerConfigTabBar _triggerConfigTabBar;

    public string Name => "Basic";

    public TriggerConfigBasicTab(TriggerConfigTabBar triggerConfigTabBar)
    {
        _triggerConfigTabBar = triggerConfigTabBar;
    }

    public void Draw()
    {
        using var basicTab = ImRaii.TabItem("Basic");
        if (!basicTab)
            return;

        if (_triggerConfigTabBar.SelectedTrigger is null)
            return;

        var chatTriggerConfig = (ChatTriggerConfig)_triggerConfigTabBar.SelectedTrigger;

        //begin name field
        var name = chatTriggerConfig.Name;
        if (ImGui.InputText("Name", ref name, 64)) {
            chatTriggerConfig.Name = name;
        }
        //end name field

        //begin regex field
        var regex = chatTriggerConfig.Regex;
        if (ImGui.InputText("Regex", ref regex, 255)) {
            chatTriggerConfig.Regex = regex;
        }
        //end regex field

        //begin retrigger delay field
        var retriggerDelay = (int)chatTriggerConfig.RetriggerDelay;
        if (ImGui.InputInt("Retrigger Delay (ms)", ref retriggerDelay)) {
            chatTriggerConfig.RetriggerDelay = retriggerDelay;
        }

        //end retrigger delay field
        var useFilter = chatTriggerConfig.UseFilter;
        if (ImGui.Checkbox("Use Filters", ref useFilter)) {
            chatTriggerConfig.UseFilter = useFilter;
        }
    }
}
