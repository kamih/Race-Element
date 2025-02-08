using MessagePack;
using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;
using System.Collections.Concurrent;

namespace RaceElement.Graph;

public sealed class DataGraph : ConcurrentBag<AbstractNode>
{
    public ConcurrentBag<AbstractEdge> Edges { get; private set; } = [];

    /// <summary>
    /// Empties the data graph.
    /// </summary>
    public void ClearGraph()
    {
        Clear();
        Edges.Clear();
    }

    public bool TryAddEdge(AbstractEdge edge)
    {
        if (edge.FromNode == null || edge.ToNode == null)
            return false;

        Edges.Add(edge);
        return true;
    }

    public bool TryGetEdges(AbstractNode fromNode, AbstractNode toNode, out List<AbstractEdge> edges)
    {
        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode && x.ToNode == toNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        edges = [];
        return false;
    }

    public bool TryGetEdgesFrom(AbstractNode fromNode, out List<AbstractEdge> edges)
    {
        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        edges = [];
        return false;
    }

    public bool TryGetEdgesTo(AbstractNode toNode, out List<AbstractEdge> edges)
    {
        var found = Edges.AsParallel().Where(x => x.ToNode == toNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        edges = [];
        return false;
    }

    public byte[] GetBytes() => MessagePackSerializer.Serialize<GraphMessageObject>(new()
    {
        Nodes = [.. this],
        Edges = [.. Edges]
    });

    public void InsertGraphMessage(byte[] bytes)
    {
        var standardOptions = MessagePackSerializerOptions.Standard;
        var gmo = MessagePackSerializer.Deserialize<GraphMessageObject>(bytes, options: standardOptions);
        Parallel.For(0, gmo.Nodes.Length, i => Add(gmo.Nodes[i]));
        Parallel.For(0, gmo.Edges.Length, i => this.Edges.Add(gmo.Edges[i]));
    }
}

[MessagePackObject]
public record GraphMessageObject
{
    [Key(0)] public required AbstractNode[] Nodes { get; set; }

    [Key(1)] public required AbstractEdge[] Edges { get; set; }
}
