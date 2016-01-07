using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class UnbufferedCodeNode : INode
    {
        [AssertKind(true, JadeiteSyntaxKind.Minus)]
        public Token PrefixHyphen { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Statement { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.UnbufferedCode;

        internal UnbufferedCodeNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (PrefixHyphen != null)
                yield return PrefixHyphen;

            yield return Statement;
        }
    }
}