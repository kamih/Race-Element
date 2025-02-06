using RaceElement.Graph.Edge;
using RaceElement.Graph.Node;
using System.Collections.Concurrent;

namespace RaceElement.Graph;

public sealed class DataGraph : ConcurrentDictionary<AbstractNode, List<AbstractEdge>>
{
    public bool TryAddNode(AbstractNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (ContainsKey(node))
            return false;

        return TryAdd(node, []);
    }

    public bool TryAddEdge(AbstractEdge edge)
    {
        ArgumentNullException.ThrowIfNull(edge);

        if (ContainsKey(edge.FromNode))
        {
            this[edge.FromNode].Add(edge);
            return true;
        }

        return false;
    }

    public bool TryAddEdges(AbstractEdge[] edges)
    {
        ArgumentNullException.ThrowIfNull(edges);
        foreach (AbstractEdge edge in edges)
            if (!ContainsKey(edge.FromNode))
                return false;

        foreach (AbstractEdge edge in edges)
            this[edge.FromNode].Add(edge);

        return true;
    }

    public bool TryGetEdges(AbstractNode fromNode, AbstractNode toNode, out List<AbstractEdge> edges)
    {
        if (ContainsKey(fromNode) && ContainsKey(toNode))
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

    public bool TryGetEdgesFrom(AbstractNode fromNode, out List<AbstractEdge> edges)
    {
        if (ContainsKey(fromNode))
        {
            edges = this[fromNode];
            return true;
        }

        edges = [];
        return false;
    }

    public bool TryGetEdgesTo(AbstractNode toNode, out List<AbstractEdge> edges)
    {
        if (ContainsKey(toNode))
        {
            List<AbstractEdge> found = [];

            foreach (var node in this)
                found.AddRange(node.Value.FindAll(x => x.ToNode == toNode));

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

