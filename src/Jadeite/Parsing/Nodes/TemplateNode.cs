using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TemplateNode : INode
    {
        public ModelDefinitionNode ModelDefinition { get; private set; }
        public DocumentNode Document { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Template;

        internal TemplateNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelDefinition;
            yield return Document;
        }

        internal void SetModelDefinition(ModelDefinitionNode node)
        {
            Debug.Assert(ModelDefinition == null);
            ModelDefinition = node;
        }

        internal void SetDocument(DocumentNode node)
        {
            Debug.Assert(Document == null);
            Document = node;
        }
    }
}