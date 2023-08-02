using System.Collections.Generic;
using System.Linq;
using AethersenseReduxReborn.Trigger;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow.TriggersTab;

public class TriggerConfigDevicesTab : ITab
{
    private readonly TriggerConfigTabBar _triggerConfigTabBar;
    private readonly Configuration       _configuration;
    private readonly List<string>        _seenDevices;

    public string Name => "Devices";

    public TriggerConfigDevicesTab(TriggerConfigTabBar triggerConfigTabBar, Configuration configuration)
    {
        _triggerConfigTabBar = triggerConfigTabBar;
        _configuration       = configuration;
        _seenDevices         = _configuration.SeenDevices;
    }

    public void Draw()
    {
        using var devicesTab = ImRaii.TabItem("Devices");
        if (!devicesTab)
            return;

        if (_triggerConfigTabBar.SelectedTrigger is null)
            return;

        var triggerConfig = (TriggerConfig)_triggerConfigTabBar.SelectedTrigger;

        //Begin enabled devices selection
        if (_seenDevices.Count > 0) {
            var selected = new bool[_seenDevices.Count];
            var modified = false;
            foreach (var (device, j) in _seenDevices.Select((value, i) => (value, i))) {
                if (triggerConfig.EnabledDevices.Contains(device)) {
                    selected[j] = true;
                } else {
                    selected[j] = false;
                }
            }

            using var devicesListBox = ImRaii.ListBox("Enabled Devices");
            if (devicesListBox) {
                foreach (var (device, j) in _seenDevices.Select((value, i) => (value, i))) {
                    if (ImGui.Selectable(device, selected[j])) {
                        selected[j] = !selected[j];
                        modified    = true;
                    }
                }
            }

            if (modified) {
                var toEnable = new List<string>();
                foreach (var (device, j) in _seenDevices.Select((value, i) => (value, i))) {
                    if (selected[j]) {
                        toEnable.Add(device);
                    }
                }

                triggerConfig.EnabledDevices = toEnable;
            }
        } else {
            ImGui.Text("Connect to Intiface and connect devices to populate the list.");
        }
    }
}
