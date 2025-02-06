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
        var personNode = _graph.Where(x => x.Key is PersonNode node && node.FirstName == driverFirstName).Select(x => (PersonNode)x.Key).FirstOrDefault();
        if (personNode != null)
        {
            _graph.TryGetEdgesFrom(personNode, out var personEdges);

            RacingDriverNode? raceDriver = (RacingDriverNode)personEdges.FirstOrDefault(x => x.ToNode is RacingDriverNode).ToNode;

            _graph.TryGetEdgesTo(raceDriver, out var ownerEdges);
            PersonNode personBehindDriver = (PersonNode)ownerEdges.First(x => x.FromNode is PersonNode).FromNode;

            Debug.WriteLine($"{personBehindDriver?.FirstName}");

            var lapTimes = personEdges.Where(x => x.ToNode is LapTimeDataNode).Select(x => (LapTimeDataNode)x.ToNode);
            foreach (var item in lapTimes)
                Debug.WriteLine(item?.LapTimeMs);
        }
    }

    private void AddTestData()
    {
        var car = new RacingCarNode() { CarNumber = 1 };
        _graph.TryAddNode(car);

        var person = new PersonNode() { FirstName = "First 1", LastName = "Last Name 1" };
        var driver = new RacingDriverNode() { DriverNumber = 1 };
        _graph.TryAddNode(person);
        _graph.TryAddNode(driver);
        _graph.TryAddEdge(new OwnsNode(person, driver));
        _graph.TryAddEdge(new OwnsNode(car, driver));

        var person2 = new PersonNode() { FirstName = "First 2", LastName = "Last Name 2" };
        var driver2 = new RacingDriverNode() { DriverNumber = 1 };
        _graph.TryAddNode(person2);
        _graph.TryAddNode(driver2);
        _graph.TryAddEdge(new OwnsNode(person2, driver2));
        _graph.TryAddEdge(new OwnsNode(car, driver2));

        LapTimeDataNode fastestLapA = new() { LapTimeMs = 1000, SectorTimesMs = [300, 500, 200] };
        _graph.TryAddNode(fastestLapA);
        _graph.TryAddEdge(new OwnsNode(driver, fastestLapA));

        Thread.Sleep(200);
        LapTimeDataNode fastestLapB = new() { LapTimeMs = 900, SectorTimesMs = [300, 400, 200] };
        _graph.TryAddNode(fastestLapB);
        _graph.TryAddEdge(new OwnsNode(driver2, fastestLapB));
    }
}

public sealed record class PositionNode : AbstractIntegerNode { public PositionNode(int value) : base(value) { } }
public sealed record class LapTimeDataNode : AbstractNode
{
    /// <summary>
    /// The duration of the lap in milliseconds.
    /// -1 if invalid.
    /// </summary>
    public int LapTimeMs { get; init; } = -1;

    /// <summary>
    /// Sector Split times in milliseconds, empty if none exist.
    /// </summary>
    public int[] SectorTimesMs { get; init; } = [];
}
public sealed record class SectorTimeNode : AbstractIntegerNode { public SectorTimeNode(int sectorTimeMs) : base(sectorTimeMs) { } }

public sealed record class RacingDriverNode : AbstractNode
{
    public required int DriverNumber { get; init; }
}

public sealed record class RacingCarNode : AbstractNode
{
    public required int CarNumber { get; init; }
}

public sealed record class PersonNode : AbstractNode
{
    public string FirstName;
    public string LastName;
    public string Country;
}

