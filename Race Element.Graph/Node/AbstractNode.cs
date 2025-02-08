using MessagePack;
using RaceElement.Graph.Edge;

namespace RaceElement.Graph.Node;
/// <summary>
/// Provides an abstract implementation of a node used in <see cref="DataGraph"/>
[MessagePackObject]
[Union(0, typeof(AbstractEdge))]
public abstract record class AbstractNode
{
    [Key(0)] public Guid Id { get; private init; } = Guid.NewGuid();

    public virtual void Accept(NodeVisitor visitor) => visitor.Visit(this);
}


