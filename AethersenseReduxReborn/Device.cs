using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buttplug.Client;
using Buttplug.Core.Messages;

namespace AethersenseReduxReborn;

public class Device
{
    private readonly ButtplugClientDevice _internalDevice;

    public string               Name      => _internalDevice.Name;
    public List<DeviceActuator> Actuators { get; }

    public Device(ButtplugClientDevice internalDevice)
    {
        _internalDevice = internalDevice;
        var internalList = new List<GenericDeviceMessageAttributes>();
        foreach (ActuatorType actuatorType in Enum.GetValues(typeof(ActuatorType))){
            internalList.AddRange(_internalDevice.GenericAcutatorAttributes(actuatorType));
        }

        Actuators = internalList.Select(deviceActuator => new DeviceActuator {
                                                                                 Index        = deviceActuator.Index,
                                                                                 ActuatorType = deviceActuator.ActuatorType,
                                                                                 Description  = deviceActuator.FeatureDescriptor,
                                                                                 Steps        = deviceActuator.StepCount,
                                                                             })
                                .ToList();
    }

    public void SendCommandToActuator(uint index, double value)
    {
        var actuator       = Actuators.Single(actuator => actuator.Index == index);
        var processedValue = actuator.ProcessValue(value);

        // Only send the new value if it has changed enough to result in a new response from the device. 
        if (processedValue.Item2){
            Service.PluginLog.Debug("Sending value {0} to device {1} actuator {2}", processedValue.Item1, Name, actuator.Description);
            Task.Run(async () => await _internalDevice.ScalarAsync(new ScalarCmd.ScalarSubcommand(actuator.Index, processedValue.Item1, actuator.ActuatorType)));
//            _internalDevice.ScalarAsync(new ScalarCmd.ScalarSubcommand(actuator.Index, processedValue.Item1, actuator.ActuatorType));
        }
    }
}

public class DeviceActuator
{
    private         double       _previousValue;
    public required uint         Index        { get; init; }
    public required ActuatorType ActuatorType { get; init; }
    public required string       Description  { get; init; }
    public required uint         Steps        { get; init; }

    public (double, bool) ProcessValue(double value)
    {
        var quantized = double.Round(Steps * value) / Steps;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        // Exact comparison is intentional.
        if (quantized == _previousValue)
            return (quantized, false);
        Service.PluginLog.Debug("Quantized value {0} to {1}", value, quantized);
        _previousValue = quantized;
        return (quantized, true);
    }
}
