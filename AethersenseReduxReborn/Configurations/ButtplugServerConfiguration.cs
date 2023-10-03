namespace AethersenseReduxReborn.Configurations;

public sealed class ButtplugServerConfiguration: ConfigurationBase
{
    public override int Version { get; set; } = 1;

    public string Address { get; set; } = "ws://127.0.0.1:12345";
}
