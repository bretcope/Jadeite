using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jadeite.Parser
{
    public abstract partial class LexerBase
    {
        protected const char INVALID_CHAR = char.MaxValue;

        private string _trivia = "";

        private Queue<Token> _tokenQueue = new Queue<Token>(); 

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
            QueueNext();
            return _tokenQueue.Dequeue();
        }

        public Token Peek()
        {
            QueueNext();
            return _tokenQueue.Peek();
        }

        private void QueueNext()
        {
            while (_tokenQueue.Count == 0)
                Lex();
        }

        protected Token ConsumeToken(TokenType type, int length, object value = null, bool useTextAsValue = false)
        {
            var text = Input.Substring(Index, length);
            var tok = new Token
            {
                Type = type,
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

            _tokenQueue.Enqueue(tok);
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