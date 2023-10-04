using System;
using System.Collections.Generic;
using System.Linq;
using AethersenseReduxReborn.Configurations;
using AethersenseReduxReborn.SignalSources;
using Dalamud.Plugin.Services;

namespace AethersenseReduxReborn;

public sealed class SignalService: IDisposable
{
    private readonly ButtplugWrapper     _buttplugWrapper;
    private readonly SignalConfiguration _signalConfiguration;
    public           List<SignalGroup>   SignalGroups { get; }

    public SignalService(ButtplugWrapper buttplugWrapper, SignalConfiguration signalConfiguration)
    {
        _buttplugWrapper         =  buttplugWrapper;
        _signalConfiguration     =  signalConfiguration;
        SignalGroups             =  signalConfiguration.SignalGroups;
        Service.Framework.Update += FrameworkUpdate;
    }

    public void SaveConfiguration() { _signalConfiguration.SignalGroups = SignalGroups; }

    public void Dispose() { Service.Framework.Update -= FrameworkUpdate; }

    private void FrameworkUpdate(IFramework framework)
    {
        if(_buttplugWrapper.Connected == false) 
            return;
        foreach (var signalGroup in SignalGroups.Where(signalGroup => signalGroup.Enabled)){
            signalGroup.UpdateSources(framework.UpdateDelta.TotalMilliseconds);
            try{
                _buttplugWrapper.SendCommandToDevice(signalGroup.DeviceName, signalGroup.ActuatorIndex, signalGroup.Signal);
            }
            catch(InvalidOperationException){
                signalGroup.Enabled = false;
            }
            catch (Exception e){
                Service.PluginLog.Warning("Exception while sending command to device {0}:\n{1}", signalGroup.DeviceName, e);
                signalGroup.Enabled = false;
            }
        }
    }
}
