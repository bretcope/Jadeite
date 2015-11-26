using System;

namespace Jadeite.Internals
{
    public class Lexer : LexerBase
    {
        private delegate LexMethod LexMethod();

        private LexMethod _lexMethod;

        public Lexer(string input) : base(input)
        {
            //
        }

        protected override void Lex()
        {
            _lexMethod = _lexMethod();
        }

        private LexMethod LexHeader()
        {
//            IsNext
//            ConsumeToken(TokenType.Model);
            // deal with insignificant whitespace
            //ConsumeToken();

            // new line
        }

        private void ScanIndent()
        {
            var character = CurrentCharacter();
            if (character == ' ' || character == '\t')
            {
                //
            }
        }

        private void ScanTagBody()
        {
            //
        }

        private void DiscoverIndent()
        {
            //
        }
    }
}