namespace RaceElement.Graph.Node.ValueNodes;

public abstract record class AbstractFloatNode : AbstractNode
{
    public AbstractFloatNode(float value) => Value = value;

    public float Value { get; init; }
}
