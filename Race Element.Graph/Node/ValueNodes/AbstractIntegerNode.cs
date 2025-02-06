namespace RaceElement.Graph.Node.ValueNodes;

/// <summary>
/// Create an abstract implementation of a simple integer node with a value.
/// </summary>
public abstract record class AbstractIntegerNode : AbstractNode
{
    public AbstractIntegerNode(int value) => Value = value;

    public int Value { get; init; }
}
