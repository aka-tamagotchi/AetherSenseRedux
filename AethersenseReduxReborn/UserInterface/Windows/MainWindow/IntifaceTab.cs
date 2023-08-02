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

        //if (ImGui.Button(plugin.Scanning ? "Scanning..." : "Scan Now")){
        //    if (!plugin.Scanning)
        //    {
        //        Task.Run(plugin.DoScan);
        //    }
        //}

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
                ImGui.Indent();
                foreach (var device in _plugin.ConnectedDevices) {
                    ImGui.Text($"{device.Key} [{(int)device.Value}]");
                }

                ImGui.Unindent();
            }
        }
    }
}
