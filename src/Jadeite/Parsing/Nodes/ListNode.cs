using System.Collections.Generic;
using System.Linq;

namespace Jadeite.Parsing.Nodes
{
    public abstract class ListNode<T> : INode where T : ISyntaxElement
    {
        private readonly SyntaxList<T> _children = new SyntaxList<T>();
        
        public abstract JadeiteKind Kind { get; }
        public int ChildrenCount => _children.Count;

        internal ListNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            return _children.Cast<ISyntaxElement>();
        }

        internal virtual void Add(T element)
        {
            _children.Add(element);
        }
    }

    // All of the classes which inherit from ListNode follows:

    [NodeKind(JadeiteKind.TextBodyElementList)]
    public sealed class TextBodyElementListNode : ListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.TextBodyElementList;

        internal TextBodyElementListNode() { }

        internal override void Add(ISyntaxElement element)
        {
            ParsingDebug.AssertKindIsOneOf(
                element.Kind,
                JadeiteKind.HtmlText,
                JadeiteKind.InterpolatedTag,
                JadeiteKind.EscapedInterpolatedExpression,
                JadeiteKind.UnescapedInterpolatedExpression);

            base.Add(element);
        }
    }

    [NodeKind(JadeiteKind.MixinList)]
    public sealed class MixinListNode : ListNode<MixinDefinitionNode>
    {
        public override JadeiteKind Kind => JadeiteKind.MixinList;

        internal MixinListNode() { }
    }

    [NodeKind(JadeiteKind.DocumentBody)]
    public sealed class DocumentBodyNode : ListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.DocumentBody;

        internal DocumentBodyNode() { }
    }

    [NodeKind(JadeiteKind.ClassOrIdList)]
    public sealed class ClassOrIdListNode : ListNode<ClassOrIdNode>
    {
        public override JadeiteKind Kind => JadeiteKind.ClassOrIdList;

        internal ClassOrIdListNode() { }
    }

    [NodeKind(JadeiteKind.EndOfLineList)]
    public sealed class EndOfLineListNode : ListNode<Token>
    {
        public override JadeiteKind Kind => JadeiteKind.EndOfLineList;

        internal EndOfLineListNode() { }

        internal override void Add(Token element)
        {
            ParsingDebug.AssertKindIsOneOf(element.Kind, JadeiteKind.EndOfLine);
            base.Add(element);
        }
    }

    [NodeKind(JadeiteKind.StatementList)]
    public sealed class StatementListNode : ListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.StatementList;

        internal StatementListNode() { }
    }

    // todo SwitchSectionListNode

    // todo SwitchLabelListNode
}