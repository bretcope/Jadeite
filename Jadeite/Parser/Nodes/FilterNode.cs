using System.Collections.Generic;

namespace Jadeite.Parser.Nodes
{
    public class FilterNode : Node
    {
        public override string Type => "Filter";

        public string Value { get; }
        public List<AttributeItem> Attributes { get; }

        public FilterNode(string value, BlockNode block, List<AttributeItem> attributes)
        {
            Value = value;
            Block = block;
            Attributes = attributes;
        }
}