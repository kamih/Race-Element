using RaceElement.Graph.Node;

namespace RaceElement.Graph.Edge;
/// <summary>
/// Provides an abstract implementation of an edge used in <see cref="DataGraph"/>
/// </summary>
public abstract record class AbstractEdge : AbstractNode
{
    /// <summary>
    /// The node this edge comes from.
    /// </summary>
    public required AbstractNode FromNode { get; init; }

    /// <summary>
    /// the node this edge goes to.
    /// </summary>
    public required AbstractNode ToNode { get; init; }

    /// <summary>
    /// The time when this Edge was created.
    /// </summary>
    public DateTime TimeStampUtc { get; private init; } = DateTime.UtcNow;

    public DataEdge AsDataEdge() => new()
    {
        Id = Id,
        FromNode = FromNode.Id,
        ToNode = ToNode.Id,
        TimeStampUtc = TimeStampUtc,
        EdgeType = this.EqualityContract.FullName,
    };
}

public record class DataEdge
{
    public Guid Id { get; set; }
    public Guid FromNode { get; set; }
    public Guid ToNode { get; set; }
    public DateTime TimeStampUtc { get; set; }
    public string EdgeType { get; set; }
}