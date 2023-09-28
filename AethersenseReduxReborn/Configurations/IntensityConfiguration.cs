using System.Collections.Generic;
using AethersenseReduxReborn.IntensitySource;
using AethersenseReduxReborn.IntensitySource.Interfaces;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace AethersenseReduxReborn.Configurations;

public class IntensityConfiguration : ConfigurationBase
{
    public int Version { get; set; } = 1;

    public required List<IIntensitySource> IntensitySources  { get; set; }
    public required List<ScalarCollection> ScalarCollections { get; set; }

    public static IntensityConfiguration DefaultConfiguration()
    {
        const string defaultTriggerName    = "DefaultTrigger";
        const string defaultCollectionName = "DefaultCollection";

        return new IntensityConfiguration
               {
                   IntensitySources = new List<IIntensitySource>
                                      {
                                          new ChatTrigger
                                          {
                                              Name                 = defaultTriggerName,
                                              ScalarCollectionName = defaultCollectionName,
                                          },
                                      },
                   ScalarCollections = new List<ScalarCollection>
                                       {
                                           new(defaultCollectionName),
                                       },
               };
    }
}
