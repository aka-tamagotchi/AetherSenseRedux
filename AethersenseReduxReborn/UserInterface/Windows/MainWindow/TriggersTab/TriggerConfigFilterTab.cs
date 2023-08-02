using AethersenseReduxReborn.XIVChatTypes;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggerConfigFilterTab : ITab
{
    private          int                 _selectedFilterCategory;

    public string Name => "Filters";

    public void Draw()
    {
        using var filtersTab = ImRaii.TabItem("Filters");
        if (!filtersTab)
            return;

        using var filterCategoryCombo =
            ImRaii.Combo("##filtercategory", XivChatFilter.FilterCategoryNames[_selectedFilterCategory]);
        if (filterCategoryCombo) {
            var k = 0;
            foreach (var name in XivChatFilter.FilterCategoryNames) {
                if (name == "GM Messages") {
                    // don't show the GM chat options for this filter configuration.
                    k++;
                    continue;
                }

                var isSelected = k == _selectedFilterCategory;
                if (ImGui.Selectable(name, isSelected)) {
                    _selectedFilterCategory = k;
                }

                k++;
            }

        }
    }
}
