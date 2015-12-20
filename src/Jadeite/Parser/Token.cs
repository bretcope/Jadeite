
using System;
using Jadeite.Parser.Nodes;

namespace Jadeite.Parser
{
    public enum TokenType
    {
        Invalid = 0,
        EndOfInput,

        // significant white space
        Indent,
        Outdent,
        EndOfLine,

        // punctuation
        Hash,
        Dot,
        Pipe,
        PipePipe,
        And,
        AndAnd,
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
        Equals,
        EqualsEquals,
        LessThan,
        LessThanLessThan,
        LessThanEquals,
        GreaterThan,
        GreaterThanGreaterThan,
        GreaterThanEquals,
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

        // keywords
        Extends,
        Prepend,
        Append,
        Block,
        Include,
        Mixin,
        Each,
        If,
        Else,
        Switch,
        Case,
        AndAttributes, // &attributes
        Model,
        Doctype,

        // templating elements
        BufferedHtmlComment,
        UnbufferedHtmlComment,
        HtmlIdentifier,
        HtmlText,

        // code elements
        CodeComment,
        CodeIdentifier,
        IntegerLiteral,
        FloatingPointLiteral,
        StringLiteral,
    }

    internal static class Keyword
    {
        public const string EXTENDS = "extends";
        public const string PREPEND = "prepend";
        public const string APPEND = "append";
        public const string BLOCK = "block";
        public const string INCLUDE = "include";
        public const string MIXIN = "mixin";
        public const string EACH = "each";
        public const string IF = "if";
        public const string ELSE = "else";
        public const string SWITCH = "switch";
        public const string CASE = "case";
        public const string ANDATTRIBUTES = "&attributes";
        public const string MODEL = "model";
        public const string DOCTYPE = "doctype";

        public static string GetString(TokenType type)
        {
            switch (type)
            {
                case TokenType.Extends:
                    return EXTENDS;
                case TokenType.Prepend:
                    return PREPEND;
                case TokenType.Append:
                    return APPEND;
                case TokenType.Block:
                    return BLOCK;
                case TokenType.Include:
                    return INCLUDE;
                case TokenType.Mixin:
                    return MIXIN;
                case TokenType.Each:
                    return EACH;
                case TokenType.If:
                    return IF;
                case TokenType.Else:
                    return ELSE;
                case TokenType.Switch:
                    return SWITCH;
                case TokenType.Case:
                    return CASE;
                case TokenType.AndAttributes:
                    return ANDATTRIBUTES;
                case TokenType.Model:
                    return MODEL;
                case TokenType.Doctype:
                    return DOCTYPE;
                default:
                    throw new Exception($"Keyword.GetString() called with token type {type}. Only keyword token types are allowed."); // todo
            }
        }
    }

    public class Token : ISyntaxElement
    {
        public TokenType Type { get; internal set; }
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