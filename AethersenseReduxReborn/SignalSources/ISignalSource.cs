using System;

namespace AethersenseReduxReborn.SignalSources;

public interface ISignalSource: IDisposable
{
    public string Name  { get; set; }
    public double Value { get; }

    public void Update(double elapsedMilliseconds);

    public void DrawConfig();
}
