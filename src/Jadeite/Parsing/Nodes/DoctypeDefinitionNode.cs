using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DoctypeDefinitionNode : INode
    {
        public Token DoctypeKeyword { get; private set; }
        public TextBodyElementListNode Body { get; private set; }
        public Token EndOfLine { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DoctypeDefinition;

        internal DoctypeDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return DoctypeKeyword;
            yield return Body;
            yield return EndOfLine;
        }

        internal void SetDoctypeKeyword(Token doctype)
        {
            ParsingDebug.AssertKindIsOneOf(doctype.Kind, JadeiteSyntaxKind.DoctypeKeyword);
            ParsingDebug.Assert(DoctypeKeyword == null);
            DoctypeKeyword = doctype;
        }

        internal void SetTextBody(TextBodyElementListNode body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }

        internal void SetEndOfLine(Token eol)
        {
            ParsingDebug.AssertKindIsOneOf(eol.Kind, JadeiteSyntaxKind.EndOfLine);
            ParsingDebug.Assert(EndOfLine == null);
            EndOfLine = eol;
        }
    }
}