
using System;

namespace Jadeite.Internals
{
    public partial class Lexer
    {
        private void TransitionToDocument()
        {
            if (_stateStack.Count > 0)
                throw new Exception("Can only transition to document state at the beginning of the input."); // todo

            PushState(LexerState.Document);
        }

        private void ScanDocument()
        {
            switch (CurrentChar())
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    TransitionToIndent();
                    return;
                case INVALID_CHAR:
                    ConsumeToken(TokenType.EndOfInput, 0);
                    return;
                case '|':
                    var tok = ConsumeToken(TokenType.Pipe, 1);
                    switch (CurrentChar())
                    {
                        case ' ':
                            ConsumeTrivia(1);
                            tok.TrailingTrivia = PopTrivia();
                            TransitionToBody(isInterpolation: false);
                            return;
                        case '\n':
                        case '\r':
                        case INVALID_CHAR:
                            return;
                        default:
                            throw new Exception($"Expected pipe to be followed by a space on line {Line}."); // todo
                    }
                case '/':
                    if (ScanLineComment() == null)
                        throw new Exception($"Unkown token at line {Line}, column {Column}.");
                    return;
                case '.':
                case '#':
                case '&':
                    TransitionToTag(isInterpolation: false);
                    return;
                default:
                    if (!TryTransitionToKeyword())
                        TransitionToTag(isInterpolation: false);
                    return;
            }
        }

        private bool TryTransitionToKeyword()
        {
            var tok = ConsumePossibleKeyword();
            if (tok == null)
                return false;

            switch (tok.Type)
            {
                case TokenType.Each:
                case TokenType.If:
                case TokenType.Else:
                case TokenType.Switch:
                case TokenType.Case:
                case TokenType.Model:
                    TransitionToCode(exitToken: TokenType.NewLine);
                    return true;
                case TokenType.Extends:
                case TokenType.Prepend:
                case TokenType.Append:
                case TokenType.Block:
                case TokenType.Mixin:
                default:
                    throw new Exception($"Unsupported transition from document to keyword type {tok.Type}.");
            }
        }
    }
}
