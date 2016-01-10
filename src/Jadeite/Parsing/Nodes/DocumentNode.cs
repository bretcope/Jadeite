using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.Document)]
    public sealed class DocumentNode : INode
    {
        [AssertKind(true, JadeiteKind.EndOfLineList)]
        public EndOfLineListNode EndOfLines { get; internal set; }
        [AssertKind(true, JadeiteKind.ExtendsDefinition)]
        public InvocationNode Extends { get; internal set; }
        [AssertNotNull]
        public DocumentBodyNode Body { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.Document;

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