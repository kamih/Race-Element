namespace RaceElement.Graph.Node;
/// <summary>
/// Provides an abstract implementation of a node used in <see cref="DataGraph"/>
/// </summary>
public abstract record class AbstractNode
{
    public Guid Id { get; private init; } = Guid.NewGuid();

    public virtual void Accept(NodeVisitor visitor) => visitor.Visit(this);
}

public abstract record class AbstractIntegerNode : AbstractNode
{
    public AbstractIntegerNode(int value) => Value = value;

    public int Value { get; init; }
}
