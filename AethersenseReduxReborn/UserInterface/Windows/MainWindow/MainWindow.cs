using System;
using System.Collections.Generic;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public sealed class MainWindow: Window, IDisposable
{
    private readonly List<ITab> _tabs;

    public static string Name => "Aethersense Redux Reborn";

    public MainWindow(ButtplugWrapper buttplugWrapper, SignalService signalService)
        : base(Name)
    {
        _tabs = new List<ITab> {
                                   new ButtplugClientTab(buttplugWrapper),
                                   new SignalGroupTab(buttplugWrapper, signalService),
                               };
    }

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("Main Window Tab Bar");
        if (!tabBar)
            return;
        for (var i = 0; i < _tabs.Count; i++){
            using var id = ImRaii.PushId(i);
            _tabs[i].Draw();
        }
    }

    public void Dispose()
    {
        foreach (var tab in _tabs){
            tab.Dispose();
        }
    }
}
