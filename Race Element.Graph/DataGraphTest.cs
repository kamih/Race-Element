using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;
using RaceElement.Graph.Node.ValueNodes;
using System.Diagnostics;

namespace RaceElement.Graph;
internal class DataGraphTest
{
    private readonly DataGraph _graph;

    public DataGraphTest()
    {
        _graph = new();

        AddTestData();

        var allLapTimes = _graph.Where(x => x.Key is LapTimeDataNode)
                                .Select(x => (LapTimeDataNode)x.Key);
        var fastestLap = allLapTimes.MinBy(x => x.LapTimeMs);

        // get all the fastest laptimes for person2
        string driverFirstName = "First 2";
        var driverNode = _graph.Where(x => x.Key is RacingDriverNode node && node.FirstName == driverFirstName).Select(x => (RacingDriverNode)x.Key).FirstOrDefault();
        if (driverNode != null)
        {
            _graph.TryGetEdgesFrom(driverNode, out var driverEdges);

            RacingDriverNode? raceDriver = (RacingDriverNode)driverEdges.FirstOrDefault(x => x.ToNode is RacingDriverNode).ToNode;

            Debug.WriteLine($"{driverNode.FirstName}");

            var lapTimes = driverEdges.Where(x => x.ToNode is LapTimeDataNode).Select(x => (LapTimeDataNode)x.ToNode);
            foreach (var item in lapTimes)
                Debug.WriteLine(item?.LapTimeMs);
        }
    }

    private void AddTestData()
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

        Thread.Sleep(200);
        LapTimeDataNode lapDataB = new() { LapIndex = 1, LapTimeMs = 900, SectorTimesMs = [300, 400, 200] };
        _graph.TryAddNode(lapDataB);
        _graph.TryAddEdge(new OwnsEdge(driver2, lapDataB));
    }
}

public sealed record class PositionNode : AbstractIntegerNode { public PositionNode(int value) : base(value) { } }
public sealed record class LapTimeDataNode : AbstractNode
{
    /// <summary>
    /// Lap index (starting from 0). -1 means invalid and not set.
    /// </summary>
    public required int LapIndex { get; init; } = -1;

    /// <summary>
    /// The duration of the lap in milliseconds.
    /// -1 if invalid.
    /// </summary>
    public int LapTimeMs { get; init; } = -1;

    /// <summary>
    /// Sector Split times in milliseconds. Empty array if none exist.
    /// </summary>
    public int[] SectorTimesMs { get; init; } = [];
}

public sealed record class RacingDriverNode : AbstractNode
{
    /// <summary>
    /// Can be driver index or specific number if assigned.
    /// Should be used to identify the driver within the game.
    /// -1 is invalid.
    /// </summary>
    public required int DriverId { get; init; } = -1;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

public sealed record class RacingCarNode : AbstractNode
{
    public required int CarNumber { get; init; }

    /// <summary>
    /// Race Position
    /// </summary>
    public int Position { get; set; } = -1;
}

