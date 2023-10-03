using System;

namespace AethersenseReduxReborn.SignalSources;

public abstract class SignalBase: ISignalSource
{
    private double _value;

    public required string Name { get; set; }

    public double Value {
        get => _value;
        protected set => _value = double.Clamp(value, 0, 1);
    }

    public abstract void Update(double elapsedMilliseconds);

    public abstract void DrawConfig();

    protected virtual void Dispose(bool disposing)
    {
        if (disposing){
        }
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
