using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AethersenseReduxReborn.ButtplugHelpers;
using AethersenseReduxReborn.Services.Interfaces;
using Buttplug.Client;
using Buttplug.Core.Messages;

namespace AethersenseReduxReborn.Services;

public class DeviceService : IDeviceService
{
    private CancellationTokenSource? _deviceCts;

    private readonly List<DeviceAttribute>      _attributes = new();
    private readonly List<ButtplugClientDevice> _devices    = new();

    private readonly IButtplugService _buttplugService;
    private readonly DalamudLogger    _log;

    public List<ButtplugClientDevice> Devices          => _devices;
    public List<DeviceAttribute>      DeviceAttributes => _attributes;

    public DeviceService(IButtplugService buttplugService, DalamudLogger log)
    {
        _buttplugService = buttplugService;
        _log             = log;

        _buttplugService.ConnectionStatusChanged += ServerConnectionStatusChanged;
        _buttplugService.DeviceAdded             += DeviceAdded;
        _buttplugService.DeviceRemoved           += DeviceRemoved;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _deviceCts?.Cancel();
        _deviceCts?.Dispose();

        _buttplugService.ConnectionStatusChanged -= ServerConnectionStatusChanged;
        _buttplugService.DeviceAdded             -= DeviceAdded;
        _buttplugService.DeviceRemoved           -= DeviceRemoved;
    }


    private void DeviceAdded(object? sender, DeviceAddedEventArgs args) { AddDevice(args.Device); }

    private void DeviceRemoved(object? sender, DeviceRemovedEventArgs args) { RemoveDevice(args.Device); }

    private void ServerConnectionStatusChanged(object? sender, ButtplugConnectionStatusChangedEventArgs args)
    {
        switch (args.Status) {
            case ButtplugServerConnectionStatus.Disconnecting or ButtplugServerConnectionStatus.Disconnected:
                if (Devices.Count != 0) {
                    _log.Information("Server disconnect. Removing all devices.");
                    DeviceAttributes.RemoveAll(_ => true);
                    Devices.RemoveAll(_ => true);
                }
                break;
        }
    }

    private void AddDevice(ButtplugClientDevice device)
    {
        _devices.Add(device);

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Vibrate)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Rotate)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Oscillate)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Constrict)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Inflate)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Position)) _attributes.Add(new DeviceAttribute(device, attribute));

        foreach (var attribute in device.GenericAcutatorAttributes(ActuatorType.Unknown)) _attributes.Add(new DeviceAttribute(device, attribute));
    }

    private void RemoveDevice(ButtplugClientDevice device)
    {
        _devices.Remove(device);
        _attributes.RemoveAll(d => d.DeviceName == device.Name);
    }

    public async Task SendScalarToAttribute(DeviceAttribute attribute, double value)
    {
        await _devices.First(device => device.Name == attribute.DeviceName)
                      .ScalarAsync(new ScalarCmd.ScalarSubcommand(attribute.Index, value, attribute.ActuatorType));
    }
}
