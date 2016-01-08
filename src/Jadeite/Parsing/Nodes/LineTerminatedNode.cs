using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class LineTerminatedNode : INode
    {
        [AssertNotNull]
        public ISyntaxElement LeftHandSide { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        public JadeiteSyntaxKind Kind { get; }

        internal LineTerminatedNode(JadeiteSyntaxKind kind)
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