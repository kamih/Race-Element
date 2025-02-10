using RaceElement.Data.Common;
using RaceElement.Data.Common.Graph;
using RaceElement.Graph;
using RaceElement.Graph.Edge;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DataGraphTest;

[Overlay(
    Name = "Data Graph Leaderboard",
    Description = "",
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class DataGraphLeaderBoardOverlay : CommonAbstractOverlay
{
    private readonly InfoPanel _panel;

    public DataGraphLeaderBoardOverlay(Rectangle rectangle) : base(rectangle, "Data Graph Leaderboard")
    {
        _panel = new InfoPanel(12, 550);
        Width = 550;
        Height = 250;
        RefreshRateHz = 2;
    }

    public sealed override void BeforeStart()
    {
        if (IsPreviewing) return;
    }

    public sealed override void BeforeStop()
    {
        _panel?.Dispose();
    }

    public sealed override bool ShouldRender() => true;

    public sealed override void Render(Graphics g)
    {
        if (SimDataProvider.RacingGraph.IsEmpty) return;

        var graph = SimDataProvider.RacingGraph;

        // start of time tracking
        var allLapTimes = graph.Where(x => x is LapTimeDataNode);
        var allDrivers = graph.Where(x => x is RacingDriverNode);
        var allCars = graph.Where(x => x is RacingCarNode);

        if (allLapTimes.Any())
        {
            LapTimeDataNode? fastestLap = allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs) as LapTimeDataNode;

            _ = graph.TryGetEdgesTo(fastestLap, out var edges);
            if (edges.Count != 0)
            {
                var fastestDriverId = edges.First().ParentId;
                var fastestDriver = (RacingDriverNode)allDrivers.First(x => x.Id == fastestDriverId);

                graph.TryGetEdgesTo(fastestDriver, out var driverEdgesTo);

                RacingCarNode fastestCar = (RacingCarNode)allCars.First(x => driverEdgesTo.Select(x => x.ParentId).Contains(x.Id));
                _panel.AddLine("Fastest", $"#{fastestCar.CarNumber} - {fastestDriver.Name} - L{fastestLap.LapIndex} - {TimeSpan.FromMilliseconds(fastestLap.LapTimeMs):mm\\:ss\\:fff}");
            }
        }

        _panel.AddLine("Nodes", $"{graph.Count}");
        _panel.AddLine("Edges", $"{graph.Edges.Count}");
        if (allLapTimes.Any())
        {
            _panel.AddLine("Laps", $"{allLapTimes.Count()}");
            int[] avgLapTimeMs = allLapTimes.Select(x => ((LapTimeDataNode)x).LapTimeMs).ToArray();
            AddTimeStats(_panel, [.. avgLapTimeMs]);
        }

        _panel.Draw(g);
    }

    private static (double min, double max, double mean, double median, double std) CalculateMetrics(List<double> list)
    {
        var mean = list.Average();
        var std = Math.Sqrt(list.Aggregate(0.0, (a, x) => a + (x - mean) * (x - mean)) / list.Count);
        var sorted = list.OrderBy(x => x).ToList();
        var median = sorted.Count % 2 == 0 ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2 : sorted[sorted.Count / 2];
        return (sorted[0], sorted[^1], mean, median, std);
    }

    private static void AddStats(InfoPanel panel, List<double> data)
    {
        var (min, max, mean, median, std) = CalculateMetrics(data);
        panel.AddLine("Min", $"{min:F4}");
        panel.AddLine("Avg", $"{mean:F4}");
        panel.AddLine("Max", $"{max:F4}");
        panel.AddLine("Median", $"{median:F4}");
        panel.AddLine("StDev", $"{std:F4}");
    }

    private static void AddTimeStats(InfoPanel panel, List<double> data)
    {
        var (min, max, mean, median, std) = CalculateMetrics(data);
        panel.AddLine("Min", $"{TimeSpan.FromMilliseconds(min):mm\\:ss\\:fff}");
        panel.AddLine("Avg", $"{TimeSpan.FromMilliseconds(mean):mm\\:ss\\:ffff}");
        panel.AddLine("Max", $"{TimeSpan.FromMilliseconds(max):mm\\:ss\\:fff}");
        panel.AddLine("Median", $"{TimeSpan.FromMilliseconds(median):mm\\:ss\\:ffff}");
        panel.AddLine("StDev", $"{TimeSpan.FromMilliseconds(std):mm\\:ss\\:ffff}");
    }
}
