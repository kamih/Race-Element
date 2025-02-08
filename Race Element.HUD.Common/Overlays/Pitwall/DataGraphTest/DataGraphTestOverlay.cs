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
        Parallel.For(0, 1000, i =>
        {
            var someCar = new RacingCarNode() { CarNumber = i };
            _graph.Add(someCar);
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"random {i}", LastName = "Last Name" };
            _graph.Add(someDriver);
            _graph.TryAddEdge(new OwnsEdge(someCar, someDriver));

            Parallel.For(0, 1000, j =>
            {
                int s1 = Random.Shared.Next(10000, 40000);
                int s2 = Random.Shared.Next(10000, 40000);
                int s3 = Random.Shared.Next(10000, 40000);
                LapTimeDataNode lapData = new() { LapIndex = j + 1, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
                _graph.Add(lapData);
                _graph.TryAddEdge(new OwnsEdge(someDriver, lapData));
            });
        });
    }

    public sealed override bool ShouldRender() => true;

    public sealed override void Render(Graphics g)
    {
        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Edges.Count}");

        var now = TimeProvider.System.GetTimestamp();

        var allLapTimes = _graph.Where(x => x is LapTimeDataNode);
        var allDrivers = _graph.Where(x => x is RacingDriverNode);
        LapTimeDataNode fastestLap = (LapTimeDataNode)allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs);
        _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriver = (RacingDriverNode)edges.First().FromNode;

        var elapsedTime = TimeProvider.System.GetElapsedTime(now);

        _panel.AddLine("Time", $"{elapsedTime}");

        _panel.AddLine("Amount of laps", $"{allLapTimes.Count()}");
        _panel.AddLine("Amount of Driver", $"{allLapTimes.Count()}");
        _panel.AddLine("Fastest driver", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3}");

        _panel.Draw(g);
    }
}
