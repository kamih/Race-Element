using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;
using RaceElement.Graph.Node.ValueNodes;

namespace RaceElement.Graph;
internal class TestClass
{
    private readonly DataGraph _graph;

    public TestClass()
    {
        _graph = new();

        var person = new PersonNode() { FirstName = "First", LastName = "Last Name" };
        var driver = new RacingDriverNode() { DriverNumber = 1 };
        _graph.TryAddNode(person);
        _graph.TryAddNode(driver);
        _graph.TryAddEdge(new OwnsNode(person, driver));

        var person2 = new PersonNode() { FirstName = "First", LastName = "Last Name" };
        var driver2 = new RacingDriverNode() { DriverNumber = 1 };
        _graph.TryAddNode(person2);
        _graph.TryAddNode(driver2);
        _graph.TryAddEdge(new OwnsNode(person2, driver2));

        SectorTimeNode s1 = new(300), s2 = new(200), s3 = new(500);
        LapTimeNode fastestLapA = new(1000);
        _graph.TryAddNode(fastestLapA);
        _graph.TryAddEdge(new OwnsNode(person, fastestLapA));

        Thread.Sleep(200);
        LapTimeNode fastestLapB = new(980);
        _graph.TryAddNode(fastestLapB);
        _graph.TryAddEdge(new OwnsNode(person, fastestLapB));


        var allLapTimes = _graph.Where(x => x.Key is LapTimeNode)
                                .Select(x => (LapTimeNode)x.Key);

        var fastestLap = allLapTimes.Min(x => x.Value);
    }
}

public sealed record class LapTimeNode : AbstractIntegerNode
{
    public LapTimeNode(int lapTimeMs) : base(lapTimeMs) { }
}
public sealed record class SectorTimeNode : AbstractIntegerNode
{
    public SectorTimeNode(int sectorTimeMs) : base(sectorTimeMs) { }
}

public sealed record class RacingDriverNode : AbstractNode
{
    public required int DriverNumber { get; init; }
}

public sealed record class PersonNode : AbstractNode
{
    public string FirstName;
    public string LastName;
}

