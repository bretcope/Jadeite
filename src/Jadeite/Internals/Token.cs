
using System;

namespace Jadeite.Internals
{
    public enum TokenType
    {
        Invalid = 0,
        EndOfInput,

        // significant white space
        Indent,
        NewLine,

        // symbols / operators
        Dot,
        Pipe,
        Hash,
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
        Minus,
        PlusPlus,
        MinusMinus,
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
        HtmlIdentifier,
        CodeIdentifier,
        NumberLiteral,
        StringLiteral,
        BodyText,
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
                default:
                    throw new Exception($"Keyword.GetString() called with token type {type}. Only keyword token types are allowed."); // todo
            }
        }
    }

    public class Token
    {
        public TokenType Type { get; internal set; }
        public string RawValue { get; internal set; }
        public string Value { get; internal set; }
        public Position Position { get; internal set; }
        public string LeadingTrivia { get; internal set; }
        public string TrailingTrivia { get; internal set; }

        internal Token()
        {
        }
    }
}