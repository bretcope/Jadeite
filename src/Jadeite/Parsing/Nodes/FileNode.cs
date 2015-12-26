using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public class FileNode : Node
    {
        public override JadeiteSyntaxKind Kind => JadeiteSyntaxKind.File;

        public TemplateNode Template { get; private set; }
        public ElementList Mixins { get; } = new ElementList(JadeiteSyntaxKind.MixinList);

        internal FileNode() { }

        internal void AddTemplate(TemplateNode node)
        {
            Debug.Assert(Template == null);

            AddChild(node);
            Template = node;
        }

        internal void AddMixin(MixinDefinitionNode node)
        {
            AddChild(node);
            Mixins.Add(node);
        }
    }
}