using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AethersenseReduxReborn.Configurations;
using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;

namespace AethersenseReduxReborn;

public sealed class ButtplugWrapper: IDisposable
{
    private readonly ButtplugServerConfiguration _serverConfiguration;

    private readonly ButtplugClient           _buttplugClient;
    private          CancellationTokenSource? _buttplugCts;

    public bool         Connected => _buttplugClient.Connected;
    public List<Device> Devices   { get; }

    public ButtplugWrapper(string name, ButtplugServerConfiguration serverConfiguration)
    {
        _serverConfiguration          =  serverConfiguration;
        _buttplugClient               =  new ButtplugClient(name);
        Devices                       =  new List<Device>();
        _buttplugClient.DeviceAdded   += DeviceAdded;
        _buttplugClient.DeviceRemoved += DeviceRemoved;
    }

    public void SendCommandToDevice(string name, uint index, double value)
    {
        try{
            Devices.Single(device => device.Name == name).SendCommandToActuator(index, value);
        } catch (InvalidOperationException e){
            Service.PluginLog.Error("Could not locate device with name {0}", name);
            throw;
        }
    }

    public async Task Connect()
    {
        _buttplugCts = new CancellationTokenSource();
        try{
            await _buttplugClient.ConnectAsync(new ButtplugWebsocketConnector(new Uri(_serverConfiguration.Address)),
                                               _buttplugCts.Token);
            Service.PluginLog.Information("Connected to server.");
        } catch (Exception ex){
            Service.PluginLog.Error(ex, "Failed to connect to buttplug server.");
        }
    }

    public async Task Disconnect()
    {
        await _buttplugClient.DisconnectAsync();
        _buttplugCts?.Cancel();
    }

    private void DeviceAdded(object? sender, DeviceAddedEventArgs args)
    {
        Service.PluginLog.Information("Device added: {0}", args.Device.Name);
        Devices.Add(new Device(args.Device));
    }

    private void DeviceRemoved(object? sender, DeviceRemovedEventArgs args)
    {
        try{
            Devices.Remove(Devices.Single(device => device.Name == args.Device.Name));
        } catch (Exception e){
            Service.PluginLog.Error("Unable to remove device from list of devices", e);
        }
    }

    public void Dispose()
    {
        _buttplugClient.DisconnectAsync();
        _buttplugClient.Dispose();
        _buttplugCts?.Dispose();
    }
}
