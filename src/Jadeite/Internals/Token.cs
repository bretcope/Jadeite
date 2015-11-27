
namespace Jadeite.Internals
{
    public enum TokenType
    {
        Invalid = 0,
        EndOfInput,

        // significant white space
        IndentDefinition,
        Indent,
        Outdent,
        NewLine,
        BlankLine,
        Space,

        // symbols / operators
        Dot,
        Pipe,
        OpenParen,
        CloseParen,
        OpenEscapedInterpolation,    // #{
        OpenNonEscapedInterpolation, // !{
        CloseCurly,
        OpenTagInterpolation, // #[
        CloseSquareBracket,
        Equals,
        EqualsEquals,
        NotEquals, // !=
        Comma,
        Plus,
        LogicalOr,  // ||
        LogicalAnd, // &&
        LogicalNot, // !
        QuestionMark,
        Colon,

        // keywords
        Extends,
        Prepend,
        Append,
        Block,
        Mixin,
        Each,
        If,
        Else,
        Switch,
        Case,
        AndAttributes, // &attributes
        Model,

        // elements
        LineComment,
        BlockComment,
        DocType,
        TagName,
        ClassName,
        Id,
        Text,
        NumberLiteral,
        Identifier,
    }

    internal static class Keyword
    {
        public const string EXTENDS = "extends";
        public const string PREPEND = "prepend";
        public const string APPEND = "append";
        public const string BLOCK = "block";
        public const string MIXIN = "mixin";
        public const string EACH = "each";
        public const string IF = "if";
        public const string ELSE = "else";
        public const string SWITCH = "switch";
        public const string CASE = "case";
        public const string ANDATTRIBUTES = "&attributes";
        public const string MODEL = "model";
    }

    public class Token
    {
        public TokenType Type { get; internal set; }
        public string RawValue { get; internal set; }
        public string UsefulValue { get; internal set; }
        public Position Position { get; internal set; }
        public string LeadingTrivia { get; internal set; }
        public string TrailingTrivia { get; internal set; }

        internal Token()
        {
        }
    }
}