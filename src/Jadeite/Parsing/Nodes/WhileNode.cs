using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.WhileStatement)]
    public sealed class WhileNode : INode
    {
        [AssertKind(JadeiteKind.WhileKeyword)]
        public Token WhileKeyword { get; internal set; }
        public ISyntaxElement Condition { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.WhileStatement;

        internal WhileNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return WhileKeyword;

            if (Condition != null)
                yield return Condition;

            yield return EndOfLine;
            yield return Block;
        }
    }
}