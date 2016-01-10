using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.Template)]
    public sealed class TemplateNode : INode
    {
        [AssertKind(JadeiteKind.ModelDefinition)]
        public ModelDefinitionNode ModelDefinition { get; internal set; }
        [AssertKind(JadeiteKind.Document)]
        public DocumentNode Document { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.Template;

        internal TemplateNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return ModelDefinition;
            yield return Document;
        }
    }
}