namespace Jadeite.Parser.Nodes
{
    public class CommentNode : Node
    {
         public override string Type => "Comment";

        public string Value { get; }
        public bool Buffer { get; }

        public CommentNode(string value, bool buffer)
        {
            Value = value;
            Buffer = buffer;
        }
    }
}