using Buttplug.Client;
using Buttplug.Core.Messages;

namespace AethersenseReduxReborn.ButtplugHelpers;

public class DeviceAttribute
{
    public string       DeviceName   { get; private set; }
    public uint         Index        { get; private set; }
    public ActuatorType ActuatorType { get; private set; }
    public uint         StepCount    { get; private set; }
    public string       Description  { get; private set; }

    public DeviceAttribute(ButtplugClientDevice device, GenericDeviceMessageAttributes attributes)
    {
        DeviceName   = device.Name;
        Index        = attributes.Index;
        ActuatorType = attributes.ActuatorType;
        StepCount    = attributes.StepCount;
        Description  = attributes.FeatureDescriptor;
    }
}
