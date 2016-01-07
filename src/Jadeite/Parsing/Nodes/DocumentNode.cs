using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DocumentNode : INode
    {
        [AssertKind(true, JadeiteSyntaxKind.EndOfLineList)]
        public EndOfLineListNode EndOfLines { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.ExtendsDefinition)]
        public InvocationNode Extends { get; internal set; }
        [AssertNotNull]
        public DocumentBodyNode Body { get; internal set; }

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
    }
}