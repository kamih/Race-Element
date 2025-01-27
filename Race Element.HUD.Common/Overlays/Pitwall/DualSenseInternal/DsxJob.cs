using DualSenseAPI;
using RaceElement.Core.Jobs.Loop;
using static RaceElement.HUD.Common.Overlays.Pitwall.DualSenseInternal.Resources;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DualSenseInternal;

internal sealed class DsxJob(DsiOverlay overlay) : AbstractLoopJob
{
    private DualSense? _controller;

    public override void BeforeRun()
    {
        DualSense[] available = DualSense.EnumerateControllers().ToArray();
        if (available != null && available.Length > 0)
        {
            _controller = available.First();
            _controller.Acquire();
        }
    }
    public sealed override void RunAction()
    {
        if (_controller == null || !overlay.ShouldRender()) return;


        _controller.OutputState.RightRumble = 0.5f;
        _controller.OutputState.L2Effect = TriggerEffect.Default;
        _controller.ReadWriteOnce();



        DsxPacket tcPacket = TriggerHaptics.HandleAcceleration(overlay._config);
        if (tcPacket != null)
        {
            //ServerResponse response = Receive();
            //HandleResponse(response);
        }

        DsxPacket absPacket = TriggerHaptics.HandleBraking(overlay._config);
        if (absPacket != null)
        {
            //ServerResponse response = Receive();
            //HandleResponse(response);
        }
    }
    public override void AfterCancel()
    {
        _controller?.Release();
    }

    private void SetDefaultFeedback()
    {
        if (_controller != null)
        {


        }

    }
}
