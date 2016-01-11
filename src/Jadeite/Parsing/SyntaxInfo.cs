using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Jadeite.Parsing
{
    public enum SyntaxCategory
    {
        InvalidToken,
        Whitespace,
        Punctuation,
        TemplateKeyword,
        CodeKeyword,
        TypeKeyword,
        TemplateLiteral,
        CodeLiteral,
        InvalidNode,
        TemplateNode,
        CodeExpression,
        CodeStatement
    }

    internal class StartOfAttribute : Attribute
    {
        internal SyntaxCategory Category { get; }

        internal StartOfAttribute(SyntaxCategory category)
        {
            Category = category;
        }
    }

    public static class SyntaxInfo
    {
        private struct Range
        {
            public int Start;
            public int End;
        }

        private static readonly Range[] _ranges;
        private static readonly string[] _keywords;
        private static readonly int _keywordOffset;

        static SyntaxInfo()
        {
            var categories = (SyntaxCategory[])Enum.GetValues(typeof(SyntaxCategory));

            _ranges = InitCategoryRanges(categories);
            _keywords = InitKeywords(categories, out _keywordOffset);
        }

        private static Range[] InitCategoryRanges(SyntaxCategory[] categories)
        {
            var ranges = new Range[categories.Length];

            // figure out category ranges
            var cat = (int)SyntaxCategory.InvalidToken;
            var type = typeof(JadeiteKind);
            foreach (var val in Enum.GetValues(type))
            {
                var ival = (int)val;
                var attr = type.GetMember(val.ToString())[0].GetCustomAttribute<StartOfAttribute>();
                if (attr != null)
                {
                    cat = (int)attr.Category;
                    ranges[cat].Start = ival;
                    ranges[cat].End = ival + 1;
                }
                else
                {
                    ranges[cat].End++;
                }
            }

            return ranges;
        }

        private static string[] InitKeywords(SyntaxCategory[] categories, out int offset)
        {
            var start = -1;
            var end = -1;

            foreach (var c in categories)
            {
                if (c.ToString().EndsWith("Keyword"))
                {
                    var r = _ranges[(int)c];

                    if (start == -1 || start > r.Start)
                        start = r.Start;

                    if (end == -1 || end < r.End)
                        end = r.End;
                }
            }

            Debug.Assert(start != -1);
            Debug.Assert(end != -1);

            offset = start;
            var length = end - start;
            var keywords = new string[length];

            for (var i = 0; i < length; i++)
            {
                var kind = (JadeiteKind)(i + offset);
                var str = kind.ToString();

                Debug.Assert(IsTokenKind(kind));
                Debug.Assert(str.EndsWith("Keyword"));

                keywords[i] = str.Substring(0, str.Length - "Keyword".Length).ToLowerInvariant();
            }

            return keywords;
        }

        public static bool IsOfCategory(this JadeiteKind kind, SyntaxCategory category)
        {
            var r = _ranges[(int)category];
            var ival = (int)kind;
            return ival >= r.Start && ival < r.End;
        }

        public static SyntaxCategory GetCategory(this JadeiteKind kind)
        {
            for (var i = 0; i < _ranges.Length; i++)
            {
                var cat = (SyntaxCategory)i;
                if (IsOfCategory(kind, cat))
                    return cat;
            }

            throw new Exception($"Unknown JadeiteKind {kind}. It does not appear to be in a category range.");
        }

        public static bool IsTokenKind(this JadeiteKind kind)
        {
            return kind > JadeiteKind.InvalidToken && kind < JadeiteKind.InvalidNode;
        }

        public static bool IsNodeKind(this JadeiteKind kind)
        {
            return kind > JadeiteKind.InvalidNode;
        }

        public static bool IsAssignmentOperator(this JadeiteKind kind)
        {
            switch (kind)
            {
                case JadeiteKind.Equals:
                case JadeiteKind.PlusEquals:
                case JadeiteKind.MinusEquals:
                case JadeiteKind.AsteriskEquals:
                case JadeiteKind.ForwardSlashEquals:
                case JadeiteKind.PercentEquals:
                case JadeiteKind.AndEquals:
                case JadeiteKind.PipeEquals:
                case JadeiteKind.CaretEquals:
                case JadeiteKind.LessThanLessThanEquals:
                case JadeiteKind.GreaterThanGreaterThanEquals:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumericTypeKeyword(this JadeiteKind kind)
        {
            switch (kind)
            {
                case JadeiteKind.SByteKeyword:
                case JadeiteKind.ByteKeyword:
                case JadeiteKind.ShortKeyword:
                case JadeiteKind.UShortKeyword:
                case JadeiteKind.IntKeyword:
                case JadeiteKind.UIntKeyword:
                case JadeiteKind.LongKeyword:
                case JadeiteKind.ULongKeyword:
                case JadeiteKind.CharKeyword:
                case JadeiteKind.DoubleKeyword:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf(this JadeiteKind kind, JadeiteKind kind1)
        {
            return kind == kind1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf(this JadeiteKind kind, JadeiteKind kind1, JadeiteKind kind2)
        {
            return kind == kind1 || kind == kind2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf(this JadeiteKind kind, JadeiteKind kind1, JadeiteKind kind2, JadeiteKind kind3)
        {
            return kind == kind1 || kind == kind2 || kind == kind3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf(this JadeiteKind kind, JadeiteKind kind1, JadeiteKind kind2, JadeiteKind kind3, JadeiteKind kind4)
        {
            return kind == kind1 || kind == kind2 || kind == kind3 || kind == kind4;
        }

        internal static string GetKeywordString(this JadeiteKind kind)
        {
            return _keywords[(int)kind - _keywordOffset];
        }
    }
}