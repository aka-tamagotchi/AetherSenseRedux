using System;
using System.Text.Json;

namespace AethersenseReduxReborn.Configurations;

[Serializable]
public class ConfigurationBase : IConfiguration
{
    public delegate void                       ConfigurationChangedDelegate(object sender, EventArgs args);
    public event ConfigurationChangedDelegate? OnConfigurationChanged;

    public virtual int Version { get; set; }

    public ConfigurationBase DeepCopy() { return JsonSerializer.Deserialize<ConfigurationBase>(JsonSerializer.Serialize(this))!; }

    protected void ConfigurationChanged() { OnConfigurationChanged?.Invoke(this, EventArgs.Empty); }
}
