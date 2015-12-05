using System;
using System.Collections.Generic;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        // don't need stack because the tag state is never nested
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
                    if (IsWhiteSpaceNewLineOrEnd(CurrentChar()))
                    {
                        ExitState();
                        TransitionToPipelessText();
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
                case '=':
                    ConsumeToken(TokenType.Equals, 1);
                    ConsumeWhiteSpaceAsTrivia();
                    ExitState();
                    TransitionToCode(_isTagInterpolation ? TokenType.CloseSquareBracket : TokenType.NewLine);
                    return;
                case '(':
                    ConsumeToken(TokenType.OpenParen, 1);
                    TransitionToAttributes();
                    return;
                case ' ':
                    ConsumeTrivia(1);
                    ExitState();
                    TransitionToBody(_isTagInterpolation);
                    return;
                case ']':
                    if (_isTagInterpolation)
                    {
                        ExitState();
                        TransitionToBody(true);
                        return;
                    }
                    throw new Exception($"Unexpected token ']' at line {Line}, column {Column}.");
                case '\r':
                case '\n':
                case INVALID_CHAR:
                    ExitState();
                    return;
                default:
                    ScanHtmlIdentifierOrThrow();
                    return;
            }
        }

        private void ScanHtmlIdentifierOrThrow()
        {
            if (!IsWordOrHyphenCharacter(CurrentChar()))
                throw new Exception($"Unexpected token at line {Line}, column {Column}.");

            var len = 1;
            for (var i = Index + 1; i < Length; i++)
            {
                if (IsWordOrHyphenCharacter(Input[i]))
                    len++;
                else
                    break;
            }

            ConsumeToken(TokenType.HtmlIdentifier, len);
        }
    }
}
