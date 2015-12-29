
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class StartNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public FileNode File { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Start;

        internal StartNode() { }

        internal void AddEndOfLine(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.EndOfLine);
            Debug.Assert(File == null); // shouldn't be adding new lines after file has been added

            Children.Add(tok);
        }

        internal void SetFile(FileNode node)
        {
            Debug.Assert(File == null);

            Children.Add(node);
            File = node;
        }
    }
}
