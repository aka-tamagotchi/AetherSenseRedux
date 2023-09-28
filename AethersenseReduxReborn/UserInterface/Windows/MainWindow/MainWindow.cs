using System;
using System.Collections.Generic;
using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using Dalamud.Interface.Raii;
using Dalamud.Interface.Windowing;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public sealed class MainWindow : Window, IDisposable
{
    private readonly IButtplugService  _buttplugService;
    private readonly IDeviceService    _deviceService;
    private readonly IIntensityService _intensityService;
    private readonly DalamudLogger     _dalamudLogger;
    private readonly List<ITab>        _tabs;

    public static string Name => "Aethersense Redux Reborn";

    public MainWindow(IButtplugService buttplugService, IDeviceService deviceService, IIntensityService intensityService, DalamudLogger dalamudLogger)
        : base(Name)
    {
        _buttplugService  = buttplugService;
        _deviceService    = deviceService;
        _intensityService = intensityService;
        _dalamudLogger    = dalamudLogger;

        _tabs = new List<ITab>
                {
                    new ButtPlugTab(_buttplugService),
                    new DevicesTab(_buttplugService, _deviceService, _intensityService, _dalamudLogger),
                    new TriggersTab(_deviceService, _intensityService, _dalamudLogger),
                    new SourcesTab(_intensityService, _dalamudLogger),
                };
    }

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("Main Window Tab Bar");
        if (tabBar)
            for (var i = 0; i < _tabs.Count; i++) {
                using var id = ImRaii.PushId(i);
                _tabs[i].Draw();
            }
    }

    public void Dispose()
    {
        foreach (var tab in _tabs) tab.Dispose();
    }
}
