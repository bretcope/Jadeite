using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentBlockNode : INode
    {
        public Token Indent { get; private set; }
        public DocumentBodyNode Body { get; private set; }
        public Token Outdent { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DocumentBlock;

        internal DocumentBlockNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Indent;
            yield return Body;
            yield return Outdent;
        }

        internal void SetIndent(Token indent)
        {
            ParsingDebug.AssertKindIsOneOf(indent.Kind, JadeiteSyntaxKind.Indent);
            ParsingDebug.Assert(Indent == null);
            Indent = indent;
        }

        internal void SetBody(DocumentBodyNode body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }

        internal void SetOutdent(Token outdent)
        {
            ParsingDebug.AssertKindIsOneOf(outdent.Kind, JadeiteSyntaxKind.Outdent);
            ParsingDebug.Assert(Outdent == null);
            Outdent = outdent;
        }
    }
}