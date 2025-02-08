using MessagePack;
using RaceElement.Graph.Node;

namespace RaceElement.Graph.Edge;
/// <summary>
/// Provides an abstract implementation of an edge used in <see cref="DataGraph"/>
/// </summary>
[MessagePackObject]
[Union(0, typeof(OwnsEdge))]
public abstract record class AbstractEdge : AbstractNode
{
    /// <summary>
    /// The node this edge comes from.
    /// </summary>
    [Key(0)] public AbstractNode? FromNode { get; init; } = default;

    /// <summary>
    /// the node this edge goes to.
    /// </summary>

    [Key(1)] public AbstractNode? ToNode { get; init; } = default;

    /// <summary>
    /// The time when this Edge was created.
    /// </summary>
    [Key(2)] public DateTime TimeStampUtc { get; private init; } = DateTime.UtcNow;
}