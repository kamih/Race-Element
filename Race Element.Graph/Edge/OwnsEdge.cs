namespace RaceElement.Graph.Edge;

/// <summary>
/// A simple relationship between two nodes.<br/>
/// The <see cref="AbstractEdge.FromNode"/> owns the <see cref="AbstractEdge.ToNode"/>
/// </summary>
public sealed record class OwnsEdge : AbstractEdge { }
