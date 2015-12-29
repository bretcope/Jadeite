using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class DoctypeDefinitionNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public 

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.DoctypeDefinition;

        internal DoctypeDefinitionNode() { }

        internal void SetDoctypeKeyword(Token tok)
        {
            Debug.Assert(tok.Kind == JadeiteSyntaxKind.DoctypeKeyword);
            Children.Add(tok);
        }

        //
    }
}