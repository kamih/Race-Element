using RaceElement.Data.ACC.EntryList;
using RaceElement.Data.ACC.Session;
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
    private sealed class OpponentsConfiguration : OverlayConfiguration
    {
        [ConfigGrouping("Opponents", "Change how many opponents are displayed either ahead or behind")]
        public OpponentsGrouping Opponents { get; init; } = new();
        public sealed class OpponentsGrouping
        {
            public int AheadCount { get; init; } = 3;
            public int BehindCount { get; init; } = 3;
        }


        [ConfigGrouping("Data", "Change which opponents data is displayed.")]
        public BehaviorGrouping Data { get; init; } = new();
        public sealed class BehaviorGrouping
        {
            public bool Sectors { get; init; } = true;
            public bool Gap { get; init; } = true;
            public bool Difference { get; init; } = true;
        }
    }
    private readonly record struct OpponentsModel
    {
        public CarDataModel[] Ahead { get; init; }
        public CarDataModel[] Behind { get; init; }
    }
    private readonly record struct CarDataModel
    {
        public int CarIndex { get; init; }
        public int Laptime { get; init; }
        public int[] Sectors { get; init; }
        public GapModel Gap { get; init; }
    }
    private readonly record struct GapModel
    {
        public float GapTime { get; init; }
        public int GapLaps { get; init; }
    }

    public OpponentsOverlay(Rectangle rectangle) : base(rectangle, "Opponents")
    {
    }

    public sealed override void Render(Graphics g)
    {
        OpponentsModel model = CreateOpponentsModel();

        if (model.Ahead?.Length == 0 && model.Behind?.Length == 0)
            return;

        foreach (var item in model.Ahead)
        {

        }

        // add local car


        foreach (var item in model.Behind)
        {

        }

    }

    private OpponentsModel CreateOpponentsModel()
    {
        var allCars = EntryListTracker.Instance.Cars;
        if (allCars.Count == 0) return new();

        int playerCarId = pageGraphics.PlayerCarID;
        var playerCar = allCars.FirstOrDefault(x => x.Key == playerCarId);
        int playerCarPosition = playerCar.Value.RealtimeCarUpdate.Position;

        List<CarDataModel> carsAhead = [];
        if (playerCarPosition > 1)
        {
            for (int i = playerCarPosition - 1; i >= 1; i--)
            {
                var ahead = allCars.FirstOrDefault(x => x.Value.RealtimeCarUpdate.Position == i);
                if (ahead.Value != null)
                {
                    int laptime = -1;
                    int[] sectors = [-1, -1, -1];

                    float gap = GapTracker.Instance.TimeGapBetween(playerCarId, playerCar.Value.RealtimeCarUpdate.SplinePosition, ahead.Key);
                    CarDataModel model = new()
                    {
                        CarIndex = ahead.Key,
                        Laptime = laptime,
                        Sectors = sectors,
                        Gap = new GapModel()
                        {
                            GapLaps = playerCar.Value.RealtimeCarUpdate.Laps - ahead.Value.RealtimeCarUpdate.Laps,
                            GapTime = gap
                        }
                    };
                    carsAhead.Add(model);
                }
            }
        }


        CarDataModel[] behind = [];

        return new OpponentsModel() { Ahead = [.. carsAhead], Behind = behind };
    }

    /// <summary>
    /// Gets the position of the car, based on <see cref="_config"/> If multiclass is set it will return the multiclass position
    /// </summary>
    /// <param name="carId"></param>
    /// <returns>0 if not car id not found.</returns>
    private int GetPosition(int carId)
    {
        var allCars = EntryListTracker.Instance.Cars;
        if (allCars.Count == 0) return 0;

        var car = allCars.FirstOrDefault(x => x.Key == carId);
        if (car.Value == null) return 0;

        return car.Value.RealtimeCarUpdate.Position;
    }

}
