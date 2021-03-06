﻿using System;

namespace Jadeite.Parsing
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
                    ConsumeToken(JadeiteKind.Dot, 1);
                    if (IsWhiteSpaceNewLineOrEnd(CurrentChar()))
                    {
                        ConsumeWhiteSpaceAsTrivia();
                        if (!IsNewLineOrEnd(CurrentChar()))
                            throw new Exception($"Invalid pipeless text syntax. Expected a new line after the dot on line {Line}."); // todo

                        ExitState();
                        TransitionToPipelessText();
                    }
                    return;
                case '#':
                    ConsumeToken(JadeiteKind.Hash, 1);
                    return;
                case '&':
                    ConsumeToken(JadeiteKind.And, 1);
                    if (!TryConsumeKeyword(JadeiteKind.AttributesKeyword))
                        throw new Exception($"Unexpected '&' not followed by 'attributes' at Line {Line} Column {Column}."); // todo

                    if (CurrentChar() != '(')
                        throw new Exception($"Expected an open paren '(' to follow &attributes at Line {Line} Column {Column}."); // todo

                    ConsumeToken(JadeiteKind.OpenParen, 1);
                    TransitionToAndAttributes();
                    return;
                case ':':
                    var tok = ConsumeToken(JadeiteKind.Colon, 1);
                    if (ConsumeWhiteSpaceAsTrivia() > 0)
                        tok.TrailingTrivia = PopTrivia();
                    return;
                case '=':
                    ConsumeToken(JadeiteKind.Equals, 1);
                    ExitState();
                    TransitionToCode(_isTagInterpolation ? CodeScanMode.SquareInterpolation : CodeScanMode.Line);
                    return;
                case '!':
                    if (NextChar() == '=')
                    {
                        ConsumeToken(JadeiteKind.BangEquals, 2);
                        ExitState();
                        TransitionToCode(_isTagInterpolation ? CodeScanMode.SquareInterpolation : CodeScanMode.Line);
                        return;
                    }
                    throw new Exception($"Unexpected token ']' at line {Line}, column {Column}."); // todo
                case '(':
                    ConsumeToken(JadeiteKind.OpenParen, 1);
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
                    throw new Exception($"Unexpected token ']' at line {Line}, column {Column}."); // todo
                case '/':
                    ConsumeToken(JadeiteKind.ForwardSlash, 1);
                    ConsumeWhiteSpaceAsTrivia();
                    if (!IsNewLineOrEnd(CurrentChar()))
                        throw new Exception($"Expected end of line after the '/' on line {Line}."); // todo
                    ExitState();
                    return;
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

            ConsumeToken(JadeiteKind.HtmlIdentifier, len, useTextAsValue: true);
        }
    }
}
