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

        protected void AddItem(T item)
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

        internal void AddArgument(ISyntaxElement argument)
        {
            AddItem(argument);
        }
    }

    public sealed class TagAttributeListNode : CommaSeparatedListNode<TagAttributeNode>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.TagAttributesList;

        internal TagAttributeListNode() { }

        internal void AddTagAttribute(TagAttributeNode tagAttribute)
        {
            AddItem(tagAttribute);
        }
    }

    public sealed class ExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.ExpressionList;

        internal ExpressionListNode() { }

        internal void AddExpression(ISyntaxElement expression)
        {
            AddItem(expression);
        }
    }

    public sealed class StatementExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.StatementExpressionList;

        internal StatementExpressionListNode() { }

        internal void AddStatementExpression(ISyntaxElement statementExpression)
        {
            AddItem(statementExpression);
        }
    }

    public sealed class IdentifierOrDeclarationListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.IdentifierOrDeclarationList;

        internal IdentifierOrDeclarationListNode() { }

        internal void AddIdentifierOrDeclaration(ISyntaxElement item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteSyntaxKind.CodeIdentifier, JadeiteSyntaxKind.SingleVariableDeclaration);
            AddItem(item);
        }
    }

    public sealed class CodeIdentifierListNode : CommaSeparatedListNode<Token>
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.CodeIdentifierList;

        internal CodeIdentifierListNode() { }

        internal void AddCodeIdentifier(Token item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteSyntaxKind.CodeIdentifier);
            AddItem(item);
        }
    }
}