namespace RaceElement.Graph;

public sealed class DataGraph : Dictionary<AbstractNode, List<AbstractEdge>>
{
    private readonly Dictionary<AbstractNode, List<AbstractEdge>> _edges = [];

    public void AddNewNode(AbstractNode node)
    {
        if (!ContainsKey(node))
            Add(node, []);
    }

    public bool TryAddEdge(AbstractNode fromNode, AbstractEdge edge)
    {
        if (ContainsKey(fromNode))
        {
            this[fromNode].Add(edge);
            return true;
        }

        return false;
    }

    public bool TryGetEdges(AbstractNode fromNode, out List<AbstractEdge> edges)
    {
        if (ContainsKey(fromNode))
        {
            edges = this[fromNode];
            return true;
        }

        edges = [];
        return false;
    }

    public bool TryGetEdges(AbstractNode fromNode, AbstractNode toNode, out List<AbstractEdge> edges)
    {
        if (ContainsKey(fromNode))
        {
            var found = this[fromNode].FindAll(x => x.ToNode == toNode);
            if (found.Count > 0)
            {
                edges = found;
                return true;
            }
        }

        edges = [];
        return false;
    }
}

public abstract class AbstractNode
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public virtual void Accept(NodeVisitor visitor) => visitor.Visit(this);
}

public abstract class NodeVisitor
{
    public abstract void Visit(AbstractNode node);
}

public abstract class AbstractEdge : AbstractNode
{
    public required AbstractNode FromNode { get; set; }
    public required AbstractNode ToNode { get; set; }

    /// <summary>
    /// The time when this Edge was created.
    /// </summary>
    public required DateTime TimeStampUtc { get; init; }
}