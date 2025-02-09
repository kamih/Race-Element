namespace RaceElement.Graph.Edge;

/// <summary>
/// A simple relationship between two nodes.<br/>
/// The <see cref="AbstractEdge.FromNodeId"/> owns the <see cref="AbstractEdge.ToNodeId"/>
/// </summary>
public sealed record class OwnsEdge : AbstractEdge { }
