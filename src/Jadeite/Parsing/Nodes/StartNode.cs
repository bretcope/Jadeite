
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class StartNode : INode
    {
        [AssertKind(true, JadeiteSyntaxKind.EndOfLineList)]
        public EndOfLineListNode EndOfLines { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.File)]
        public FileNode File { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Start;

        internal StartNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (EndOfLines != null)
                yield return EndOfLines;

            yield return File;
        }
    }
}
