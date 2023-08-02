using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface;

public interface ITab
{
    public string Name  { get; }

    public void Draw()
    {
        using var tab = ImRaii.TabItem(Name);
        if (!tab)
            return;
        ImGui.Text("Not implemented yet.");
    }
}
