namespace RaceElement.Graph.Edge;

/// <summary>
/// A simple relationship between two nodes.<br/>
/// The <see cref="AbstractEdge.ParentId"/> owns the <see cref="AbstractEdge.ChildId"/>
/// </summary>
public sealed record class OwnsEdge : AbstractEdge { }
