using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TagNode : INode
    {
        public Token ElementName { get; private set; }
        public ClassOrIdListNode ClassOrIdList { get; private set; }
        public BracketedNode Attributes { get; private set; }
        public AndAttributesNode AndAttributes { get; private set; }
        public ISyntaxElement Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Tag;

        internal TagNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (ElementName != null)
                yield return ElementName;

            if (ClassOrIdList != null)
                yield return ClassOrIdList;

            if (Attributes != null)
                yield return Attributes;

            if (AndAttributes != null)
                yield return AndAttributes;

            yield return Body;
        }

        internal void SetElementName(Token name)
        {
            ParsingDebug.AssertKindIsOneOf(name.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(ElementName == null);
            ElementName = name;
        }

        internal void SetClassOrIdList(ClassOrIdListNode list)
        {
            ParsingDebug.Assert(ClassOrIdList == null);
            ClassOrIdList = list;
        }

        internal void SetAttributes(BracketedNode attributes)
        {
            ParsingDebug.AssertKindIsOneOf(attributes.Kind, JadeiteSyntaxKind.TagAttributes);
            ParsingDebug.Assert(Attributes == null);
            Attributes = attributes;
        }

        internal void SetAndAttributes(AndAttributesNode andAttributes)
        {
            ParsingDebug.Assert(AndAttributes == null);
            AndAttributes = andAttributes;
        }

        internal void SetBody(ISyntaxElement body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }
    }
}