using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.SelfClosingBody,
        JadeiteKind.BreakStatement,
        JadeiteKind.ContinueStatement,
        JadeiteKind.Statement
    )]
    public sealed class LineTerminatedNode : INode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        public JadeiteKind Kind { get; }

        internal LineTerminatedNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LeftHandSide;
            yield return EndOfLine;
        }
    }
}