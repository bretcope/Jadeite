using System.Collections.Generic;
using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class TemplateNode : INode
    {
        [AssertKind(JadeiteSyntaxKind.ModelDefinition)]
        public ModelDefinitionNode ModelDefinition { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.Document)]
        public DocumentNode Document { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.Template;

        internal TemplateNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelDefinition;
            yield return Document;
        }
    }
}