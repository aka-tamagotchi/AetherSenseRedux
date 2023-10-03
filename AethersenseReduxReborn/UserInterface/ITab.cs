using System;

namespace AethersenseReduxReborn.UserInterface;

public interface ITab: IDisposable
{
    public string Name { get; }

    public void Draw() { }
}
