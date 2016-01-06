
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class StartNode : INode
    {
        public EndOfLineListNode EndOfLines { get; private set; }
        public FileNode File { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Start;

        internal StartNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (EndOfLines != null)
                yield return EndOfLines;

            yield return File;
        }

        internal void SetEndOfLines(EndOfLineListNode endOfLines)
        {
            Debug.Assert(EndOfLines == null);
            EndOfLines = endOfLines;
        }

        internal void SetFile(FileNode node)
        {
            Debug.Assert(File == null);
            File = node;
        }
    }
}
