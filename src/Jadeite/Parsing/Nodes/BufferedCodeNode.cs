using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class BufferedCodeNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public Token BeginningToken { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Expression { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        public JadeiteSyntaxKind Kind { get; }

        internal BufferedCodeNode(JadeiteSyntaxKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return BeginningToken;
            yield return Expression;
            yield return EndOfLine;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.AssertKindIsOneOf(Kind, JadeiteSyntaxKind.EscapedBufferedCode, JadeiteSyntaxKind.UnescapedBufferedCode);
            ParsingDebug.Assert((Kind == JadeiteSyntaxKind.EscapedBufferedCode && BeginningToken.Kind == JadeiteSyntaxKind.Equals)
                || (Kind == JadeiteSyntaxKind.UnescapedBufferedCode && BeginningToken.Kind == JadeiteSyntaxKind.BangEquals));
        }
    }
}