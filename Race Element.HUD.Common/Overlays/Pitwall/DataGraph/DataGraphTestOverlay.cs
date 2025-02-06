using RaceElement.Graph;
using RaceElement.Graph.Edge;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        _panel = new InfoPanel(12, 300);
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

        LapTimeDataNode lapDataA = new() { LapIndex = 0, LapTimeMs = 1000, SectorTimesMs = [300, 500, 200] };
        _graph.TryAddNode(lapDataA);
        _graph.TryAddEdge(new OwnsEdge(driver, lapDataA));

        LapTimeDataNode lapDataB = new() { LapIndex = 1, LapTimeMs = 900, SectorTimesMs = [300, 400, 200] };
        _graph.TryAddNode(lapDataB);
        _graph.TryAddEdge(new OwnsEdge(driver2, lapDataB));
    }

    public override bool ShouldRender() => true;

    public override void Render(Graphics g)
    {
        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Select(x => x.Value.Count).Sum()}");
        _panel.Draw(g);
    }
}
