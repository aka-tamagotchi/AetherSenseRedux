using System.Text.Json;

namespace AethersenseReduxReborn.Configurations;

public interface IConfiguration
{
    public int Version { get; set; }
}
