using System;
using Buttplug.Client;

namespace AethersenseReduxReborn.Services.Interfaces;

public interface IButtplugService : IDisposable
{
    public ButtplugServerConnectionStatus ConnectionStatus { get; }

    public delegate void ConnectionStatusChangedDelegate(object sender, ButtplugConnectionStatusChangedEventArgs args);
    public delegate void DeviceAddedDelegate(object             sender, DeviceAddedEventArgs                     args);
    public delegate void DeviceRemovedDelegate(object           sender, DeviceRemovedEventArgs                   args);

    public event ConnectionStatusChangedDelegate ConnectionStatusChanged;
    public event DeviceAddedDelegate             DeviceAdded;
    public event DeviceRemovedDelegate           DeviceRemoved;

    public void ConnectToServer();
    public void DisconnectFromServer();
}

public enum ButtplugServerConnectionStatus
{
    Connecting,
    Connected,
    Disconnecting,
    Disconnected,
    Unknown,
}

public class ButtplugConnectionStatusChangedEventArgs : EventArgs
{
    public required ButtplugServerConnectionStatus Status { get; init; }
}
