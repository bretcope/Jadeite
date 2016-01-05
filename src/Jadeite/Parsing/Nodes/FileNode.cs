using System.Diagnostics;

namespace Jadeite.Parsing.Nodes
{
    public sealed class FileNode : INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.File;

        public ElementList Children { get; } = new ElementList();
        public TemplateNode Template { get; private set; }
        public MixinListNode Mixins { get; private set; }

        internal FileNode() { }

        internal void SetTemplate(TemplateNode node)
        {
            Debug.Assert(Template == null);
            Debug.Assert(Mixins == null);

            Children.Add(node);
            Template = node;
        }

        internal void SetMixins(MixinListNode node)
        {
            Debug.Assert(Mixins == null);
            Mixins = node;
        }
    }
}