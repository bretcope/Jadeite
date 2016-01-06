using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentNode : INode
    {
        public EndOfLineListNode EndOfLines { get; private set; }
        public InvocationNode Extends { get; private set; }
        public DocumentBodyNode Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Document;

        internal DocumentNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (EndOfLines != null)
                yield return EndOfLines;

            if (Extends != null)
                yield return Extends;

            yield return Body;
        }

        internal void SetEndOfLines(EndOfLineListNode endOfLines)
        {
            ParsingDebug.Assert(EndOfLines == null);
            EndOfLines = endOfLines;
        }

        internal void SetExtendsDefinition(InvocationNode extends)
        {
            ParsingDebug.AssertKindIsOneOf(extends.Kind, JadeiteSyntaxKind.ExtendsDefinition);
            ParsingDebug.Assert(Extends == null);
            Extends = extends;
        }
        internal void SetDocumentBody(DocumentBodyNode node)
        {
            ParsingDebug.Assert(Body == null);
            Body = node;
        }
    }
}