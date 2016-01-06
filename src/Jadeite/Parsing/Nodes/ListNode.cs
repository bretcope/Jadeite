using System.Collections.Generic;
using System.Linq;

namespace Jadeite.Parsing.Nodes
{
    public abstract class ListNode<T> : INode where T : ISyntaxElement
    {
        private readonly SyntaxList<T> _children = new SyntaxList<T>();
        
        public abstract JadeiteSyntaxKind Kind { get; }
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

    public sealed class TextBodyElementListNode : ListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TextBodyElementList;

        internal TextBodyElementListNode() { }

        internal override void Add(ISyntaxElement element)
        {
            ParsingDebug.AssertKindIsOneOf(
                element.Kind,
                JadeiteSyntaxKind.HtmlText,
                JadeiteSyntaxKind.InterpolatedTag,
                JadeiteSyntaxKind.EscapedInterpolatedExpression,
                JadeiteSyntaxKind.UnescapedInterpolatedExpression);

            base.Add(element);
        }
    }

    public sealed class MixinListNode : ListNode<MixinDefinitionNode>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinList;

        internal MixinListNode() { }
    }

    public sealed class DocumentBodyNode : ListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DocumentBody;

        internal DocumentBodyNode() { }
    }

    public sealed class ClassOrIdListNode : ListNode<ClassOrIdNode>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ClassOrIdList;

        internal ClassOrIdListNode() { }
    }

    public sealed class EndOfLineListNode : ListNode<Token>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.EndOfLineList;

        internal EndOfLineListNode() { }

        internal override void Add(Token element)
        {
            ParsingDebug.AssertKindIsOneOf(element.Kind, JadeiteSyntaxKind.EndOfLine);
            base.Add(element);
        }
    }

    public sealed class StatementListNode : ListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.StatementList;

        internal StatementListNode() { }
    }

    // todo SwitchSectionListNode

    // todo SwitchLabelListNode
}