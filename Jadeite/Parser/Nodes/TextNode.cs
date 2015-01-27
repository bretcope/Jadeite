namespace Jadeite.Parser.Nodes
{
    public class TextNode : Node
    {
        public override string Type => "Text";
        public override bool IsText => true;

        public string Value { get; set; }

        public TextNode(string value)
        {
            Value = value;
        }
    }
}