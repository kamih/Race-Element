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
        var found = Edges.Where(x => x.FromNode == fromNode && x.ToNode == toNode);
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
        var found = Edges.Where(x => x.FromNode == fromNode);
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
        var found = Edges.Where(x => x.ToNode == toNode);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        edges = [];
        return false;
    }

}
