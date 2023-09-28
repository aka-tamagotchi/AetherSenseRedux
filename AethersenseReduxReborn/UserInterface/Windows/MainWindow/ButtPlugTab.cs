using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class ButtPlugTab : TabBase
{
    private readonly IButtplugService _buttplugService;

    public override string Name => "Buttplug";

    public ButtPlugTab(IButtplugService buttplugService) { _buttplugService = buttplugService; }


    protected override void DrawTab()
    {
        ImGui.Text($"Server status = {_buttplugService.ConnectionStatus}");
        if (_buttplugService.ConnectionStatus is not (ButtplugServerConnectionStatus.Connected or ButtplugServerConnectionStatus.Connecting))
            if (ImGui.Button("Connect"))
                _buttplugService.ConnectToServer();

        if (_buttplugService.ConnectionStatus is ButtplugServerConnectionStatus.Connected)
            if (ImGui.Button("Disconnect"))
                _buttplugService.DisconnectFromServer();
    }
}
