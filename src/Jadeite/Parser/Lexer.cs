using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parser
{
    public partial class Lexer : LexerBase
    {
        private enum LexerState : byte
        {
            Document,
            Indent,
            Tag,
            PipelessText,
            Body,
            Code,
            Attributes,
            AndAttributes,
            HtmlComment,
            NamedBlock,
        }

        private readonly Stack<LexerState> _stateStack = new Stack<LexerState>();

        private LexerState State => _stateStack.Peek();

        public string IndentString { get; }

        public Lexer(string input, string indent = "\t") : base(input)
        {
            SetIndent(indent);
            IndentString = indent;

            TransitionToDocument();
        }

        protected override void Lex()
        {
            switch (State)
            {
                case LexerState.Document:
                    ScanDocument();
                    break;
                case LexerState.Indent:
                    ScanIndent();
                    break;
                case LexerState.Tag:
                    ScanTag();
                    break;
                case LexerState.PipelessText:
                    ScanPipelessText();
                    break;
                case LexerState.Body:
                    ScanBody();
                    break;
                case LexerState.Code:
                    ScanCode();
                    break;
                case LexerState.Attributes:
                    ScanAttributes();
                    break;
                case LexerState.AndAttributes:
                    ScanAndAttributes();
                    break;
                case LexerState.HtmlComment:
                    ScanHtmlComment();
                    break;
                case LexerState.NamedBlock:
                    ScanNamedBlock();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void PushState(LexerState state)
        {
            Debug.Assert(_stateStack.Count == 0 || _stateStack.Peek() != LexerState.Indent, "Cannot nest from indent state.");
            _stateStack.Push(state);
        }

        private void ExitState()
        {
            var oldState = _stateStack.Pop();
            switch (oldState)
            {
                case LexerState.Body:
                    OnExitBody();
                    break;
            }
        }

        private Token ConsumePossibleJadeiteKeyword()
        {
            Token tok = null;

            switch (CurrentChar())
            {
                case 'a':
                    TryConsumeKeyword(TokenType.Append, out tok);
                    break;
                case 'b':
                    TryConsumeKeyword(TokenType.Block, out tok);
                    break;
                case 'd':
                    TryConsumeKeyword(TokenType.Doctype, out tok);
                    break;
                case 'e':
                    TryConsumeKeyword(TokenType.Extends, out tok);
                    break;
                case 'i':
                    TryConsumeKeyword(TokenType.Include, out tok);
                    break;
                case 'm':
                    if (TryConsumeKeyword(TokenType.Mixin, out tok)
                        || TryConsumeKeyword(TokenType.Model, out tok))
                    {
                    }
                    break;
                case 'p':
                    TryConsumeKeyword(TokenType.Prepend, out tok);
                    break;
            }

            return tok;
        }

        private Token ConsumePossibleCodeKeyword()
        {
            Token tok = null;

            switch (CurrentChar())
            {
                case 'c':
                    TryConsumeKeyword(TokenType.Case, out tok);
                    break;
                case 'e':
                    if (TryConsumeKeyword(TokenType.Each, out tok)
                        || TryConsumeKeyword(TokenType.Else, out tok))
                    {
                    }
                    break;
                case 'i':
                    TryConsumeKeyword(TokenType.If, out tok);
                    break;
                case 's':
                    TryConsumeKeyword(TokenType.Switch, out tok);
                    break;
            }

            return tok;
        }

        private bool TryConsumeKeyword(TokenType type)
        {
            Token tok;
            return TryConsumeKeyword(type, out tok);
        }

        private bool TryConsumeKeyword(TokenType type, out Token tok)
        {
            var keyword = Keyword.GetString(type);
            var kLen = keyword.Length;
            if (kLen > Length - Index)
            {
                tok = null;
                return false;
            }

            var ii = Index;
            for (var ki = 0; ki < kLen; ki++, ii++)
            {
                if (keyword[ki] != Input[ii])
                {
                    tok = null;
                    return false;
                }
            }

            // also need to check that the following character constitutes a word boundary
            if (IsWordCharacter(CharAt(ii)))
            {
                tok = null;
                return false;
            }

            tok = ConsumeToken(type, kLen);

            // consume any white space after the keyword as trivia
            if (ConsumeWhiteSpaceAsTrivia() > 0)
                tok.TrailingTrivia = PopTrivia();

            return true;
        }
    }
}