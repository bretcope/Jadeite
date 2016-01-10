using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.UnbufferedCode)]
    public sealed class UnbufferedCodeNode : INode
    {
        [AssertKind(true, JadeiteKind.Minus)]
        public Token PrefixHyphen { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement Statement { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.UnbufferedCode;

        internal UnbufferedCodeNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (PrefixHyphen != null)
                yield return PrefixHyphen;

            yield return Statement;
        }
    }
}