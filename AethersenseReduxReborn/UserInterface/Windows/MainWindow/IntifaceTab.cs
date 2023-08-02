using System.Numerics;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class IntifaceTab : ITab
{
    private string _address;
    
    private readonly Plugin        _plugin;
    private readonly Configuration _configuration;

    public string Name => "Intiface";

    public IntifaceTab(Plugin plugin, Configuration configuration)
    {
        _plugin        = plugin;
        _configuration = configuration;

        _address = _configuration.Address;
    }
    
    public void Draw()
    {
        using var intifaceTab = ImRaii.TabItem("Intiface");
        if (!intifaceTab)
            return;

        var address = _address;
        if (ImGui.InputText("Intiface Address", ref address, 64)) {
            _address = address;
        }

        ImGui.SameLine();
        switch (_plugin.Status) {
            case ButtplugStatus.Connected:
            {
                if (ImGui.Button("Disconnect")) {
                    _plugin.Stop(true);
                }

                break;
            }
            case ButtplugStatus.Connecting:
            case ButtplugStatus.Disconnecting:
            {
                if (ImGui.Button("Wait...")) { }

                break;
            }
            case ButtplugStatus.Error:
            case ButtplugStatus.Uninitialized:
            case ButtplugStatus.Disconnected:
            default:
            {
                if (ImGui.Button("Connect")) {
                    _configuration.Address = _address;
                    _plugin.Start();
                }

                break;
            }
        }

        ImGui.Spacing();
        using var statusChild = ImRaii.Child("status", new Vector2(0, 0), true);
        if (statusChild) {
            if (_plugin.WaitType == WaitType.SlowTimer) {
                ImGui.TextColored(new Vector4(1, 0, 0, 1),
                                  "High resolution timers not available, patterns will be inaccurate.");
            }

            ImGui.Text("Connection Status:");
            ImGui.Indent();
            ImGui.Text(_plugin.Status == ButtplugStatus.Connected  ? "Connected" :
                       _plugin.Status == ButtplugStatus.Connecting ? "Connecting..." :
                       _plugin.Status == ButtplugStatus.Error      ? "Error" : "Disconnected");
            if (_plugin.LastException != null) {
                ImGui.Text(_plugin.LastException.Message);
            }

            ImGui.Unindent();
            if (_plugin.Status == ButtplugStatus.Connected) {
                ImGui.Text("Devices Connected:");

                foreach (var device in _plugin.DevicePool) {
                    ImGui.Text("-");
                    ImGui.SameLine();
                    ImGui.Indent();
                    ImGui.Text($"{device.Name} [{(int)device.Ups}]");
                    if (device.HasBattery)
                        ImGui.Text($"Battery: {device.Battery}");
                    if (device.HasVibrate) {
                        ImGui.Text("Vibrators:");
                        foreach (var attribute in device.Vibrators) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    if (device.HasRotate) {
                        ImGui.Text("Rotators:");
                        foreach (var attribute in device.Rotators) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    if (device.HasOscillate) {
                        ImGui.Text("Oscillators:");
                        foreach (var attribute in device.Oscillators) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    if (device.HasConstrict) {
                        ImGui.Text("Constrictors:");
                        foreach (var attribute in device.Constrictors) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    if (device.HasInflate) {
                        ImGui.Text("Inflators:");
                        foreach (var attribute in device.Inflators) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    if (device.HasPosition) {
                        ImGui.Text("Positioners:");
                        foreach (var attribute in device.Positioners) {
                            ImGui.Text($"{attribute.Index} - Description: {attribute.FeatureDescriptor}. Steps Available: {attribute.StepCount}");
                        }
                    }

                    ImGui.Unindent();
                }
            }
        }
    }
}
