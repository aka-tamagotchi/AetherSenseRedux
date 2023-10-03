using System;
using System.Collections.Generic;
using System.Linq;
using AethersenseReduxReborn.SignalSources;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace AethersenseReduxReborn.Configurations;

public sealed class SignalConfiguration: ConfigurationBase, IDisposable
{
    public override int Version { get; set; } = 1;

    public required List<SignalGroup> SignalGroups { get; set; }

    public static SignalConfiguration DefaultConfiguration()
    {
        const string defaultTriggerName = "DefaultTrigger";
        const string defaultGroupName   = "DefaultCollection";

        var defaultGroup = new SignalGroup(defaultGroupName);
        defaultGroup.AddSignalSource(new ChatTriggerSignal(ChatTriggerSignalConfig.DefaultConfig()) {
                                                                                                        Name = defaultTriggerName,
                                                                                                    });


        return new SignalConfiguration {
                                           SignalGroups = new List<SignalGroup> {
                                                                                    defaultGroup,
                                                                                },
                                       };
    }

    public void Dispose()
    {
        foreach (var signalSource in SignalGroups.SelectMany(signalGroup => signalGroup.SignalSources)){
            signalSource.Dispose();
        }
    }
}
