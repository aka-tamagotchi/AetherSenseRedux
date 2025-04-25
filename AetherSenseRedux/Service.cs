using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace AetherSenseRedux;

internal class Service
{
    public static Plugin Plugin = null!;
    public static IDalamudPluginInterface PluginInterface = null!;

    [PluginService]
    internal static ITextureProvider TextureProvider { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; } = null!;

    [PluginService]
    internal static IChatGui ChatGui { get; private set; } = null!;

    [PluginService]
    internal static IGameInteropProvider GameInteropProvider { get; set; } = null!;

    [PluginService]
    internal static IObjectTable ObjectTable { get; set; } = null!;

    [PluginService]
    internal static IClientState ClientState { get; set; } = null!;

    [PluginService]
    internal static IDataManager DataManager { get; set; } = null!;
}