using RaceElement.Graph;
using RaceElement.Graph.Edge;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System.Drawing;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DataGraphTest;

[Overlay(
    Name = "Data Graph Test",
    Description = "",
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class DataGraphTestOverlay : CommonAbstractOverlay
{
    private readonly DataGraph _graph;
    private InfoPanel _panel;
    public DataGraphTestOverlay(Rectangle rectangle) : base(rectangle, "Data Graph Test")
    {
        _graph = new DataGraph();
        _panel = new InfoPanel(12, 500);
        Width = 500;
        RefreshRateHz = 1;
    }

    public sealed override void BeforeStart()
    {
        int carCount = 200;
        int racingDriverCount = 20;
        int lapCount = 200;

        _ = Parallel.For(0, carCount, i =>
         {
             var someCar = new RacingCarNode() { CarNumber = i + 1 };
             _graph.Add(someCar);
         });

        _ = Parallel.For(0, racingDriverCount, i =>
        {
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"random {i}", LastName = "Last Name" };
            _graph.Add(someDriver);

            int carNumber = Random.Shared.Next(1, carCount);
            RacingCarNode? raceCar = _graph.FirstOrDefault(x => x is RacingCarNode car && car.CarNumber == carNumber) as RacingCarNode;
            _graph.TryAddEdge(new OwnsEdge() { FromNode = raceCar, ToNode = someDriver });

            Parallel.For(1, lapCount, j =>
            {
                int s1 = Random.Shared.Next(10000, 40000);
                int s2 = Random.Shared.Next(10000, 40000);
                int s3 = Random.Shared.Next(10000, 40000);
                LapTimeDataNode lapData = new() { LapIndex = j, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
                _graph.Add(lapData);
                _graph.TryAddEdge(new OwnsEdge()
                {
                    FromNode = someDriver,
                    ToNode = lapData
                });
            });
        });

        var data = _graph.GetData();
        _graph.ClearGraph();
        _graph.InsertData(data);
    }

    public sealed override bool ShouldRender() => true;

    public sealed override void Render(Graphics g)
    {
        var now = TimeProvider.System.GetTimestamp();

        var allLapTimes = _graph.Where(x => x is LapTimeDataNode);
        var allDrivers = _graph.Where(x => x is RacingDriverNode);
        var allCars = _graph.Where(x => x is RacingCarNode);
        long allLapsCount = allLapTimes.Count();
        long allDriversCount = allDrivers.Count();
        long allCarsCount = allCars.Count();

        LapTimeDataNode? fastestLap = allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs) as LapTimeDataNode;
        _ = _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriver = (RacingDriverNode)edges.First().FromNode;

        var elapsedTime = TimeProvider.System.GetElapsedTime(now);

        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Edges.Count}");
        _panel.AddLine("Time", $"{elapsedTime}");
        _panel.AddLine("Laps", $"{allLapsCount}");
        _panel.AddLine("Drivers", $"{allDriversCount}");
        _panel.AddLine("Cars", $"{allCarsCount}");
        _panel.AddLine("Fastest driver", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3}");

        _panel.Draw(g);
    }
}
