using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public abstract class CommaSeparatedListNode<T> : INode where T : ISyntaxElement
    {
        public SyntaxList<T> Items { get; } = new SyntaxList<T>();
        public SyntaxList<Token> Commas { get; } = new SyntaxList<Token>();
        public abstract JadeiteKind Kind { get; }

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
            ParsingDebug.AssertKindIsOneOf(comma.Kind, JadeiteKind.Comma);
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

    [NodeKind(JadeiteKind.ArgumentList)]
    public sealed class ArgumentListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.ArgumentList;

        internal ArgumentListNode() { }

        internal void AddArgument(ISyntaxElement argument)
        {
            AddItem(argument);
        }
    }

    [NodeKind(JadeiteKind.TagAttributesList)]
    public sealed class TagAttributeListNode : CommaSeparatedListNode<TagAttributeNode>
    {
        public override JadeiteKind Kind => JadeiteKind.TagAttributesList;

        internal TagAttributeListNode() { }

        internal void AddTagAttribute(TagAttributeNode tagAttribute)
        {
            AddItem(tagAttribute);
        }
    }

    [NodeKind(JadeiteKind.ExpressionList)]
    public sealed class ExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.ExpressionList;

        internal ExpressionListNode() { }

        internal void AddExpression(ISyntaxElement expression)
        {
            AddItem(expression);
        }
    }

    [NodeKind(JadeiteKind.StatementExpressionList)]
    public sealed class StatementExpressionListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.StatementExpressionList;

        internal StatementExpressionListNode() { }

        internal void AddStatementExpression(ISyntaxElement statementExpression)
        {
            AddItem(statementExpression);
        }
    }

    [NodeKind(JadeiteKind.IdentifierOrDeclarationList)]
    public sealed class IdentifierOrDeclarationListNode : CommaSeparatedListNode<ISyntaxElement>
    {
        public override JadeiteKind Kind => JadeiteKind.IdentifierOrDeclarationList;

        internal IdentifierOrDeclarationListNode() { }

        internal void AddIdentifierOrDeclaration(ISyntaxElement item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteKind.CodeIdentifier, JadeiteKind.SingleVariableDeclaration);
            AddItem(item);
        }
    }

    [NodeKind(JadeiteKind.CodeIdentifierList)]
    public sealed class CodeIdentifierListNode : CommaSeparatedListNode<Token>
    {
        public override JadeiteKind Kind => JadeiteKind.CodeIdentifierList;

        internal CodeIdentifierListNode() { }

        internal void AddCodeIdentifier(Token item)
        {
            ParsingDebug.AssertKindIsOneOf(item.Kind, JadeiteKind.CodeIdentifier);
            AddItem(item);
        }
    }
}