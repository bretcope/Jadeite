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
                    if (TryConsumeKeyword(JadeiteSyntaxKind.AppendKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.AttributesKeyword);
                    break;
                case 'b':
                    TryConsumeKeyword(JadeiteSyntaxKind.BlockKeyword, out tok);
                    break;
                case 'd':
                    TryConsumeKeyword(JadeiteSyntaxKind.DoctypeKeyword, out tok);
                    break;
                case 'e':
                    TryConsumeKeyword(JadeiteSyntaxKind.ExtendsKeyword, out tok);
                    break;
                case 'i':
                    TryConsumeKeyword(JadeiteSyntaxKind.IncludeKeyword, out tok);
                    break;
                case 'm':
                    TryConsumeKeyword(JadeiteSyntaxKind.MixinKeyword, out tok);
                    break;
                case 'p':
                    TryConsumeKeyword(JadeiteSyntaxKind.PrependKeyword, out tok);
                    break;
            }

            return tok;
        }

        private Token ConsumePossibleCodeKeyword()
        {
            Token tok = null;

            switch (CurrentChar())
            {
                case 'b':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.BreakKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.BoolKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.ByteKeyword, out tok);
                    break;
                case 'c':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.CaseKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.ConstKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.ContinueKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.CharKeyword, out tok);
                    break;
                case 'd':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.DefaultKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.DoubleKeyword, out tok);
                    break;
                case 'e':
                    TryConsumeKeyword(JadeiteSyntaxKind.ElseKeyword, out tok);
                    break;
                case 'f':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.FalseKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.FuncKeyword, out tok);
                    break;
                case 'i':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.IfKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.InKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.IntKeyword, out tok);
                    break;
                case 'l':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.LoopKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.LongKeyword, out tok);
                    break;
                case 'm':
                    TryConsumeKeyword(JadeiteSyntaxKind.ModelKeyword, out tok);
                    break;
                case 'n':
                    TryConsumeKeyword(JadeiteSyntaxKind.NullKeyword, out tok);
                    break;
                case 'r':
                    TryConsumeKeyword(JadeiteSyntaxKind.ReturnKeyword, out tok);
                    break;
                case 's':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.SwitchKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.SByteKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.ShortKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.StringKeyword, out tok);
                    break;
                case 't':
                    TryConsumeKeyword(JadeiteSyntaxKind.TrueKeyword, out tok);
                    break;
                case 'u':
                    if (TryConsumeKeyword(JadeiteSyntaxKind.UIntKeyword, out tok))
                        break;
                    if (TryConsumeKeyword(JadeiteSyntaxKind.ULongKeyword, out tok))
                        break;
                    TryConsumeKeyword(JadeiteSyntaxKind.UShortKeyword, out tok);
                    break;
                case 'v':
                    TryConsumeKeyword(JadeiteSyntaxKind.VarKeyword, out tok);
                    break;
            }

            return tok;
        }

        private bool TryConsumeKeyword(JadeiteSyntaxKind type)
        {
            Token tok;
            return TryConsumeKeyword(type, out tok);
        }

        private bool TryConsumeKeyword(JadeiteSyntaxKind type, out Token tok)
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