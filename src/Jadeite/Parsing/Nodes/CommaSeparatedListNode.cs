using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public abstract class CommaSeparatedListNode<T> : INode where T : ISyntaxElement
    {
        public SyntaxList<T> Items { get; } = new SyntaxList<T>();
        public SyntaxList<Token> Commas { get; } = new SyntaxList<Token>();
        public abstract JadeiteSyntaxKind Kind { get; }

        internal CommaSeparatedListNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                yield return Items[i];

                if (i < Commas.Count)
                    yield return Commas[i];
            }
        }

        internal void AddComma(Token comma)
        {
            ParsingDebug.AssertKindIsOneOf(comma.Kind, JadeiteSyntaxKind.Comma);
            ParsingDebug.Assert(Items.Count == Commas.Count + 1);

            Commas.Add(comma);
        }

        internal virtual void AddItem(T item)
        {
            ParsingDebug.Assert(Items.Count == Commas.Count);
            Items.Add(item);
        }
    }

    // All of the classes which inherit from CommaSeparatedListNode follows:

    public sealed class ArgumentListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ArgumentList;

        internal ArgumentListNode() { }
    }

    public sealed class TagAttributesListNode : CommaSeparatedListNode<TagAttributeNode>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TagAttributesList;

        internal TagAttributesListNode() { }
    }

    public sealed class ExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ExpressionList;

        internal ExpressionListNode() { }
    }

    public sealed class StatementExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.StatementExpressionList;

        internal StatementExpressionListNode() { }
    }

    public sealed class IdentifierOrDeclarationListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.IdentifierOrDeclarationList;

        internal IdentifierOrDeclarationListNode() { }

        internal override void AddItem(ISyntaxElement item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteSyntaxKind.CodeIdentifier, JadeiteSyntaxKind.SingleVariableDeclaration);
            base.AddItem(item);
        }
    }

    public sealed class CodeIdentifierListNode : CommaSeparatedListNode<Token>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.CodeIdentifierList;

        internal CodeIdentifierListNode() { }

        internal override void AddItem(Token item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteSyntaxKind.CodeIdentifier);
            base.AddItem(item);
        }
    }
}