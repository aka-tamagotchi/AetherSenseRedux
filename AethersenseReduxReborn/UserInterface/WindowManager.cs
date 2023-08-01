using System;
using System.Collections.Generic;
using Dalamud.Interface.Windowing;

namespace AethersenseReduxReborn.UserInterface;

public sealed class WindowManager : IDisposable
{
    private readonly Dictionary<string, Window> windows      = new();
    private readonly WindowSystem               windowSystem = new("Aethersense Redux Reborn");

    public void AddWindow(string name, Window window)
    {
        windows.Add(name, window);
        windowSystem.AddWindow(window);
    }

    public void OpenWindow(string name)
    {
        if (windows.TryGetValue(name, out var window))
            window.IsOpen = true;
    }

    public void CloseWindow(string name)
    {
        if (windows.TryGetValue(name, out var window))
            window.IsOpen = false;
    }

    public void ToggleWindow(string name)
    {
        if (windows.TryGetValue(name, out var window))
            window.IsOpen = !window.IsOpen;
    }

    public void Draw() { windowSystem.Draw(); }

    public void Dispose() { windowSystem.RemoveAllWindows(); }
}
