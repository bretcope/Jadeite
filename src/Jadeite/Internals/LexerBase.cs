﻿using System;
using System.Collections.Generic;

namespace Jadeite.Internals
{
    public abstract class LexerBase
    {
        protected enum LexerContext
        {
            Header,
            Document,
            TagInterpolation,
            PlainTextBlock,
        }

        internal const char INVALID_CHAR = char.MaxValue;

        protected int _index = 0;
        protected int _line = 1;
        protected int _column = 1;
        protected int _length;
        protected bool _indentDiscovered;
        protected string _indentString;
        protected int _indentLevel;
        protected string _leadingTrivia = "";

        private Queue<Token> _tokenQueue = new Queue<Token>();
        private Stack<LexerContext> _contextStack = new Stack<LexerContext>();

        public string Input { get; }

        protected LexerContext Context => _contextStack.Peek();

        protected LexerBase(string input)
        {
            _length = input.Length;
            Input = input;

            EnterContext(LexerContext.Document);
            EnterContext(LexerContext.Header);
        }

        protected abstract void Lex();

        public Token Advance()
        {
            if (_tokenQueue.Count == 0)
                QueueNext();

            return _tokenQueue.Dequeue();
        }

        public Token Peek()
        {
            if (_tokenQueue.Count == 0)
                QueueNext();

            return _tokenQueue.Peek();
        }

        private void QueueNext()
        {
            while (_tokenQueue.Count == 0)
            {
                Lex();
            }
        }

        protected void EnterContext(LexerContext context)
        {
            _contextStack.Push(context);
        }

        protected void ExitContext()
        {
            if (_contextStack.Count == 1)
                throw new Exception("The lexer attempted to exit the last context."); // todo

            _contextStack.Pop();
        }

        protected char CurrentCharacter()
        {
            return _index < _length ? Input[_index] : INVALID_CHAR;
        }

        protected char PeekCharacter()
        {
            var i = _index + 1;
            return i < _length ? Input[i] : INVALID_CHAR;
        }

        protected void ConsumeToken(TokenType type)
        {
            if (GetToken(type) == null)
                throw new Exception($"Expected token type {type}"); // todo
        }

        protected bool ConsumeOptionalToken(TokenType type)
        {
            return GetToken(type) != null;
        }

        // a quick way to get case-insensitive comparison (prefix must be lowercase)
        protected bool IsNextText(string lowerCasePrefix, bool requireTrailingWordBoundary = false)
        {
            if (lowerCasePrefix.Length > _length - _index)
                return false;

            var ii = _index;
            for (var pi = 0; pi < lowerCasePrefix.Length; pi++, ii++)
            {
                var inChar = Input[ii];
                var pChar = lowerCasePrefix[pi];

                if (inChar == pChar)
                    continue;

                if ((inChar | 32) == pChar && inChar >= 65 && inChar <= 90)
                    continue;

                return false;
            }

            if (requireTrailingWordBoundary)
            {
                if (ii == _length)
                    return true;

                if (IsWordCharacter(Input[ii]))
                    return false;
            }

            return true;
        }

        protected void ConsumeLeadingTrivia()
        {
            var end = _index;
            while (end < _length)
            {
                var c = Input[end];
                if (c == ' ' || c == '\t')
                    end++;
                else
                    break;
            }

            if (end > _index)
            {
                _leadingTrivia += Input.Substring(_index, end - _index);
                _index = end;
            }
        }

        // yes, this could be done with a regex, but the switch is an order of magnitude faster
        protected static bool IsWordCharacter(char c)
        {
            switch (c)
            {
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
                case '_':
                    return true;
                default:
                    return false;
            }
        }

        private Token GetToken(TokenType type)
        {
            switch (type)
            {
//                case TokenType.Invalid:
//                case TokenType.EndOfInput:
//                case TokenType.Indent:
//                case TokenType.Outdent:
//                case TokenType.NewLine:
//                    break;
//                case TokenType.Space:
//                    return GetExactTextToken(type, ' ');
//                case TokenType.Dot:
//                    return GetExactTextToken(type, '.');
//                case TokenType.Pipe:
//                    return GetExactTextToken(type, '|');
//                case TokenType.OpenParen:
//                    return GetExactTextToken(type, '(');
//                case TokenType.CloseParen:
//                    return GetExactTextToken(type, ')');
//                case TokenType.OpenEscapedInterpolation:
//                    return GetExactTextToken(type, "#{");
//                case TokenType.OpenNonEscapedInterpolation:
//                    return GetExactTextToken(type, "!{");
//                case TokenType.CloseCurly:
//                    return GetExactTextToken(type, '}');
//                case TokenType.OpenTagInterpolation:
//                    return GetExactTextToken(type, "#[");
//                case TokenType.CloseSquareBracket:
//                    return GetExactTextToken(type, ']');
//                case TokenType.Equals:
//                    return GetExactTextToken(type, '=');
//                case TokenType.EqualsEquals:
//                    return GetExactTextToken(type, "==");
//                case TokenType.NotEquals:
//                    return GetExactTextToken(type, "!=");
//                case TokenType.Comma:
//                    return GetExactTextToken(type, ',');
//                case TokenType.Plus:
//                    return GetExactTextToken(type, '+');
//                case TokenType.LogicalOr:
//                    return GetExactTextToken(type, "||");
//                case TokenType.LogicalAnd:
//                    return GetExactTextToken(type, "&&");
//                case TokenType.LogicalNot:
//                    return GetExactTextToken(type, '!');
//                case TokenType.QuestionMark:
//                    return GetExactTextToken(type, '?');
//                case TokenType.Colon:
//                    return GetExactTextToken(type, ':');
//                case TokenType.Extends:
//                    return GetExactTextToken(type, Keyword.EXTENDS);
                case TokenType.Prepend:
                    return GetExactTextToken(type, Keyword.PREPEND);
                case TokenType.Append:
                    return GetExactTextToken(type, Keyword.APPEND);
                case TokenType.Block:
                    return GetExactTextToken(type, Keyword.BLOCK);
                case TokenType.Mixin:
                    return GetExactTextToken(type, Keyword.MIXIN);
                case TokenType.Each:
                    return GetExactTextToken(type, Keyword.EACH);
                case TokenType.If:
                    return GetExactTextToken(type, Keyword.IF);
                case TokenType.Else:
                    return GetExactTextToken(type, Keyword.ELSE);
                case TokenType.Switch:
                    return GetExactTextToken(type, Keyword.SWITCH);
                case TokenType.Case:
                    return GetExactTextToken(type, Keyword.CASE);
                case TokenType.AndAttributes:
                    return GetExactTextToken(type, Keyword.ANDATTRIBUTES);
                case TokenType.Model:
                    return GetExactTextToken(type, Keyword.MODEL);
                case TokenType.LineComment:
                    return GetLineComment();
                case TokenType.BlockComment:
                case TokenType.DocType:
                case TokenType.TagName:
                case TokenType.ClassName:
                case TokenType.Id:
                case TokenType.Text:
                case TokenType.NumberLiteral:
                case TokenType.Identifier:
                    break;
            }

            throw new Exception($"Huh? {type} token..."); // todo
        }

        private Token GetExactTextToken(TokenType type, char literal)
        {
            return CurrentCharacter() == literal ? CreateToken(type, 1) : null;
        }

        private Token GetExactTextToken(TokenType type, string literal)
        {
            return IsNextText(literal) ? CreateToken(type, literal.Length, literal) : null;
        }
        
        private Token GetLineComment()
        {
            if (!IsNextText("//"))
                return null;

            var start = _index + 2;
            var length = 0;
            for (var i = start; i < _length; i++)
            {
                var c = Input[i];
                if (c == '\n' || c == '\r')
                    break;

                length++;
            }

            var useful = Input.Substring(start, length);
            return CreateToken(TokenType.LineComment, length + 2, useful);
        }

        private Token CreateToken(TokenType type, int length, string usefulValue = null)
        {
            var raw = Input.Substring(_index, length);
            var tok = new Token
            {
                Type = type,
                RawValue = raw,
                UsefulValue = usefulValue ?? raw,
                Position = new Position
                {
                    Index = _index,
                    Line = _line,
                    Column = _column,
                },
                LeadingTrivia = _leadingTrivia
            };

            _tokenQueue.Enqueue(tok);

            _leadingTrivia = "";

            // figure out the new line and column numbers
            // this does mean we rescan the text, but it's probably the most reliable way
            for (var i = 0; i < length; i++)
            {
                var c = raw[i];
                if (c == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else if (c == '\r')
                {
                    if (c + 1 < raw.Length && raw[c + 1] == '\n')
                        i++;

                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
            }

            _index += length;

            return tok;
        }
    }
}
