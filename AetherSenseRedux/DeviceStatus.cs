namespace AetherSenseRedux;

/// <summary>
/// Publicly exposed information about a Device's current status.
/// </summary>
public class DeviceStatus
{
    public double UPS {get; init;}
    
    public double LastIntensity {get; init;}
}