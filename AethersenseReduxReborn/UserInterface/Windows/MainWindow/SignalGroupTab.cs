using System;
using System.Numerics;
using AethersenseReduxReborn.SignalSources;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class SignalGroupTab: TabBase
{
    private SignalGroup?     _selectedSignalGroup;
    private SignalSourceType _newSignalSourceType = SignalSourceType.ChatTrigger;

    private readonly ButtplugWrapper _buttplugWrapper;
    private readonly SignalService   _signalService;

    public override string Name => "Signal Groups";

    public SignalGroupTab(ButtplugWrapper buttplugWrapper, SignalService signalService)
    {
        _buttplugWrapper = buttplugWrapper;
        _signalService   = signalService;
    }

    protected override void DrawTab()
    {
        var availableRegion = ImGui.GetContentRegionAvail();

        DrawSignalGroupList();
        ImGui.SameLine();
        DrawSelectedGroup();
        return;

        void DrawSignalGroupList()
        {
            using var listChild = ImRaii.Child("###GroupListChild", new Vector2(availableRegion.X * 0.25f, 0), true);
            if (!listChild)
                return;
            if (_signalService.SignalGroups.Count == 0)
                return;
            var listRegion = new Vector2 {
                                             X = ImGui.GetContentRegionAvail().X,
                                             Y = ImGui.GetContentRegionAvail().Y / ImGui.GetTextLineHeightWithSpacing() * ImGui.GetTextLineHeightWithSpacing(),
                                         };
            using var groupList = ImRaii.ListBox("###SignalGroupList", listRegion);
            if (!groupList)
                return;
            foreach (var signalGroup in _signalService.SignalGroups)
                if (ImGui.Selectable(signalGroup.Name, _selectedSignalGroup == signalGroup))
                    _selectedSignalGroup = _selectedSignalGroup             == signalGroup ? null : signalGroup;
        }

        void DrawSelectedGroup()
        {
            using var configChild = ImRaii.Child("###ConfigChild", Vector2.Zero, true);
            if (!configChild)
                return;
            if (_selectedSignalGroup is null)
                return;

            DrawSignalGroup(_selectedSignalGroup);
        }
    }

    private void DrawSignalGroup(SignalGroup signalGroup)
    {
        using var id = ImRaii.PushId(signalGroup.Name);
        ImGui.Text(signalGroup.Name);
        DrawActuatorCombo();
        ImGui.NewLine();
        ImGui.Text($"Intensity: {signalGroup.Signal}");
        DrawCombineType();
        ImGui.NewLine();
        ImGui.Text("New signal source: ");
        ImGui.SameLine();
        using (var newSignalSourceCombo = ImRaii.Combo("NewSignalSourceType", _newSignalSourceType.DisplayString(), ImGuiComboFlags.NoArrowButton)){
            if (newSignalSourceCombo){
                foreach (var signalSourceType in Enum.GetValues<SignalSourceType>()){
                    if (ImGui.Selectable(signalSourceType.DisplayString(), signalSourceType == _newSignalSourceType))
                        _newSignalSourceType = signalSourceType;
                }
            }
        }
        if (ImGui.Button("Add"))
            switch (_newSignalSourceType){
                case SignalSourceType.ChatTrigger:
                    signalGroup.AddSignalSource(new ChatTriggerSignal(ChatTriggerSignalConfig.DefaultConfig()) {
                                                                                                                   Name = "New signal source",
                                                                                                               });
                    break;
                case SignalSourceType.PlayerAttribute:
                    signalGroup.AddSignalSource(new PlayerAttributeSignal(PlayerAttributeSignalConfig.DefaultConfig()) {
                                                                                                                           Name = "New signal source",
                                                                                                                       });
                    break;
                default:
                    Service.PluginLog.Error("Tried to add unknown signal source type", _newSignalSourceType);
                    break;
            }

        var signalGroupId = 0;
        foreach (var signalSource in signalGroup.SignalSources){
            ImRaii.PushId(signalGroupId);
            signalSource.DrawConfig();
            if (ImGui.Button("Remove"))
                _selectedSignalGroup.RemoveSignalSource(signalSource);
            ImGui.Separator();
            signalGroupId++;
        }

        
        return;

        void DrawActuatorCombo()
        {
            using var actuatorCombo = ImRaii.Combo("Actuator", $"{signalGroup.DeviceName} - {signalGroup.ActuatorIndex}", ImGuiComboFlags.NoArrowButton);
            if (!actuatorCombo)
                return;
            foreach (var device in _buttplugWrapper.Devices){
                foreach (var actuator in device.Actuators){
                    if (!ImGui.Selectable($"{actuator.Index} - {actuator.Description}", actuator.Index == signalGroup.ActuatorIndex))
                        continue;
                    signalGroup.DeviceName    = device.Name;
                    signalGroup.ActuatorIndex = actuator.Index;
                }
            }
        }

        void DrawCombineType()
        {
            ImGui.Text("CombineType: ");
            ImGui.SameLine();
            using var combineTypeCombo = ImRaii.Combo("CombineType", signalGroup.CombineType.ToString(), ImGuiComboFlags.NoArrowButton);
            if (!combineTypeCombo)
                return;
            foreach (var combineType in Enum.GetValues<CombineType>()){
                if (ImGui.Selectable(combineType.ToString(), combineType == signalGroup.CombineType))
                    signalGroup.CombineType = combineType;
            }
        }
    }
}
