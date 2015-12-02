using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private TokenType _codeExitToken;

        private void TransitionToCode(TokenType exitToken)
        {
            PushState(LexerState.Code);
            _codeExitToken = exitToken;
        }

        private void ScanCode()
        {
            throw new NotImplementedException();
        }
    }
}
