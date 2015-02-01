namespace Jadeite.Parser.Nodes
{
    public class LiteralNode : Node
    {
        public override string Type => "Literal";

        public string Value { get; }

        public LiteralNode(string value)
        {
            Value = value;
        }
    }
}