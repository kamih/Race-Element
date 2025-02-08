using System.Collections.Concurrent;
using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;

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
        edges = [];
        if (fromNode == null || toNode == null) return false;

        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode && x.ToNode == toNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        return false;
    }

    public bool TryGetEdgesFrom(AbstractNode fromNode, out List<AbstractEdge> edges)
    {
        edges = [];
        if (fromNode == null) return false;

        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        return false;
    }

    public bool TryGetEdgesTo(AbstractNode toNode, out List<AbstractEdge> edges)
    {
        edges = [];
        if (toNode == null) return false;

        var found = Edges.AsParallel().Where(x => x.ToNode == toNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        return false;
    }

}
