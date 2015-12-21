using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parser.Nodes
{
    public class HtmlTagNode : HtmlNode
    {
        public string TagName { get; private set; } = "div";
        public Token TagNameToken { get; private set; }

        public string Id { get; private set; }
        public Token IdToken { get; private set; }

        public List<string> ClassNames { get; private set; }
        public List<Token> ClassNameTokens { get; private set; }

        public HtmlAttributesListNode Attributes { get; private set; }

        public List<ISyntaxElement> Body { get; private set; }

        internal void AddIndentTokens(IEnumerable<Token> tokens)
        {
            foreach (var tok in tokens)
            {
                Debug.Assert(tok.Type == JadeiteSyntaxKind.Indent);
                AddChild(tok);
            }
        }

        internal void SetTagName(Token tok)
        {
            Debug.Assert(tok.Type == JadeiteSyntaxKind.HtmlIdentifier);
            Debug.Assert(TagNameToken == null);
            Debug.Assert(tok.Value is string);

            AddChild(tok);
            TagName = (string)tok.Value;
            TagNameToken = tok;
        }

        internal void SetId(Token tok)
        {
            Debug.Assert(tok.Type == JadeiteSyntaxKind.HtmlIdentifier);
            Debug.Assert(Id == null);
            Debug.Assert(tok.Value is string);

            AddChild(tok);
            Id = (string)tok.Value;
            IdToken = tok;
        }

        internal void AddClassName(Token tok)
        {
            Debug.Assert(tok.Type == JadeiteSyntaxKind.HtmlIdentifier);
            Debug.Assert(tok.Value is string);

            AddChild(tok);

            if (ClassNames == null)
            {
                ClassNames = new List<string>();
                ClassNameTokens = new List<Token>();
            }

            ClassNames.Add((string)tok.Value);
            ClassNameTokens.Add(tok);
        }

        internal void SetAttributes(HtmlAttributesListNode attributes)
        {
            Debug.Assert(Attributes == null);

            AddChild(attributes);
            Attributes = attributes;
        }

        internal void AddBodyElement(ISyntaxElement element)
        {
            if (Body == null)
                Body = new List<ISyntaxElement>();

            AddChild(element);
            Body.Add(element);
        }
    }
}