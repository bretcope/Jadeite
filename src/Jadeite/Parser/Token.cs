
using System;
using Jadeite.Parser.Nodes;

namespace Jadeite.Parser
{
    public enum JadeiteSyntaxKind
    {
        [StartOf(SyntaxCategory.InvalidToken)]
        InvalidToken = 0,

        [StartOf(SyntaxCategory.Whitespace)]
        EndOfInput,
        Indent,
        Outdent,
        EndOfLine,

        [StartOf(SyntaxCategory.Punctuation)]
        Hash,
        Dot,
        Pipe,
        PipePipe,
        PipeEquals,
        And,
        AndAnd,
        AndEquals,
        Caret,
        CaretEquals,
        Plus,
        PlusPlus,
        PlusEquals,
        Minus,
        MinusMinus,
        MinusEquals,
        Asterisk,
        AsteriskEquals,
        ForwardSlash,
        ForwardSlashEquals,
        Percent,
        PercentEquals,
        Equals,
        EqualsEquals,
        LessThan,
        LessThanLessThan,
        LessThanEquals,
        LessThanLessThanEquals,
        GreaterThan,
        GreaterThanGreaterThan,
        GreaterThanEquals,
        GreaterThanGreaterThanEquals,
        Comma,
        SemiColon,
        Bang,
        BangEquals,
        QuestionMark,
        Colon,
        OpenParen,
        CloseParen,
        OpenEscapedInterpolation,    // #{
        OpenNonEscapedInterpolation, // !{
        OpenCurly,
        CloseCurly,
        OpenTagInterpolation, // #[
        OpenSquareBracket,
        CloseSquareBracket,

        // ALL KEYWORD SECTIONS MUST BE CONSECUTIVE

        [StartOf(SyntaxCategory.TemplateKeyword)]
        AppendKeyword,
        AttributesKeyword,
        BlockKeyword,
        DoctypeKeyword,
        ExtendsKeyword,
        IncludeKeyword,
        MixinKeyword,
        PrependKeyword,

        [StartOf(SyntaxCategory.CodeKeyword)]
        BreakKeyword,
        CaseKeyword,
        ConstKeyword,
        ContinueKeyword,
        DefaultKeyword,
        ElseKeyword,
        FalseKeyword,
        FuncKeyword,
        IfKeyword,
        InKeyword,
        LoopKeyword,
        ModelKeyword,
        NullKeyword,
        ReturnKeyword,
        SwitchKeyword,
        TrueKeyword,
        VarKeyword,

        [StartOf(SyntaxCategory.TypeKeyword)]
        BoolKeyword,
        ByteKeyword,
        CharKeyword,
        DoubleKeyword,
        IntKeyword,
        LongKeyword,
        SByteKeyword,
        ShortKeyword,
        StringKeyword,
        UIntKeyword,
        ULongKeyword,
        UShortKeyword,

        [StartOf(SyntaxCategory.TemplateLiteral)]
        BufferedHtmlComment,
        UnbufferedHtmlComment,
        HtmlIdentifier,
        HtmlText,

        [StartOf(SyntaxCategory.CodeLiteral)]
        CodeComment,
        CodeIdentifier,
        IntegerLiteral,
        FloatingPointLiteral,
        StringLiteral,

        [StartOf(SyntaxCategory.InvalidNode)]
        InvalidNode,
    }

    public class Token : ISyntaxElement
    {
        public JadeiteSyntaxKind Type { get; internal set; }
        public string Text { get; internal set; }
        public object Value { get; internal set; }
        public Position Position { get; internal set; }
        public string LeadingTrivia { get; internal set; }
        public string TrailingTrivia { get; internal set; }

        public bool IsToken => true;
        public bool IsNode => false;
        public bool IsHtmlNode => false;
        public bool IsCodeNode => false;

        internal Token()
        {
        }
    }
}