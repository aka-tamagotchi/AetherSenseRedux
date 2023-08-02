using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AethersenseReduxReborn.Pattern;
using AethersenseReduxReborn.Trigger;
using AethersenseReduxReborn.XIVChatTypes;
using Dalamud.Interface.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public sealed class MainWindow : Window, IDisposable
{
    private readonly Plugin        _plugin;
    private readonly Configuration _configuration;

    private readonly Vector2          _windowSize = new(640, 400);
    private const    ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar;

    private readonly MainWindowTabBar _mainWindowTabBar;


    public const string Name = "Aethersense Redux Reborn";

    // In order to keep the UI from trampling all over the configuration as changes are being made, we keep a working copy here when needed.
    //private Configuration? _workingCopy;

    public MainWindow(Plugin plugin, Configuration configuration)
        : base(Name)
    {
        _plugin        = plugin;
        _configuration = configuration;

        SizeConstraints = new WindowSizeConstraints
                          {
                              MinimumSize = _windowSize,
                              MaximumSize = _windowSize,
                          };

        Flags = WindowFlags;

        _mainWindowTabBar = new MainWindowTabBar(_plugin, _configuration);
    }

    public void Dispose() { }

    public override void Draw()
    {
        DrawMenuBar();
        ImGui.BeginChild("body", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), false);
        ImGui.Indent(1); //for some reason the UI likes to cut off a pixel on the left side if we don't do this
        _mainWindowTabBar.Draw();
        ImGui.Unindent(1); //for some reason the UI likes to cut off a pixel on the left side if we don't do this
        ImGui.EndChild();
        DrawFooter();
    }

    private static void DrawMenuBar()
    {
        if (!ImGui.BeginMenuBar())
            return;

        if (ImGui.BeginMenu("File")) {
            ImGui.MenuItem("Import...", "", false, false);
            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("NOT IMPLEMENTED");
            }

            ImGui.MenuItem("Export...", "", false, false);
            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("NOT IMPLEMENTED");
            }

            ImGui.EndMenu();
        }

        ImGui.EndMenuBar();
    }

    private void DrawFooter()
    {
        if (ImGui.Button("Save")) {
            _configuration.Save();
            _plugin.Reload();
        }

        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip("Save configuration changes to disk.");
        }
    }
}
