using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AethersenseReduxReborn.ButtplugHelpers;
using Buttplug.Client;

namespace AethersenseReduxReborn.Services.Interfaces;

public interface IDeviceService : IDisposable
{
    public List<ButtplugClientDevice> Devices          { get; }
    public List<DeviceAttribute>      DeviceAttributes { get; }

    public Task SendScalarToAttribute(DeviceAttribute attribute, double value);
}
