using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.IfStatement)]
    public sealed class IfNode : INode
    {
        [AssertKind(JadeiteKind.IfKeyword)]
        public Token IfKeyword { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Condition { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }
        [AssertKind(true, JadeiteKind.ElseStatement)]
        public ElseNode ElseStatement { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.IfStatement;

        internal IfNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return IfKeyword;
            yield return Condition;
            yield return EndOfLine;
            yield return Block;

            if (ElseStatement != null)
                yield return ElseStatement;
        }
    }
}