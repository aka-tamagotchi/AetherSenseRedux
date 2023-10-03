using System;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface;

public class TabBase: ITab
{
    public virtual string Name => "";

    public void Draw()
    {
        using var tab = ImRaii.TabItem(Name);
        if (!tab)
            return;
        DrawTab();
    }

    protected virtual void DrawTab() { ImGui.Text("Not implemented yet."); }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing){ }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
