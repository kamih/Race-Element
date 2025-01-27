using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Internal;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static RaceElement.HUD.Common.Overlays.Pitwall.DualSenseInternal.Resources;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DualSenseInternal;

[Overlay(Name = "DSI",
    Description = "Adds active triggers for the DualSense Controller.",
    OverlayCategory = OverlayCategory.Inputs,
    OverlayType = OverlayType.Drive,
    Game = Game.RaceRoom | Game.AssettoCorsa1 | Game.AssettoCorsaEvo,
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class DsiOverlay : CommonAbstractOverlay
{
    internal readonly DsiConfiguration _config = new();
    private DsxJob _dsxJob;

    public DsiOverlay(Rectangle rectangle) : base(rectangle, "DSI")
    {
        Width = 1; Height = 1;
        RefreshRateHz = 1;
        AllowReposition = false;
    }

    public sealed override void BeforeStart()
    {
        if (IsPreviewing) return;

        _dsxJob = new DsxJob(this) { IntervalMillis = 1000 / 200 };
        _dsxJob.Run();
    }
    public sealed override void BeforeStop()
    {
        if (IsPreviewing) return;

        _dsxJob?.CancelJoin();
    }

    public sealed override bool ShouldRender() => true;// DefaultShouldRender() && !IsPreviewing;

    public sealed override void Render(Graphics g) { }
}
