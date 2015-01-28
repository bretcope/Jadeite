namespace Jadeite.Parser.Nodes
{
    public class BlockCommentNode : Node
    {
         public override string Type => "BlockComment";

        public bool Buffer { get; }
        public string Value { get; }

        public BlockCommentNode(string value, BlockNode block, bool buffer)
        {
            Value = value;
            Block = block;
            Buffer = buffer;
        }
    }
}