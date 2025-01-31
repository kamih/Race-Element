using RaceElement.Data.ACC.EntryList;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceElement.HUD.ACC.Overlays.Driving.Opponents;
[Overlay(
Name = "Opponents",
Description = "Shows information about the cars ahead and behind in terms of race position.")]
internal sealed class OpponentsOverlay : AbstractOverlay
{
    private readonly OpponentsConfiguration _config = new();
    private class OpponentsConfiguration : OverlayConfiguration
    {

    }

    public OpponentsOverlay(Rectangle rectangle) : base(rectangle, "Opponents")
    {
    }

    public sealed override void Render(Graphics g)
    {
        var allCars = EntryListTracker.Instance.Cars;
        if (allCars.Count > 0)
        {
            var playerCar = allCars.FirstOrDefault(x => x.Key == pageGraphics.PlayerCarID);
            if (playerCar.Value == null) return;

            int playerPosition = playerCar.Value.RealtimeCarUpdate.Position;
        }
    }
}
