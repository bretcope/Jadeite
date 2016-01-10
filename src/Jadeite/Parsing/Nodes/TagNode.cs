using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.Tag, JadeiteKind.InterpolatedTagDefinition)]
    public sealed class TagNode : INode
    {
        [AssertKind(true, JadeiteKind.HtmlIdentifier)]
        public Token ElementName { get; internal set; }
        [AssertKind(true, JadeiteKind.ClassOrIdList)]
        public ClassOrIdListNode ClassOrIdList { get; internal set; }
        [AssertKind(true, JadeiteKind.TagAttributes)]
        public BracketedNode Attributes { get; internal set; }
        [AssertKind(true, JadeiteKind.AndAttributes)]
        public AndAttributesNode AndAttributes { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Body { get; internal set; }

        public JadeiteKind Kind { get; }

        internal TagNode(bool interpolated)
        {
            Kind = interpolated ? JadeiteKind.InterpolatedTagDefinition : JadeiteKind.Tag;
        }

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

        public bool IsExplicitlySelfClosing()
        {
            if (Body == null)
                return false;

            return Body.Kind == JadeiteKind.SelfClosingBody || Body.Kind == JadeiteKind.ForwardSlash;
        }
    }
}