namespace AethersenseReduxReborn.IntensitySource.Interfaces;

public interface ITimeBasedIntensitySource : IIntensitySource
{
    double Update(long elapsedMilliseconds);
}
