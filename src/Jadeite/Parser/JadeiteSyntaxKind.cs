
namespace Jadeite.Parser
{
    public enum JadeiteSyntaxKind
    {
        // ============= TERMINALS =================================================

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
        BlockKeyword,
        DoctypeKeyword,
        ExtendsKeyword,
        IncludeKeyword,
        MixinKeyword,
        PrependKeyword,

        [StartOf(SyntaxCategory.CodeKeyword)]
        AttributesKeyword,
        BreakKeyword,
        CaseKeyword,
        ConstKeyword,
        ContinueKeyword,
        DefaultKeyword,
        ElseKeyword,
        FalseKeyword,
        ForKeyword,
        ForeachKeyword,
        FuncKeyword,
        IfKeyword,
        InKeyword,
        ModelKeyword,
        NullKeyword,
        ReturnKeyword,
        SwitchKeyword,
        TrueKeyword,
        VarKeyword,
        WhileKeyword,

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

        // ============= NON-TERMINALS =============================================

        [StartOf(SyntaxCategory.InvalidNode)]
        InvalidNode,
    }
}
