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
    Name = "Data Graph Test",
    Description = "",
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class DataGraphTestOverlay : CommonAbstractOverlay
{
    private readonly DataGraph _graph;
    private readonly InfoPanel _panel;

    public DataGraphTestOverlay(Rectangle rectangle) : base(rectangle, "Data Graph Test")
    {
        _graph = new DataGraph();
        _panel = new InfoPanel(12, 550);
        Width = 550;
        Height = 400;
        RefreshRateHz = 1;
    }

    public sealed override void BeforeStart()
    {
        if (IsPreviewing) return;

        int carCount = 100;
        int racingDriverCount = 400;
        int lapCount = 250;

        Debug.WriteLine($"Inserting {carCount} Racing Cars");
        var trackStates = Enum.GetValues<TrackStates>();
        _ = Parallel.For(0, carCount, i =>
         {
             var someCar = new RacingCarNode() { CarNumber = i + 1 };
             _graph.Add(someCar);
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
         });

        Debug.WriteLine($"Inserting {racingDriverCount} Racing Drivers with each having {lapCount} Laps");
        _ = Parallel.For(0, racingDriverCount, i =>
        {
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"Driver {i}", LastName = "Last Name" };
            _graph.Add(someDriver);

            int carNumber = Random.Shared.Next(1, carCount);
            RacingCarNode? raceCar = _graph.FirstOrDefault(x => x is RacingCarNode car && car.CarNumber == carNumber) as RacingCarNode;
            _graph.TryAddEdge(new OwnsEdge() { ParentId = raceCar.Id, ChildId = someDriver.Id });

            Parallel.For(1, lapCount, j =>
            {
                int s1 = Random.Shared.Next(10000, 40000);
                int s2 = Random.Shared.Next(10000, 40000);
                int s3 = Random.Shared.Next(10000, 40000);
                LapTimeDataNode lapData = new() { LapIndex = j, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
                _graph.Add(lapData);
                _graph.TryAddEdge(new OwnsEdge()
                {
                    ParentId = someDriver.Id,
                    ChildId = lapData.Id
                });
            });
        });

        var data = _graph.GetData();

        Debug.WriteLine($"{data.Nodes.Length + data.Edges.Length} Bytes");

        var lines = JsonSerializer.Serialize(data);

        string dataFilePath = AppContext.BaseDirectory + "data.txt";
        File.WriteAllText(dataFilePath, lines.ToCharArray());
        string contents = File.ReadAllText(dataFilePath);

        DataGraphBytes recoveredData = JsonSerializer.Deserialize<DataGraphBytes>(contents);
        _graph.ClearGraph();
        _graph.InsertData(recoveredData);
    }

    public sealed override void BeforeStop()
    {
        _panel?.Dispose();
        _graph.ClearGraph();
    }

    public sealed override bool ShouldRender() => true;

    public sealed override void Render(Graphics g)
    {
        if (_graph.IsEmpty) return;

        var now = TimeProvider.System.GetTimestamp();
        var allLapTimes = _graph.Where(x => x is LapTimeDataNode);
        var allDrivers = _graph.Where(x => x is RacingDriverNode);
        var allCars = _graph.Where(x => x is RacingCarNode);
        var allTrackStates = _graph.Edges.Where(x => x is TrackStateEdge);

        LapTimeDataNode? fastestLap = allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs) as LapTimeDataNode;
        _ = _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriverId = edges.First().ParentId;
        var fastestDriver = (RacingDriverNode)allDrivers.First(x => x.Id == fastestDriverId);

        _graph.TryGetEdgesTo(fastestDriver, out var driverEdgesTo);
        RacingCarNode fastestCar = (RacingCarNode)allCars.First(x => driverEdgesTo.Select(x => x.ParentId).Contains(x.Id));
        var allCarStates = _graph.Edges.Where(x => x.ParentId == fastestCar.Id && x is TrackStateEdge);
        var latestTrackState = allCarStates.Where(x => x.ParentId == fastestCar.Id).MaxBy(x => x.TimeStampUtc) as TrackStateEdge;

        var elapsedTime = TimeProvider.System.GetElapsedTime(now);
        _stats.Add(elapsedTime.TotalMilliseconds);

        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Edges.Count}");
        _panel.AddLine("Fastest", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3} - {latestTrackState.State}");
        _panel.AddLine("Time", $"{elapsedTime}");
        AddStats(_panel, [.. _stats]);
        _panel.Draw(g);
    }

    private ConcurrentBag<double> _stats = [];
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
}
