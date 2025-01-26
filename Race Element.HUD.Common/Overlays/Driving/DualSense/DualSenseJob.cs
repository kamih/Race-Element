using RaceElement.Core.Jobs.Loop;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.Resources;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal sealed class ThrottleJob(DualSenseOverlay overlay) : AbstractLoopJob
{
    public sealed override void RunAction()
    {
        if (!overlay.ShouldRender())
            return;

        TriggerHaptics.HandleAcceleration(overlay._config);
    }
    public override void AfterCancel() => ds5w_set_trigger_effect_off(0);
}

internal sealed class BrakeJob(DualSenseOverlay overlay) : AbstractLoopJob
{
    public sealed override void RunAction()
    {
        if (!overlay.ShouldRender())
            return;

        TriggerHaptics.HandleBraking(overlay._config);
    }
    public override void AfterCancel() => ds5w_set_trigger_effect_off(1);
}
