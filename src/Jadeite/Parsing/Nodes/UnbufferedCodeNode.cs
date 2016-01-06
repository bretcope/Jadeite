using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class UnbufferedCodeNode : INode
    {
        public Token PrefixHyphen { get; private set; }
        public ISyntaxElement Statement { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.UnbufferedCode;

        internal UnbufferedCodeNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (PrefixHyphen != null)
                yield return PrefixHyphen;

            yield return Statement;
        }

        internal void SetPrefix(Token prefix)
        {
            ParsingDebug.AssertKindIsOneOf(prefix.Kind, JadeiteSyntaxKind.Minus);
            ParsingDebug.Assert(PrefixHyphen == null);
            PrefixHyphen = prefix;
        }

        internal void SetStatement(ISyntaxElement statement)
        {
            ParsingDebug.Assert(Statement == null);
            Statement = statement;
        }
    }
}