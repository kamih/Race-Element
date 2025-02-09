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
        int carCount = 100;
        int racingDriverCount = 400;
        int lapCount = 250;

        Debug.WriteLine($"Inserting {carCount} RacingCars");
        var trackStates = Enum.GetValues<TrackStates>();
        _ = Parallel.For(0, carCount, i =>
         {
             var someCar = new RacingCarNode() { CarNumber = i + 1 };
             _graph.Add(someCar);
             _graph.TryAddEdge(new TrackStateEdge() { FromNodeId = someCar.Id, ToNodeId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { FromNodeId = someCar.Id, ToNodeId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
             _graph.TryAddEdge(new TrackStateEdge() { FromNodeId = someCar.Id, ToNodeId = someCar.Id, State = trackStates[Random.Shared.Next(1, trackStates.Length - 1)] });
         });

        Debug.WriteLine($"Inserting {racingDriverCount} RacingDrivers with each having {lapCount} LapTimeDatas");
        _ = Parallel.For(0, racingDriverCount, i =>
        {
            var someDriver = new RacingDriverNode() { DriverId = i * 2, FirstName = $"random {i}", LastName = "Last Name" };
            _graph.Add(someDriver);

            int carNumber = Random.Shared.Next(1, carCount);
            RacingCarNode? raceCar = _graph.FirstOrDefault(x => x is RacingCarNode car && car.CarNumber == carNumber) as RacingCarNode;
            _graph.TryAddEdge(new OwnsEdge() { FromNodeId = raceCar.Id, ToNodeId = someDriver.Id });

            Parallel.For(1, lapCount, j =>
            {
                int s1 = Random.Shared.Next(10000, 40000);
                int s2 = Random.Shared.Next(10000, 40000);
                int s3 = Random.Shared.Next(10000, 40000);
                LapTimeDataNode lapData = new() { LapIndex = j, LapTimeMs = s1 + s2 + s3, SectorTimesMs = [s1, s2, s3] };
                _graph.Add(lapData);
                _graph.TryAddEdge(new OwnsEdge()
                {
                    FromNodeId = someDriver.Id,
                    ToNodeId = lapData.Id
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
        var now = TimeProvider.System.GetTimestamp();

        var allLapTimes = _graph.Where(x => x is LapTimeDataNode);
        var allDrivers = _graph.Where(x => x is RacingDriverNode);
        var allCars = _graph.Where(x => x is RacingCarNode);
        var allTrackStates = _graph.Edges.Where(x => x is TrackStateEdge);

        long allLapsCount = allLapTimes.Count();
        long allDriversCount = allDrivers.Count();
        long allCarsCount = allCars.Count();

        LapTimeDataNode? fastestLap = allLapTimes.MinBy(x => ((LapTimeDataNode)x).LapTimeMs) as LapTimeDataNode;
        _ = _graph.TryGetEdgesTo(fastestLap, out var edges);
        var fastestDriverId = edges.First().FromNodeId;
        var fastestDriver = (RacingDriverNode)_graph.FirstOrDefault(x => x.Id == fastestDriverId);

        _graph.TryGetEdgesTo(fastestDriver, out var driverEdgesTo);
        var fastestCar = (RacingCarNode)allCars.Where(x => driverEdgesTo.Select(x => x.FromNodeId).Contains(x.Id)).FirstOrDefault();
        var allCarStates = _graph.Edges.Where(x => x.FromNodeId == fastestCar.Id && x is TrackStateEdge);
        var latestTrackState = (TrackStateEdge)allCarStates.MaxBy(x => x.TimeStampUtc);

        var elapsedTime = TimeProvider.System.GetElapsedTime(now);

        _panel.AddLine("Nodes", $"{_graph.Count}");
        _panel.AddLine("Edges", $"{_graph.Edges.Count}");
        _panel.AddLine("Time", $"{elapsedTime}");
        _panel.AddLine("Laps", $"{allLapsCount}");
        _panel.AddLine("Drivers", $"{allDriversCount}");
        _panel.AddLine("Cars", $"{allCarsCount}");
        _panel.AddLine("Fastest driver", $"{fastestDriver.FirstName} - L{fastestLap.LapIndex} - {fastestLap.LapTimeMs / 1000f:F3} - {latestTrackState.State}");

        _panel.Draw(g);
    }
}
