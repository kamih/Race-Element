using RaceElement.Graph;
using RaceElement.Graph.Edge;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System.Diagnostics;
using System.Drawing;
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
        if (IsPreviewing) return;

        int carCount = 100;
        int racingDriverCount = 400;
        int lapCount = 250;

        Debug.WriteLine($"Inserting {carCount} RacingCars");
        var trackStates = Enum.GetValues<TrackState>();
        _ = Parallel.For(0, carCount, i =>
         {
             var someCar = new RacingCarNode() { CarNumber = i + 1 };
             _graph.Add(someCar);
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, ChildId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, ChildId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { ParentId = someCar.Id, ChildId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
         });

        Debug.WriteLine($"Inserting {racingDriverCount} RacingDrivers with each having {lapCount} LapTimeDatas");
        _ = Parallel.For(0, racingDriverCount, i =>
        {
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"random {i}", LastName = "Last Name" };
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

    public sealed override bool ShouldRender() => true;

    public sealed override void Render(Graphics g)
    {
        if (_graph.IsEmpty) return;

        var now = TimeProvider.System.GetTimestamp();

        var allLapTimes = _graph.AsParallel().Where(x => x is LapTimeDataNode);
        var allDrivers = _graph.AsParallel().Where(x => x is RacingDriverNode);
        var allCars = _graph.AsParallel().Where(x => x is RacingCarNode);
        var allTrackStates = _graph.Edges.AsParallel().Where(x => x is TrackStateEdge).OrderByDescending(x => x.TimeStampUtc);

        LapTimeDataNode? fastestLap = allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs) as LapTimeDataNode;
        _ = _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriverId = edges.First().ParentId;
        var fastestDriver = (RacingDriverNode)_graph.First(x => x.Id == fastestDriverId);

        _graph.TryGetEdgesTo(fastestDriver, out var driverParents);
        RacingCarNode fastestCar = (RacingCarNode)allCars.First(x => driverParents.Select(x => x.ParentId).Contains(x.Id));
        var latestTrackState = allTrackStates.First(x => x.ParentId == fastestCar.Id) as TrackStateEdge;

        var elapsedTime = TimeProvider.System.GetElapsedTime(now);

        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Edges.Count}");
        _panel.AddLine("Fastest", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3} - {latestTrackState.State}");
        _panel.AddLine("Time", $"{elapsedTime}");

        _panel.Draw(g);
    }
}
