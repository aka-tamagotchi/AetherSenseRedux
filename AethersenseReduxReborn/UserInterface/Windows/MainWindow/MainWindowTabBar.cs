using System.Collections.Generic;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class MainWindowTabBar : ITabBar
{
    public string           Name  => "Main Window Tab Bar";
    public ImGuiTabBarFlags Flags => ImGuiTabBarFlags.None;
    public List<ITab>       Tabs  { get; set; }

    public MainWindowTabBar(Plugin plugin, Configuration configuration)
    {
        Tabs = new List<ITab>
               {
                   new IntifaceTab(plugin, configuration),
                   new TriggersTab.TriggersTab(plugin, configuration),
                   new AdvancedTab(configuration),
               };
    }


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
