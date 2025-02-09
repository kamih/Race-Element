using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public DataGraphBytes GetData()
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new AbstractNodeJsonConverter<AbstractNode>());
        options.Converters.Add(new AbstractNodeJsonConverter<AbstractEdge>());

        var edges = JsonSerializer.Serialize(Edges.ToArray(), options);
        var serialized = JsonSerializer.Serialize(this.ToArray(), options);
        DataGraphBytes data = new() { Nodes = serialized, Edges = edges };
        return data;
    }

    public void InsertData(DataGraphBytes data)
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new AbstractNodeJsonConverter<AbstractNode>());
        options.Converters.Add(new AbstractNodeJsonConverter<AbstractEdge>());

        var nodes = JsonSerializer.Deserialize<AbstractNode[]>(data.Nodes, options);
        var edges = JsonSerializer.Deserialize<AbstractEdge[]>(data.Edges, options);

        Parallel.For(0, nodes.Length, (i) => Add(nodes[i]));
        Parallel.For(0, edges.Length, (i) => TryAddEdge(edges[i]));
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

        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode.Id && x.ToNode == toNode.Id);
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

        var found = Edges.AsParallel().Where(x => x.FromNode == fromNode.Id);
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

        var found = Edges.AsParallel().Where(x => x.ToNode == toNode.Id);
        if (found.Any())
        {
            edges = found.ToList();
            return true;
        }

        return false;
    }

}
public struct DataGraphBytes
{
    public string Nodes { get; set; }

    public string Edges { get; set; }
}

public class AbstractNodeJsonConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            if (!jsonDoc.RootElement.TryGetProperty("Type", out var typeProp))
            {
                throw new JsonException("Missing 'Type' discriminator property.");
            }

            string typeName = typeProp.GetString();
            Type type = Type.GetType(typeName);

            if (type == null || !typeof(AbstractNode).IsAssignableFrom(type))
            {
                throw new JsonException($"Unknown or invalid type: {typeName}");
            }

            return (T)JsonSerializer.Deserialize(jsonDoc.RootElement.GetRawText(), type, options);
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().FullName);

        var json = JsonSerializer.Serialize(value, value.GetType(), options);
        using (var jsonDoc = JsonDocument.Parse(json))
        {
            foreach (var prop in jsonDoc.RootElement.EnumerateObject())
            {
                prop.WriteTo(writer);
            }
        }
        writer.WriteEndObject();
    }
}
