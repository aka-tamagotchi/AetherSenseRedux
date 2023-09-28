using System.Collections.Generic;
using System.Linq;
using AethersenseReduxReborn.ButtplugHelpers;
using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using Buttplug.Client;
using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class DevicesTab : TabBase
{
    private DeviceAttribute? _selectedAttribute;

    private List<DeviceDisplayListEntry>? _devicesToDisplay;

    private List<DeviceDisplayListEntry> DevicesToDisplay
    {
        get
        {
            if (_devicesToDisplay is not null)
                return _devicesToDisplay;

            _devicesToDisplay = new List<DeviceDisplayListEntry>();
            foreach (var device in _deviceService.Devices) {
                var newEntry = new DeviceDisplayListEntry(device.Name, _deviceService.DeviceAttributes
                                                                                     .Where(attribute => attribute.DeviceName == device.Name)
                                                                                     .ToList());
                newEntry.DeviceAttributes.Sort((a1, a2) => a1.Index.CompareTo(a2.Index));
                _devicesToDisplay.Add(newEntry);
            }

            return _devicesToDisplay;
        }
    }

    private readonly IButtplugService  _buttplugService;
    private readonly IDeviceService    _deviceService;
    private readonly IIntensityService _intensityService;
    private readonly DalamudLogger     _log;

    public override string Name => "Devices";

    public DevicesTab(IButtplugService buttplugService, IDeviceService deviceService, IIntensityService intensityService, DalamudLogger dalamudLogger)
    {
        _buttplugService  = buttplugService;
        _deviceService    = deviceService;
        _intensityService = intensityService;
        _log              = dalamudLogger;

        _buttplugService.DeviceAdded   += OnDeviceAdded;
        _buttplugService.DeviceRemoved += OnDeviceRemoved;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) {
            _log.Information("DevicesTab Dispose called"); // TODO: Remove this 
            _buttplugService.DeviceAdded   -= OnDeviceAdded;
            _buttplugService.DeviceRemoved -= OnDeviceRemoved;
        }

        base.Dispose(disposing);
    }

    protected override void DrawTab()
    {
        if (_buttplugService.ConnectionStatus is not ButtplugServerConnectionStatus.Connected) {
            ImGui.Text("Not connected to server.");
            return;
        }

        if (DevicesToDisplay.Count == 0) {
            ImGui.Text("No devices");
            _selectedAttribute = null;
        } else {
            _selectedAttribute ??= _deviceService.DeviceAttributes[0];

            foreach (var outputGroup in _intensityService.ScalarCollections) {
                using var id = ImRaii.PushId(outputGroup.Name);
                ImGui.Text(outputGroup.Name);
                ImGui.SameLine();
                using var attributeSelector = ImRaii.Combo("Device", $"{_selectedAttribute.DeviceName}:{_selectedAttribute.Index} - {_selectedAttribute.ActuatorType}");
                if (attributeSelector)
                    foreach (var attribute in _deviceService.DeviceAttributes) {
                        var isSelected = attribute == _selectedAttribute;
                        if (ImGui.Selectable($"{attribute.Index} - {attribute.Description} - {attribute.ActuatorType}", isSelected)) {
                            _selectedAttribute = attribute;
                            _intensityService.AssignOutputGroupToDeviceAttribute(outputGroup, _selectedAttribute);
                        }
                    }
            }

            foreach (var device in DevicesToDisplay) {
                ImGui.Text(device.DeviceName);
                ImGui.Indent();
                foreach (var attribute in device.DeviceAttributes) ImGui.Text($"{attribute.Index} - {attribute.Description} - {attribute.ActuatorType}");

                ImGui.Unindent();
            }
        }
    }

    private void OnDeviceAdded(object?   sender, DeviceAddedEventArgs   args) { _devicesToDisplay = null; }
    private void OnDeviceRemoved(object? sender, DeviceRemovedEventArgs args) { _devicesToDisplay = null; }
}

internal class DeviceDisplayListEntry
{
    public string                DeviceName       { get; init; }
    public List<DeviceAttribute> DeviceAttributes { get; init; }

    public DeviceDisplayListEntry(string deviceName, List<DeviceAttribute> deviceAttributes)
    {
        DeviceName       = deviceName;
        DeviceAttributes = deviceAttributes;
    }
}
