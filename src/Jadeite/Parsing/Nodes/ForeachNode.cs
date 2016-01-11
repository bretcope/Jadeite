using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ForeachStatement)]
    public sealed class ForeachNode : INode
    {
        [AssertKind(JadeiteKind.ForeachKeyword)]
        public Token ForeachKeyword { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement AssignmentTarget { get; internal set; }
        [AssertKind(JadeiteKind.InKeyword)]
        public Token InKeyword { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Expression { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ForeachStatement;

        internal ForeachNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ForeachKeyword;
            yield return AssignmentTarget;
            yield return InKeyword;
            yield return Expression;
            yield return EndOfLine;
            yield return Block;
        }
    }
}