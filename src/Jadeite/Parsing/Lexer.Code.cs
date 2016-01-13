using System;
using System.Collections.Generic;

namespace Jadeite.Parsing
{
    public partial class Lexer
    {
        private enum CodeScanMode : byte
        {
            Attributes,
            CurlyInterpolation,
            SquareInterpolation,
            Line,
            UnbufferedBlock,
        }

        private CodeScanMode _codeScanMode;
        private int _codeUnbufferedIndent;
        private readonly Stack<char> _codeNestingStack = new Stack<char>();

        private void TransitionToCode(CodeScanMode codeScanMode)
        {
            PushState(LexerState.Code);
            _codeScanMode = codeScanMode;

            if (codeScanMode == CodeScanMode.UnbufferedBlock)
                _codeUnbufferedIndent = IndentLevel + 1;

            _codeNestingStack.Clear();
        }

        private void ScanCode()
        {
            ConsumeWhiteSpaceAsTrivia();

            var isBlock = _codeScanMode == CodeScanMode.UnbufferedBlock;
            if (isBlock && IndentLevel < _codeUnbufferedIndent)
            {
                switch (CurrentChar())
                {
                    case '\r':
                    case '\n':
                        TransitionToIndent();
                        return;
                    default:
                        ExitState();
                        return;
                }
            }

            var isNested = _codeNestingStack.Count > 0;

            switch (CurrentChar())
            {
                case '\r':
                case '\n':
                    if (isBlock)
                        TransitionToIndent();
                    else 
                        ExitState();
                    return;
                case INVALID_CHAR:
                    ExitState();
                    return;
                case '.':
                    ConsumeToken(JadeiteKind.Dot, 1);
                    return;
                case '|':
                    switch (NextChar())
                    {
                        case '|':
                            ConsumeToken(JadeiteKind.PipePipe, 2);
                            return;
                        case '=':
                            ConsumeToken(JadeiteKind.PipeEquals, 2);
                            return;
                        default:
                            ConsumeToken(JadeiteKind.Pipe, 1);
                            return;
                    }
                case '&':
                    switch (NextChar())
                    {
                        case '&':
                            ConsumeToken(JadeiteKind.AndAnd, 2);
                            return;
                        case '=':
                            ConsumeToken(JadeiteKind.AndEquals, 2);
                            return;
                        default:
                            ConsumeToken(JadeiteKind.And, 1);
                            return;

                    }
                case '^':
                    if (NextChar() == '=')
                        ConsumeToken(JadeiteKind.CaretEquals, 2);
                    else
                        ConsumeToken(JadeiteKind.Caret, 1);
                    return;
                case '~':
                    ConsumeToken(JadeiteKind.Tilde, 1);
                    return;
                case '+':
                    switch (NextChar())
                    {
                        case '+':
                            ConsumeToken(JadeiteKind.PlusPlus, 2);
                            break;
                        case '=':
                            ConsumeToken(JadeiteKind.PlusEquals, 2);
                            break;
                        default:
                            ConsumeToken(JadeiteKind.Plus, 1);
                            break;
                    }
                    return;
                case '-':
                    switch (NextChar())
                    {
                        case '-':
                            ConsumeToken(JadeiteKind.MinusMinus, 2);
                            break;
                        case '=':
                            ConsumeToken(JadeiteKind.MinusEquals, 2);
                            break;
                        default:
                            ConsumeToken(JadeiteKind.Minus, 1);
                            break;
                    }
                    return;
                case '*':
                    if (NextChar() == '=')
                        ConsumeToken(JadeiteKind.AsteriskEquals, 2);
                    else
                        ConsumeToken(JadeiteKind.Asterisk, 1);
                    return;
                case '/':
                    switch (NextChar())
                    {
                        case '=':
                            ConsumeToken(JadeiteKind.ForwardSlashEquals, 2);
                            return;
                        case '/':
                            ScanCodeLineComment();
                            return;
                        default:
                            ConsumeToken(JadeiteKind.ForwardSlash, 1);
                            return;
                    }
                case '%':
                    if (NextChar() == '=')
                        ConsumeToken(JadeiteKind.PercentEquals, 2);
                    else
                        ConsumeToken(JadeiteKind.Percent, 1);
                    return;
                case '=':
                    if (NextChar() == '=')
                        ConsumeToken(JadeiteKind.EqualsEquals, 2);
                    else
                        ConsumeToken(JadeiteKind.Equals, 1);
                    return;
                case '<':
                    switch (NextChar())
                    {
                        case '<':
                            if (CharAt(Index + 2) == '=')
                                ConsumeToken(JadeiteKind.LessThanLessThanEquals, 3);
                            else
                                ConsumeToken(JadeiteKind.LessThanLessThan, 2);
                            return;
                        case '=':
                            ConsumeToken(JadeiteKind.LessThanEquals, 2);
                            return;
                        default:
                            ConsumeToken(JadeiteKind.LessThan, 1);
                            return;
                    }
                case '>':
                    switch (NextChar())
                    {
                        case '>':
                            if (CharAt(Index + 2) == '=')
                                ConsumeToken(JadeiteKind.GreaterThanGreaterThanEquals, 3);
                            else
                                ConsumeToken(JadeiteKind.GreaterThanGreaterThan, 2);
                            return;
                        case '=':
                            ConsumeToken(JadeiteKind.GreaterThanEquals, 2);
                            return;
                        default:
                            ConsumeToken(JadeiteKind.GreaterThan, 1);
                            return;
                    }
                case ',':
                    if (!isNested && _codeScanMode == CodeScanMode.Attributes)
                    {
                        ExitState();
                        return;
                    }
                    ConsumeToken(JadeiteKind.Comma, 1);
                    return;
                case ';':
                    ConsumeToken(JadeiteKind.SemiColon, 1);
                    return;
                case '!':
                    if (NextChar() == '=')
                        ConsumeToken(JadeiteKind.BangEquals, 2);
                    else
                        ConsumeToken(JadeiteKind.Bang, 1);
                    return;
                case '?':
                    if (NextChar() == '?')
                        ConsumeToken(JadeiteKind.QuestionMarkQuestionMark, 2);
                    else
                        ConsumeToken(JadeiteKind.QuestionMark, 1);
                    return;
                case ':':
                    ConsumeToken(JadeiteKind.Colon, 1);
                    return;
                case '(':
                    ConsumeToken(JadeiteKind.OpenParen, 1);
                    _codeNestingStack.Push('(');
                    return;
                case ')':
                    if (isNested)
                    {
                        if (_codeNestingStack.Peek() == '(')
                            _codeNestingStack.Pop();
                    }
                    else if (_codeScanMode == CodeScanMode.Attributes)
                    {
                        ExitState();
                        return;
                    }
                    ConsumeToken(JadeiteKind.CloseParen, 1);
                    return;
                case '{':
                    ConsumeToken(JadeiteKind.OpenCurly, 1);
                    _codeNestingStack.Push('{');
                    return;
                case '}':
                    if (isNested)
                    {
                        if (_codeNestingStack.Peek() == '{')
                            _codeNestingStack.Pop();
                    }
                    else if (_codeScanMode == CodeScanMode.CurlyInterpolation)
                    {
                        // curly-interpolation is a special case where we want to exit state, but still consume the token
                        ExitState();
                    }
                    ConsumeToken(JadeiteKind.CloseCurly, 1);
                    return;
                case '[':
                    ConsumeToken(JadeiteKind.OpenSquareBracket, 1);
                    _codeNestingStack.Push('[');
                    return;
                case ']':
                    if (isNested)
                    {
                        if (_codeNestingStack.Peek() == '{')
                            _codeNestingStack.Pop();
                    }
                    else if (_codeScanMode == CodeScanMode.SquareInterpolation)
                    {
                        ExitState();
                        return;
                    }
                    ConsumeToken(JadeiteKind.CloseSquareBracket, 1);
                    return;
                case '@':
                    if (NextChar() == '"')
                        ScanVerbatimString();
                    else 
                        ScanCodeIdentifier();
                    return;
                case '$':
                    if (NextChar() == '"')
                        ScanInterpolatedString();
                    else
                        ScanCodeIdentifier();
                    return;
                case '\'':
                    ScanCharacterLiteral();
                    return;
                case '"':
                    ScanStringLiteral();
                    return;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ScanNumericLiteral();
                    return;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    // lowercase letters might be a keyword
                    var tok = ConsumePossibleCodeKeyword();
                    if (tok == null)
                    {
                        // wasn't a keyword - must be an identifier
                        ScanCodeIdentifier();
                    }
                    return;
                case '_':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    // uppercase or underscore means it's definitely an identifier
                    ScanCodeIdentifier();
                    return;
                default:
                    throw new Exception($"Invalid character Line {Line} Column {Column}.");
            }
        }

        private void ScanCodeIdentifier()
        {
            var c = CurrentChar();
            if (c == '@')
            {
                ConsumeTrivia(1);
                c = CurrentChar();
            }

            if (!IsCodeIdentifierCharacter(c) || IsDecimalDigit(c))
            {
                throw new Exception($"Expected an identifier at Line {Line} Column {Column}.");
            }

            var len = 1;
            for (var i = Index + 1; i < Length; i++)
            {
                if (IsCodeIdentifierCharacter(Input[i]))
                    len++;
                else
                    break;
            }

            ConsumeToken(JadeiteKind.CodeIdentifier, len, useTextAsValue: true);
        }
    }
}
