using Dalamud.Game.Command;
using Dalamud.Plugin;
using AethersenseReduxReborn.Configurations;
using AethersenseReduxReborn.Services;
using AethersenseReduxReborn.Services.Interfaces;
using AethersenseReduxReborn.UserInterface;
using AethersenseReduxReborn.UserInterface.Windows.MainWindow;

namespace AethersenseReduxReborn;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Aethersense Redux Reborn";

    private ButtplugServerConfiguration _serverConfiguration;
    private IntensityConfiguration      _intensityConfiguration;

    private const string CommandName = "/arr";

    private readonly CommandManager    _commandManager;
    private readonly IButtplugService  _buttplugService;
    private readonly IDeviceService    _deviceService;
    private readonly IIntensityService _intensityService;
    private readonly WindowManager     _windowManager;
    private readonly DalamudLogger     _dalamudLogger;


    public Plugin(
        DalamudPluginInterface pluginInterface,
        CommandManager         commandManager)
    {
        _dalamudLogger = new DalamudLogger();

        _serverConfiguration    = new ButtplugServerConfiguration();
        _intensityConfiguration = IntensityConfiguration.DefaultConfiguration();

        _commandManager = commandManager;
        _windowManager  = new WindowManager();

        _buttplugService  = new ButtplugService(_serverConfiguration, _dalamudLogger);
        _deviceService    = new DeviceService(_buttplugService, _dalamudLogger);
        _intensityService = new IntensityService(_buttplugService, _deviceService, _intensityConfiguration, _dalamudLogger);

        _windowManager.AddWindow(MainWindow.Name, new MainWindow(_buttplugService, _deviceService, _intensityService, _dalamudLogger));
        _commandManager.AddHandler(CommandName, new CommandInfo(OnShowUI)
                                                {
                                                    HelpMessage = "Opens the Aether Sense Redux configuration window",
                                                });

        pluginInterface.UiBuilder.Draw         += DrawUi;
        pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;
    }


    public void Dispose()
    {
        _buttplugService.DisconnectFromServer();
        _buttplugService.Dispose();
        _deviceService.Dispose();
        _windowManager.Dispose();
        _commandManager.RemoveHandler(CommandName);
    }

#region UI Handlers

    private void OnShowUI(string command, string args) { _windowManager.ToggleWindow(MainWindow.Name); }

    private void DrawUi() { _windowManager.Draw(); }

    private void DrawConfigUi() { _windowManager.ToggleWindow(MainWindow.Name); }

#endregion
}
