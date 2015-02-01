namespace Jadeite.Parser.Nodes
{
    public class TagNode : AttributesNode
    {
        public override string Type => "Tag";

        public string Name { get; }
        public bool Buffer { get; set; }

        public TagNode(string name, BlockNode block = null)
        {
            Name = name;
            Block = block ?? new BlockNode();
        }
    }
}