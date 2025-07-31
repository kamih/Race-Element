// https://en.wikipedia.org/wiki/Exponential_smoothing
namespace RaceElement.HUD.ACC.Overlays.OverlayDebugInfo.OverlayEntryList;

internal class EmaFilter(int period, double initialValue)
{
    private double _value = initialValue;
    private readonly double _alpha = 2.0 / (period + 1);

    public double Update(double value)
    {
        _value = _alpha * value + (1.0 - _alpha) * _value;
        return _value;
    }
}
