using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AethersenseReduxReborn.ButtplugHelpers;
using AethersenseReduxReborn.Configurations;
using AethersenseReduxReborn.IntensitySource;
using AethersenseReduxReborn.Services.Interfaces;

namespace AethersenseReduxReborn.Services;

public class IntensityService : IIntensityService
{
    private CancellationTokenSource?                      _intensityCts;
    private List<ScalarCollection>                        _scalarCollections              = new();
    private Dictionary<ScalarCollection, DeviceAttribute> _scalarCollectionToAttributeMap = new();

    private IntensityConfiguration _intensityConfiguration;

    private readonly IButtplugService _buttplugService;
    private readonly IDeviceService   _deviceService;
    private readonly DalamudLogger    _log;

    private const int TargetLoopTime = 32;

    public List<ScalarCollection> ScalarCollections => _scalarCollections;

    public IntensityService(IButtplugService buttplugService, IDeviceService deviceService, IntensityConfiguration intensityConfiguration, DalamudLogger log)
    {
        _buttplugService        = buttplugService;
        _deviceService          = deviceService;
        _intensityConfiguration = intensityConfiguration;
        _log                    = log;

        _buttplugService.ConnectionStatusChanged       += ServerConnectionStatusChanged;
        _intensityConfiguration.OnConfigurationChanged += OnConfigurationChanged;

        ApplyConfiguration();
    }

    private void ApplyConfiguration()
    {
        foreach (var scalarCollection in _intensityConfiguration.ScalarCollections) AddNewOutputGroup(scalarCollection);

        foreach (var source in _intensityConfiguration.IntensitySources) _scalarCollections.Single(collection => collection.Name == source.ScalarCollectionName).AddIntensitySource(source);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _buttplugService.ConnectionStatusChanged       -= ServerConnectionStatusChanged;
        _intensityConfiguration.OnConfigurationChanged -= OnConfigurationChanged;
    }

    private void StartMainLoop()
    {
        _log.Information("Starting intensity main loop");
        if (_intensityCts is not null) {
            _intensityCts.Cancel();
            _intensityCts.Dispose();
        }

        _intensityCts = new CancellationTokenSource();
        MainLoop(_intensityCts.Token).ConfigureAwait(false);
    }

    private void CancelMainLoop()
    {
        _log.Information("Ending intensity main loop");
        _intensityCts?.Cancel();
    }

    private async Task MainLoop(CancellationToken cts)
    {
        var timer = new Stopwatch();

        while (true) {
            if (cts.IsCancellationRequested)
                return;
            var previousFrameTime = timer.ElapsedMilliseconds;
            timer.Restart();

            foreach (var (outputGroup, deviceAttribute) in _scalarCollectionToAttributeMap) 
                await _deviceService.SendScalarToAttribute(deviceAttribute, outputGroup.UpdateSources(previousFrameTime));

            var deviation = int.Max((int)previousFrameTime - TargetLoopTime, 0);
            _log.Verbose("frametime: {0}, deviation:{1}", previousFrameTime, deviation);
            await Task.Delay(int.Max(TargetLoopTime - deviation, 0), cts);
        }
    }

    private void ServerConnectionStatusChanged(object? sender, ButtplugConnectionStatusChangedEventArgs args)
    {
        switch (args.Status) {
            case ButtplugServerConnectionStatus.Connected:
                StartMainLoop();
                break;
            case ButtplugServerConnectionStatus.Disconnecting or ButtplugServerConnectionStatus.Disconnected:
                CancelMainLoop();
                break;
        }
    }

    public void AddNewOutputGroup(ScalarCollection scalarCollection)
    {
        _log.Information("Adding new output group: {0}", scalarCollection.Name);
        _scalarCollections.Add(scalarCollection);
    }

    public void AssignOutputGroupToDeviceAttribute(ScalarCollection scalarCollection, DeviceAttribute deviceAttribute)
    {
        _log.Information("Assigning ScalarCollection {0} to Device {1}, Index {2}", scalarCollection.Name, deviceAttribute.DeviceName, deviceAttribute.Index);
        if (_scalarCollectionToAttributeMap.ContainsKey(scalarCollection)) _scalarCollectionToAttributeMap.Remove(scalarCollection);

        _scalarCollectionToAttributeMap.Add(scalarCollection, deviceAttribute);
    }

    private void OnConfigurationChanged(object? sender, EventArgs args) { ApplyConfiguration(); }
}
