using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing
{
    public sealed partial class Lexer : LexerBase
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

        public Lexer(string input, string indent) : base(input)
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
                    TryConsumeKeyword(JadeiteKind.AppendKeyword, out tok);
                    break;
                case 'b':
                    TryConsumeKeyword(JadeiteKind.BlockKeyword, out tok);
                    break;
                case 'd':
                    TryConsumeKeyword(JadeiteKind.DoctypeKeyword, out tok);
                    break;
                case 'e':
                    TryConsumeKeyword(JadeiteKind.ExtendsKeyword, out tok);
                    break;
                case 'i':
                    TryConsumeKeyword(JadeiteKind.IncludeKeyword, out tok);
                    break;
                case 'm':
                    TryConsumeKeyword(JadeiteKind.MixinKeyword, out tok);
                    break;
                case 'p':
                    TryConsumeKeyword(JadeiteKind.PrependKeyword, out tok);
                    break;
            }

            return tok;
        }

        private Token ConsumePossibleCodeKeyword()
        {
            Token tok = null;

            switch (CurrentChar())
            {
                case 'a':
                    TryConsumeKeyword(JadeiteKind.AttributesKeyword, out tok);
                    break;
                case 'b':
                    if (TryConsumeKeyword(JadeiteKind.BreakKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.BoolKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.ByteKeyword, out tok);
                    break;
                case 'c':
                    if (TryConsumeKeyword(JadeiteKind.CaseKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ConstKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ContinueKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.CharKeyword, out tok);
                    break;
                case 'd':
                    if (TryConsumeKeyword(JadeiteKind.DefaultKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.DoubleKeyword, out tok);
                    break;
                case 'e':
                    TryConsumeKeyword(JadeiteKind.ElseKeyword, out tok);
                    break;
                case 'f':
                    if (TryConsumeKeyword(JadeiteKind.FalseKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ForKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ForeachKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.FuncKeyword, out tok);
                    break;
                case 'i':
                    if (TryConsumeKeyword(JadeiteKind.IfKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.InKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.IntKeyword, out tok);
                    break;
                case 'l':
                    TryConsumeKeyword(JadeiteKind.LongKeyword, out tok);
                    break;
                case 'm':
                    TryConsumeKeyword(JadeiteKind.ModelKeyword, out tok);
                    break;
                case 'n':
                    TryConsumeKeyword(JadeiteKind.NullKeyword, out tok);
                    break;
                case 'r':
                    TryConsumeKeyword(JadeiteKind.ReturnKeyword, out tok);
                    break;
                case 's':
                    if (TryConsumeKeyword(JadeiteKind.SwitchKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.SByteKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ShortKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.StringKeyword, out tok);
                    break;
                case 't':
                    TryConsumeKeyword(JadeiteKind.TrueKeyword, out tok);
                    break;
                case 'u':
                    if (TryConsumeKeyword(JadeiteKind.UIntKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteKind.ULongKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteKind.UShortKeyword, out tok);
                    break;
                case 'v':
                    TryConsumeKeyword(JadeiteKind.VarKeyword, out tok);
                    break;
                case 'w':
                    TryConsumeKeyword(JadeiteKind.WhileKeyword, out tok);
                    break;
            }

            return tok;
        }

        private bool TryConsumeKeyword(JadeiteKind type)
        {
            Token tok;
            return TryConsumeKeyword(type, out tok);
        }

        private bool TryConsumeKeyword(JadeiteKind type, out Token tok)
        {
            var keyword = SyntaxInfo.GetKeywordString(type);
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