using System;
using System.Collections.Generic;
using AethersenseReduxReborn.ButtplugHelpers;
using AethersenseReduxReborn.IntensitySource;

namespace AethersenseReduxReborn.Services.Interfaces;

public interface IIntensityService : IDisposable
{
    public List<ScalarCollection> ScalarCollections { get; }

    public void AddNewOutputGroup(ScalarCollection scalarCollection);

    public void AssignOutputGroupToDeviceAttribute(ScalarCollection scalarCollection, DeviceAttribute deviceAttribute);
}
