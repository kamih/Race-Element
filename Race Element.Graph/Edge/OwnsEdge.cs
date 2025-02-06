using RaceElement.Graph.Node;

namespace RaceElement.Graph.Edge;

/// <summary>
/// A simple relationship between two nodes.<br/>
/// The <see cref="AbstractEdge.FromNode"/> owns the <see cref="AbstractEdge.ToNode"/>
/// </summary>
public sealed record class OwnsNode : AbstractEdge
{
    public OwnsNode(AbstractNode parent, AbstractNode child)
    {
        FromNode = parent;
        ToNode = child;
    }
}