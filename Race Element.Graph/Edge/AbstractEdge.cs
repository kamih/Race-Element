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
    public required Guid ParentId { get; init; }

    /// <summary>
    /// the node this edge goes to.
    /// </summary>
    public Guid ChildId { get; init; }

    /// <summary>
    /// The time when this Edge was created.
    /// </summary>
    public DateTime TimeStampUtc { get; private init; } = DateTime.UtcNow;

}
