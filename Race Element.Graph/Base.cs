namespace RaceElement.Graph;

public sealed class DataGraph<T> : Dictionary<AbstractNode, List<Edge<T>>> where T : AbstractNode
{
    private readonly Dictionary<AbstractNode, List<Edge<T>>> _edges = [];

    public void AddNewNode(AbstractNode node)
    {
        if (!ContainsKey(node))
            Add(node, []);
    }

    public Edge<T> CreateEdge(T fromNode, T toNode)
    {
        var edge = new Edge<T>
        {
            FromNode = fromNode,
            ToNode = toNode,
            TimeStampUtc = DateTime.UtcNow,
        };

        if (ContainsKey(fromNode))
            this[fromNode].Add(edge);

        return edge;
    }

    public bool GetEdges(T fromNode, out List<Edge<T>> edges)
    {
        if (ContainsKey(fromNode))
        {
            edges = this[fromNode];
            return true;
        }

        edges = [];
        return false;
    }

    public bool TryGetEdges(T fromNode, T toNode, out List<Edge<T>> edges)
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

public class Edge<T> where T : AbstractNode
{
    public required T FromNode { get; set; }
    public required T ToNode { get; set; }

    /// <summary>
    /// The time when this Edge was created.
    /// </summary>
    public required DateTime TimeStampUtc { get; init; }
}