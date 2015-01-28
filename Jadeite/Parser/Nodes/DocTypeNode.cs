namespace Jadeite.Parser.Nodes
{
    public class DocTypeNode : Node
    {
        public override string Type => "Doctype";

        public string Value { get; }

        public DocTypeNode(string value)
        {
            Value = value;
        }
    }
}