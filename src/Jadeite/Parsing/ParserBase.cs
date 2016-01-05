using System.Diagnostics;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public abstract class ParserBase
    {
        private readonly Lexer _lexer;

        // Lexer proxy members
        protected Token Current => _lexer.Current();
        protected Token LookAhead => _lexer.LookAhead();
        protected Token Advance() => _lexer.Advance();
        protected Token AdvanceKind(JadeiteSyntaxKind kind) => _lexer.AdvanceKind(kind);

        internal ParserBase(string input, string indent)
        {
            _lexer = new Lexer(input, indent);
        }

        [Conditional("DEBUG")]
        protected void AssertCurrentKind(params JadeiteSyntaxKind[] oneOf)
        {
            ParsingDebug.AssertKindIsOneOf(Current.Kind, oneOf);
        }
    }
}