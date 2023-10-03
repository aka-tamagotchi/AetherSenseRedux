using System;
using System.Text.Json;

namespace AethersenseReduxReborn.Configurations;

public class ConfigurationBase
{
    public virtual  int                        Version { get; set; }
    public delegate void                       ConfigurationChangedDelegate(object sender, EventArgs args);
    public event ConfigurationChangedDelegate? OnConfigurationChanged;

    public ConfigurationBase DeepCopy() => JsonSerializer.Deserialize<ConfigurationBase>(JsonSerializer.Serialize(this))!;

    protected void ConfigurationChanged() { OnConfigurationChanged?.Invoke(this, EventArgs.Empty); }
}
