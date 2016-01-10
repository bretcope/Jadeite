using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.TypeIdentifier)]
    public sealed class TypeIdentifierNode : INode, ICustomDebugNode
    {
        public SyntaxList<Token> Parts { get; } = new SyntaxList<Token>();

        public JadeiteKind Kind => JadeiteKind.TypeIdentifier;

        internal TypeIdentifierNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            return Parts;
        }

        internal void AddToken(Token tok)
        {
            Parts.Add(tok);
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(Parts.Count % 2 == 1); // should always be an odd number of parts

            for (var i = 0; i < Parts.Count; i++)
            {
                var tok = Parts[i];
                if (i % 2 == 0)
                {
                    ParsingDebug.Assert(tok.Kind == JadeiteKind.CodeIdentifier || SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword));
                }
                else
                {
                    ParsingDebug.AssertKindIsOneOf(tok.Kind, JadeiteKind.Dot);
                }
            }
        }
    }
}