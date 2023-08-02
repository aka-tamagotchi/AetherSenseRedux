using System.Collections.Generic;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface;

public interface ITabBar
{
    public string           Name  { get; }
    public ImGuiTabBarFlags Flags { get; }
    public List<ITab>       Tabs  { get; set; }

    public void Draw()
    {
        using var tabBar = ImRaii.TabBar(Name, Flags);
        if (!tabBar)
            return;
        foreach (var tab in Tabs) {
            tab.Draw();
        }
    }
}
