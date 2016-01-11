using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ForStatement)]
    public sealed class ForNode : INode
    {
        [AssertKind(JadeiteKind.ForKeyword)]
        public Token ForKeyword { get; internal set; }
        [AssertKind(true, JadeiteKind.StatementExpressionList)]
        public StatementExpressionListNode Initializer { get; internal set; }
        [AssertKind(JadeiteKind.SemiColon)]
        public Token FirstSeparator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Condition { get; internal set; }
        [AssertKind(JadeiteKind.SemiColon)]
        public Token SecondSeparator { get; internal set; }
        [AssertKind(true, JadeiteKind.StatementExpressionList)]
        public StatementExpressionListNode Iterator { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ForeachStatement;

        internal ForNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ForKeyword;

            if (Initializer != null)
                yield return Initializer;

            yield return FirstSeparator;
            yield return Condition;
            yield return SecondSeparator;

            if (Iterator != null)
                yield return Iterator;

            yield return EndOfLine;
            yield return Block;
        }
    }
}