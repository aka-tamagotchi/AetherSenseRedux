using System.Linq;
using AethersenseReduxReborn.IntensitySource;
using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class TriggersTab : TabBase
{
    private readonly IDeviceService    _deviceService;
    private readonly IIntensityService _intensityService;
    private readonly DalamudLogger     _dalamudLogger;

    public override string Name => "Triggers";

    public TriggersTab(IDeviceService deviceService, IIntensityService intensityService, DalamudLogger dalamudLogger)
    {
        _deviceService    = deviceService;
        _intensityService = intensityService;
        _dalamudLogger    = dalamudLogger;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { }

        base.Dispose(disposing);
    }

    protected override void DrawTab()
    {
        foreach (var intensitySource in _intensityService.ScalarCollections.SelectMany(scalarCollection => scalarCollection.IntensitySources)) {
            using var id = ImRaii.PushId(intensitySource.Name);
            ImGui.Separator();
            ImGui.Text($"{intensitySource.Name}");
            switch (intensitySource) {
                case ChatTrigger trigger:
                {
                    trigger.PatternConfig.DrawConfigOptions();
                    if (ImGui.Button("Test")) {
                        trigger.TriggerPattern();
                    }

                    break;
                }
            }
        }
    }
}
