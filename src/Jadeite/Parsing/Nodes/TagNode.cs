using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TagNode : INode
    {
        [AssertKind(true, JadeiteSyntaxKind.HtmlIdentifier)]
        public Token ElementName { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.ClassOrIdList)]
        public ClassOrIdListNode ClassOrIdList { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.TagAttributes)]
        public BracketedNode Attributes { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.AndAttributes)]
        public AndAttributesNode AndAttributes { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Body { get; internal set; }

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
    }
}