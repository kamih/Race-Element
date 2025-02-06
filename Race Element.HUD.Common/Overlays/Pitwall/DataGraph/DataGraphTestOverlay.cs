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
        RefreshRateHz = 10;
    }

    public override void BeforeStart()
    {
        var car = new RacingCarNode() { CarNumber = 1 };
        _graph.TryAddNode(car);

        var driver = new RacingDriverNode() { DriverId = 1, FirstName = "First 1", LastName = "Last Name 1" };
        _graph.TryAddNode(driver);
        _graph.TryAddEdge(new OwnsEdge(car, driver));

        var driver2 = new RacingDriverNode() { DriverId = 2, FirstName = "First 2", LastName = "Last Name 2" };
        _graph.TryAddNode(driver2);
        _graph.TryAddEdge(new OwnsEdge(car, driver2));

        LapTimeDataNode lapDataA = new() { LapIndex = 0, LapTimeMs = 1500, SectorTimesMs = [600, 500, 400] };
        _graph.TryAddNode(lapDataA);
        _graph.TryAddEdge(new OwnsEdge(driver, lapDataA));

        LapTimeDataNode lapDataB = new() { LapIndex = 1, LapTimeMs = 1300, SectorTimesMs = [500, 400, 400] };
        _graph.TryAddNode(lapDataB);
        _graph.TryAddEdge(new OwnsEdge(driver2, lapDataB));
        for (int i = 3; i < 100_000; i++)
        {
            int s1 = Random.Shared.Next(300, 400);
            int s2 = Random.Shared.Next(300, 400);
            int s3 = Random.Shared.Next(300, 400);
            LapTimeDataNode lapData = new() { LapIndex = i + 1, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
            _graph.TryAddNode(lapData);
            _graph.TryAddEdge(new OwnsEdge(driver2, lapData));
        }
    }

    public override bool ShouldRender() => true;

    public override void Render(Graphics g)
    {
        var now = TimeProvider.System.GetTimestamp();
        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Select(x => x.Value.Count).Sum()}");

        var allLapTimes = _graph.Where(x => x.Key is LapTimeDataNode)
                                       .Select(x => (LapTimeDataNode)x.Key);
        var fastestLap = allLapTimes.MinBy(x => x.LapTimeMs);
        _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriver = (RacingDriverNode)edges.First().FromNode;
        _panel.AddLine("Fastest driver", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3}");

        _panel.AddLine("Time", $"{TimeProvider.System.GetElapsedTime(now)}");

        _panel.Draw(g);
    }
}
