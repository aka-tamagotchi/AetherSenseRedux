using System.Diagnostics.CodeAnalysis;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace AethersenseReduxReborn;

#pragma warning disable CS8618
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] public static IChatGui               ChatGui         { get; private set; }
    [PluginService] public static IClientState           ClientState     { get; private set; }
    [PluginService] public static IPartyList             PartyList       { get; private set; }
    [PluginService] public static ICommandManager        CommandManager  { get; private set; }
    [PluginService] public static ICondition             Condition       { get; private set; }
    [PluginService] public static IDataManager           DataManager     { get; private set; }
    [PluginService] public static IFramework             Framework       { get; private set; }
    [PluginService] public static IObjectTable           ObjectTable     { get; private set; }
    [PluginService] public static IGameGui               GameGui         { get; private set; }
    [PluginService] public static IDutyState             DutyState       { get; private set; }
    [PluginService] public static IPluginLog             PluginLog       { get; private set; }
}
#pragma warning restore CS8618
