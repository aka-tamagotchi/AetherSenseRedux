using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class ButtplugClientTab: TabBase
{
    private readonly ButtplugWrapper _buttplugWrapper;

    public override string Name => "Buttplug";

    public ButtplugClientTab(ButtplugWrapper buttplugWrapper) { _buttplugWrapper = buttplugWrapper; }

    protected override void DrawTab()
    {
        ConnectionStatus();
        ConnectionButtons();
        ImGui.Separator();
        ListDevicesAndActuators();
    }

    private void ConnectionStatus()
    {
        switch (_buttplugWrapper.Connected){
            case true:
                ImGui.Text("Connected to buttplug server.");
                break;
            case false:
                ImGui.Text("Click \"Connect\" to start.");
                break;
        }
    }

    private void ConnectionButtons()
    {
        if (_buttplugWrapper.Connected){
            if (ImGui.Button("Disconnect")) _buttplugWrapper.Disconnect().Start();
        } else{
            if (ImGui.Button("Connect")) _buttplugWrapper.Connect().Start();
        }
    }

    private void ListDevicesAndActuators()
    {
        foreach (var device in _buttplugWrapper.Devices){
            ImGui.Text(device.Name);
            ImGui.Indent();
            foreach (var actuator in device.Actuators){
                ImGui.Text($"{actuator.Index} - {actuator.ActuatorType} - {actuator.Description} - {actuator.Steps}");
            }

            ImGui.Unindent();
        }
    }
}
