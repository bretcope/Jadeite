using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TemplateNode : INode
    {
        public ElementList Children { get; } = new ElementList();
        public ModelDefinitionNode ModelDefinition { get; private set; }
        public DocumentNode Document { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Template;

        internal TemplateNode() { }

        internal void SetModelDefinition(ModelDefinitionNode node)
        {
            Debug.Assert(ModelDefinition == null);

            Children.Add(node);
            ModelDefinition = node;
        }

        internal void SetDocument(DocumentNode node)
        {
            Debug.Assert(Document == null);

            Children.Add(node);
            Document = node;
        }
    }
}