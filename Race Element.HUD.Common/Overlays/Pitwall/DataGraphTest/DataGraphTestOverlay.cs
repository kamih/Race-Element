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
internal class DataGraphTestOverlay : CommonAbstractOverlay
{
    private DataGraph _graph;
    private InfoPanel _panel;
    public DataGraphTestOverlay(Rectangle rectangle) : base(rectangle, "Data Graph Test")
    {
        _graph = new DataGraph();
        _panel = new InfoPanel(12, 500);
        Width = 500;
        RefreshRateHz = 1;
    }

    public override void BeforeStart()
    {
        for (int i = 1; i <= 200; i++)
        {
            var someCar = new RacingCarNode() { CarNumber = i };
            _graph.TryAddNode(someCar);
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"random {i}", LastName = "Last Name" };
            _graph.TryAddNode(someDriver);
            _graph.TryAddEdge(new OwnsEdge(someCar, someDriver));

            for (int j = 0; j < 1000; j++)
            {
                int s1 = Random.Shared.Next(30000, 40000);
                int s2 = Random.Shared.Next(30000, 40000);
                int s3 = Random.Shared.Next(30000, 40000);
                LapTimeDataNode lapData = new() { LapIndex = j + 1, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
                _graph.TryAddNode(lapData);
                _graph.TryAddEdge(new OwnsEdge(someDriver, lapData));
            }
        }
    }

    public override bool ShouldRender() => true;

    public override void Render(Graphics g)
    {
        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Select(x => x.Value.Count).Sum()}");

        var now = TimeProvider.System.GetTimestamp();

        var allLapTimes = _graph.Where(x => x.Key is LapTimeDataNode).Select(x => (LapTimeDataNode)x.Key);
        var fastestLap = allLapTimes.MinBy(x => x.LapTimeMs);
        _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriver = (RacingDriverNode)edges.First().FromNode;

        _panel.AddLine("Time", $"{TimeProvider.System.GetElapsedTime(now)}");

        _panel.AddLine("Amount of laps", $"{allLapTimes.Count()}");

        _panel.AddLine("Fastest driver", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3}");

        _panel.Draw(g);
    }
}
