using System;
using System.Collections.Generic;
using Dalamud.Interface.Windowing;

namespace AethersenseReduxReborn.UserInterface;

public sealed class WindowManager: IDisposable
{
    private readonly Dictionary<string, Window> _windows      = new();
    private readonly WindowSystem               _windowSystem = new("Aethersense Redux Reborn");

    public void AddWindow(string name, Window window)
    {
        _windows.Add(name, window);
        _windowSystem.AddWindow(window);
    }

    public void OpenWindow(string name)
    {
        if (_windows.TryGetValue(name, out var window))
            window.IsOpen = true;
    }

    public void CloseWindow(string name)
    {
        if (_windows.TryGetValue(name, out var window))
            window.IsOpen = false;
    }

    public void ToggleWindow(string name)
    {
        if (_windows.TryGetValue(name, out var window))
            window.IsOpen = !window.IsOpen;
    }

    public void Draw() { _windowSystem.Draw(); }

    public void Dispose()
    {
        foreach (var (name, window) in _windows){
            if (window is IDisposable disposable)
                disposable.Dispose();
        }

        _windowSystem.RemoveAllWindows();
    }
}
