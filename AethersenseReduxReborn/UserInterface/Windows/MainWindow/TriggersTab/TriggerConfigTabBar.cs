using System.Collections.Generic;
using AethersenseReduxReborn.Trigger;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggerConfigTabBar : ITabBar
{
    public dynamic? SelectedTrigger;

    public string           Name  => "Triggers Tab Bar";
    public ImGuiTabBarFlags Flags => ImGuiTabBarFlags.None;
    public List<ITab>       Tabs  { get; set; }

    public TriggerConfigTabBar(Plugin plugin, Configuration configuration)
    {
        Tabs = new List<ITab>
               {
                   new TriggerConfigBasicTab(this),
                   new TriggerConfigDevicesTab(this, configuration),
                   new TriggerConfigFilterTab(),
                   new TriggerConfigPatternTab(this, plugin),
               };
    }

    public void Draw()
    {
        using var tabBar = ImRaii.TabBar(Name, Flags);
        if (!tabBar)
            return;
        foreach (var tab in Tabs) {
            if (tab is TriggerConfigFilterTab) {
                if ((SelectedTrigger as ChatTriggerConfig) is { UseFilter: false })
                    continue;
            }

            tab.Draw();
        }
    }
}
