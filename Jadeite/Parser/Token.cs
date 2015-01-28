
using System;
using System.Collections.Generic;

namespace Jadeite.Parser
{
    public static class TokenTypes
    {
        public static readonly Type Call = typeof(CallToken);
        public static readonly Type Mixin = typeof(MixinToken);
        public static readonly Type Code = typeof(CodeToken);
        public static readonly Type Each = typeof(EachToken);
        public static readonly Type Attributes = typeof(AttributesToken);
        public static readonly Type AttributesBlock = typeof(AttributesBlockToken);
        public static readonly Type NewLine = typeof(NewLineToken);
        public static readonly Type Outdent = typeof(OutdentToken);
        public static readonly Type Indent = typeof(IndentToken);
        public static readonly Type PipelessText = typeof(PipelessTextToken);
        public static readonly Type EndOfStream = typeof(EndOfSourceToken);
        public static readonly Type Text = typeof(TextToken);
        public static readonly Type Comment = typeof(CommentToken);
        public static readonly Type Interpolation = typeof(InterpolationToken);
        public static readonly Type Tag = typeof(TagToken);
        public static readonly Type Block = typeof(BlockToken);
        public static readonly Type MixinBlock = typeof(MixinBlockToken);
        public static readonly Type Include = typeof(IncludeToken);
        public static readonly Type Colon = typeof(ColonToken);
        public static readonly Type Filter = typeof(FilterToken);
        public static readonly Type DocType = typeof(DocTypeToken);
        public static readonly Type Id = typeof(IdToken);
        public static readonly Type Class = typeof(ClassToken);
        public static readonly Type Dot = typeof(DotToken);
        public static readonly Type Extends = typeof(ExtendsToken);
        public static readonly Type Yield = typeof(YieldToken);
        public static readonly Type Case = typeof(CaseToken);
        public static readonly Type When = typeof(WhenToken);
        public static readonly Type Default = typeof(DefaultToken);
    }

    public class StringRange
    {
        private string _value;

        public string Original { get; }
        public string Value => _value ?? (_value = Original.Substring(Start, End - Start));
        public int Start { get; }
        public int End { get; }

        public StringRange(string original, int start, int end)
        {
            Original = original;
            Start = start;
            End = end;
        }
    }

    public abstract class Token
    {
        public int LineNumber { get; set; }
        public string Value { get; set; }
        public abstract string JadeEquivalentType { get; }
    }

    public class CallToken : Token
    {
        public override string JadeEquivalentType => "call";

        public string Arguments { get; set; }
    }

    public class MixinToken : Token
    {
        public override string JadeEquivalentType => "mixin";
        public string Arguments { get; set; }
    }

    public class CodeToken : Token
    {
        public override string JadeEquivalentType => "code";
        public bool RequiresBlock { get; set; }
        public bool IsIf { get; set; }
        public bool IsElse { get; set; }
        public bool Escape { get; set; }
        public bool Buffer { get; set; }
        public bool HasIf { get; set; }
    }

    public class EachToken : Token
    {
        public override string JadeEquivalentType => "each";
        public string Key { get; set; }
        public string Code { get; set; }
    }

    public class AttributesToken : Token
    {
        public override string JadeEquivalentType => "attrs";
        public List<AttributeItem> Attributes { get; set; } = new List<AttributeItem>();
        public bool SelfClosing { get; set; }
    }

    public class AttributeItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Escaped { get; set; }
    }

    public class AttributesBlockToken : Token
    {
        public override string JadeEquivalentType => "&attributes";
    }

    public class NewLineToken : Token
    {
        public override string JadeEquivalentType => "newline";
    }

    public class OutdentToken : Token
    {
        public override string JadeEquivalentType => "outdent";
    }

    public class IndentToken : Token
    {
        public override string JadeEquivalentType => "indent";
        public int Indents { get; set; }
    }

    public class PipelessTextToken : Token
    {
        public override string JadeEquivalentType => "pipeless-text";
        public List<string> Lines { get; set; } 
    }

    public class EndOfSourceToken : Token
    {
        public override string JadeEquivalentType => "eos";
    }

    public class TextToken : Token
    {
        public override string JadeEquivalentType => "text";
    }

    public class CommentToken : Token
    {
        public override string JadeEquivalentType => "comment";
        public bool Buffer { get; set; }
    }

    public class InterpolationToken : Token
    {
        public override string JadeEquivalentType => "interpolation";
    }

    public class TagToken : Token
    {
        public override string JadeEquivalentType => "tag";
        public bool SelfClosing { get; set; }
    }

    public enum BlockMode : byte
    {
        Prepend = 1,
        Append,
        Replace
    }

    public class BlockToken : Token
    {
        public override string JadeEquivalentType => "block";
        public BlockMode Mode { get; set; }
    }

    public class MixinBlockToken : Token
    {
        public override string JadeEquivalentType => "mixin-block";
    }

    public class IncludeToken : Token
    {
        public override string JadeEquivalentType => "include";
        public AttributesToken Attributes { get; set; }
        public string Filter { get; set; }
    }

    public class ColonToken : Token
    {
        public override string JadeEquivalentType => ":";
    }

    public class FilterToken : Token
    {
        public override string JadeEquivalentType => "filter";
    }

    public class DocTypeToken : Token
    {
        public override string JadeEquivalentType => "doctype";
    }

    public class IdToken : Token
    {
        public override string JadeEquivalentType => "id";
    }

    public class ClassToken : Token
    {
        public override string JadeEquivalentType => "class";
    }

    public class DotToken : Token
    {
        public override string JadeEquivalentType => "dot";
    }

    public class ExtendsToken : Token
    {
        public override string JadeEquivalentType => "extends";
    }

    public class YieldToken : Token
    {
        public override string JadeEquivalentType => "yield";
    }

    public class CaseToken : Token
    {
        public override string JadeEquivalentType => "case";
    }

    public class WhenToken : Token
    {
        public override string JadeEquivalentType => "when";
    }

    public class DefaultToken : Token
    {
        public override string JadeEquivalentType => "default";
    }
}