using RaceElement.Core.Jobs.Loop;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.DS5W;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal sealed class DualSenseJob(DualSenseOverlay overlay) : AbstractLoopJob
{
    public sealed override void RunAction()
    {
        //if (!overlay.ShouldRender())
        //    return;

        TriggerHaptics.HandleAcceleration(overlay._config.ThrottleSlip);
        TriggerHaptics.HandleBraking(overlay._config.BrakeSlip);
    }
    public override void AfterCancel()
    {
        ds5w_set_trigger_effect_off(0);
        ds5w_set_trigger_effect_off(1);
    }
}
