using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jadeite.Parsing
{
    public abstract partial class LexerBase
    {
        protected const char INVALID_CHAR = char.MaxValue;

        private string _trivia = "";

        private readonly LookAheadQueue<Token> _tokenQueue = new LookAheadQueue<Token>();

        protected int Length { get; }
        protected int Index { get; private set; }
        protected int Line { get; private set; }
        protected int Column { get; private set; }

        public string Input { get; }

        protected LexerBase(string input)
        {
            Input = input;
            Length = input.Length;

            Index = 0;
            Line = 1;
            Column = 1;
        }

        protected abstract void Lex();

        public Token Advance()
        {
            QueueTokens(1);
            return _tokenQueue.Dequeue();
        }

        public Token AdvanceKind(JadeiteSyntaxKind kind)
        {
            var tok = Advance();
            if (tok.Kind != kind)
                throw new Exception($"Expected {kind} at Line {Line} Column {Column}."); // todo

            return tok;
        }

        public Token Current()
        {
            QueueTokens(1);
            return _tokenQueue.Current();
        }

        public Token LookAhead()
        {
            QueueTokens(2);
            return _tokenQueue.LookAhead();
        }

        private void QueueTokens(int needed)
        {
            while (_tokenQueue.Count < needed)
                Lex();
        }

        protected Token ConsumeToken(JadeiteSyntaxKind type, int length, object value = null, bool useTextAsValue = false)
        {
            var text = Input.Substring(Index, length);
            var tok = new Token
            {
                Kind = type,
                Text = text,
                Value = useTextAsValue ? text : value,
                LeadingTrivia = PopTrivia(),
                Position = new Position
                {
                    Index = Index,
                    Line = Line,
                    Column = Column,
                    Length = length
                }
            };

            UpdatePosition(Index + length);

            _tokenQueue.Push(tok);
            return tok;
        }

        protected void ConsumeTrivia(int length)
        {
            if (length > 0)
            {
                var sub = Input.Substring(Index, length);
                if (_trivia.Length == 0)
                    _trivia = sub;
                else
                    _trivia += sub;

                UpdatePosition(Index + length);
            }
        }

        protected string PopTrivia()
        {
            if (_trivia.Length == 0)
                return "";

            var t = _trivia;
            _trivia = "";
            return t;
        }

        protected int ConsumeWhiteSpaceAsTrivia()
        {
            var wsLen = 0;
            for (var i = Index; i < Length; i++)
            {
                if (IsWhiteSpace(Input[i]))
                    wsLen++;
                else 
                    break;
            }

            if (wsLen > 0)
                ConsumeTrivia(wsLen);

            return wsLen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected char CharAt(int index)
        {
            return index < Length ? Input[index] : INVALID_CHAR;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected char CurrentChar()
        {
            return CharAt(Index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected char NextChar()
        {
            return CharAt(Index + 1);
        }

        private void UpdatePosition(int newIndex)
        {
            for (var i = Index; i < newIndex; i++)
            {
                switch (CharAt(i))
                {
                    case '\r':
                        if (CharAt(i + 1) != '\n')
                        {
                            Line++;
                            Column = 1;
                        }
                        break;
                    case '\n':
                        Line++;
                        Column = 1;
                        break;
                    default:
                        Column++;
                        break;
                }
            }

            Index = newIndex;
        }
    }
}