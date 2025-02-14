using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Drawing;
using static RaceElement.ACCSharedMemory;

namespace RaceElement.HUD.ACC.Overlays.Pitwall.OverlayPenaltyTypes;

[Overlay(
    Name = "Penalty Types",
    Description = "Logs penalty types (Use for debugging)",
    OverlayType = OverlayType.Pitwall
)]
internal class PentlyTypesOverlay(Rectangle rectangle) : AbstractOverlay(rectangle, "Penalty Types")
{

    private InfoPanel _panel;
    public override void BeforeStart()
    {
        Width = 500;
        Height = 500;
        _panel = new InfoPanel(11, 400);
    }

    public override void BeforeStop()
    {
        _panel?.Dispose();
    }

    public override bool ShouldRender() => true;

    public override void Render(Graphics g)
    {
        PenaltyShortcut penalty = pageGraphics.PenaltyType;
        int type = (int)penalty;
        _panel.AddLine("pen", $"{penalty}");


        _panel.Draw(g);
    }
}
