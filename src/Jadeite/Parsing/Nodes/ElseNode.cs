using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.ElseStatement)]
    public sealed class ElseNode : INode, ICustomDebugNode
    {
        [AssertKind(JadeiteKind.ElseKeyword)]
        public Token ElseKeyword { get; internal set; }
        [AssertKind(true, JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }
        [AssertKind(true, JadeiteKind.IfStatement)]
        public IfNode IfStatement { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.ElseStatement;

        internal ElseNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ElseKeyword;

            if (EndOfLine != null)
            {
                yield return EndOfLine;
                yield return Block;
            }
            else
            {
                yield return IfStatement;
            }
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert((EndOfLine == null) == (Block == null));
            ParsingDebug.Assert((EndOfLine == null) != (IfStatement == null));
        }
    }
}