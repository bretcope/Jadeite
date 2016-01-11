using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ReturnStatement)]
    public sealed class ReturnNode : INode
    {
        [AssertKind(JadeiteKind.ReturnKeyword)]
        public Token ReturnKeyword { get; internal set; }
        [AssertKind(true, JadeiteKind.ExpressionList)]
        public ExpressionListNode Expressions { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ReturnStatement;

        internal ReturnNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ReturnKeyword;

            if (Expressions != null)
                yield return Expressions;

            yield return EndOfLine;
        }
    }
}