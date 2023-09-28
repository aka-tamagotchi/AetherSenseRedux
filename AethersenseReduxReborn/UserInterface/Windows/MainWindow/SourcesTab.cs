using System.Collections.Generic;
using AethersenseReduxReborn.IntensitySource;
using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class SourcesTab : TabBase
{
    private readonly IIntensityService _intensityService;
    private readonly DalamudLogger     _dalamudLogger;

    public override string Name => "Source";

    public SourcesTab(IIntensityService intensityService, DalamudLogger dalamudLogger)
    {
        _intensityService = intensityService;
        _dalamudLogger    = dalamudLogger;
    }

    protected override void DrawTab() { }
}
