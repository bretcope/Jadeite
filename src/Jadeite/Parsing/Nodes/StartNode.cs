
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class StartNode : Node
    {
        public FileNode File { get; private set; }

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Start;

        internal StartNode() { }

        internal void AddEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Debug.Assert(File == null); // shouldn't be adding new lines after file has been added

            AddChild(tok);
        }

        internal void SetFile(FileNode node)
        {
            Debug.Assert(File == null);

            AddChild(node);
            File = node;
        }
    }
}
