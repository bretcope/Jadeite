using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private bool _isTagInterpolation;

        private void TransitionToTag(bool isInterpolation)
        {
            PushState(LexerState.Tag);
            _isTagInterpolation = isInterpolation;
        }

        private void ScanTag()
        {
            switch (CurrentChar())
            {
                case '.':
                    ConsumeToken(TokenType.Dot, 1);
                    switch (CurrentChar())
                    {
                        case ' ':
                        case '\t':
                        case '\r':
                        case '\n':
                            ExitState();
                            TransitionToPipelessText();
                            break;
                    }
                    return;
                case '#':
                    ConsumeToken(TokenType.Hash, 1);
                    return;
                case '&':
                    throw new NotImplementedException("Haven't implemented &attributes yet.");
                case ':':
                    var tok = ConsumeToken(TokenType.Colon, 1);
                    if (ConsumeWhiteSpaceAsTrivia() > 0)
                        tok.TrailingTrivia = PopTrivia();
                    return;
                case '(':
                    ConsumeToken(TokenType.OpenParen, 1);
                    TransitionToAttributes();
                    return;
                case ' ':
                    ExitState();
                    TransitionToBody();
                    return;
                default:
                    // todo - scan html identifiers
                    return;
            }
        }
    }
}
