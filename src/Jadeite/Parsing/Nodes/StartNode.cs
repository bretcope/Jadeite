
using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.Start)]
    public sealed class StartNode : INode
    {
        [AssertKind(true, JadeiteKind.EndOfLineList)]
        public EndOfLineListNode EndOfLines { get; internal set; }
        [AssertKind(JadeiteKind.File)]
        public FileNode File { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.Start;

        internal StartNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (EndOfLines != null)
                yield return EndOfLines;

            yield return File;
        }
    }
}
