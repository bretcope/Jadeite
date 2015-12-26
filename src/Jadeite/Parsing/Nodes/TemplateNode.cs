using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class TemplateNode : Node
    {
        public ModelDefinitionNode ModelDefinition { get; private set; }
        public DocumentNode Document { get; private set; }

        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Template;

        internal TemplateNode() { }

        internal void SetModelDefinition(ModelDefinitionNode node)
        {
            Debug.Assert(ModelDefinition == null);

            AddChild(node);
            ModelDefinition = node;
        }

        internal void SetDocument(DocumentNode node)
        {
            Debug.Assert(Document == null);

            AddChild(node);
            Document = node;
        }
    }
}