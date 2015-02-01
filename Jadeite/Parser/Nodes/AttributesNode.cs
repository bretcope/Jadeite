using System.Collections.Generic;
using System.Linq;

namespace Jadeite.Parser.Nodes
{
    public class AttributesNode : Node
    {
        private readonly List<AttributeItem> _attributes = new List<AttributeItem>();
        private readonly List<AttributesBlockToken> _attributeBlocks = new List<AttributesBlockToken>(); 

        public override string Type => "Attrs";
        public IReadOnlyList<AttributeItem> Attributes { get; }
        public IReadOnlyList<AttributesBlockToken> AttributesBlocks { get; }
        public bool SelfClosing { get; set; }

        public AttributesNode()
        {
            Attributes = _attributes.AsReadOnly();
            AttributesBlocks = _attributeBlocks.AsReadOnly();
        }

        public void SetAttribute(AttributeItem attribute)
        {
            if (attribute.Name != "class" && _attributes.Any(a => a.Name == attribute.Name))
                throw new JadeiteParserException("Duplicate attribute '" + attribute.Name + "' is not allowed.");

            _attributes.Add(attribute);
        }

        public void AddAttributes(AttributesBlockToken token)
        {
            _attributeBlocks.Add(token);
        }
    }
}
