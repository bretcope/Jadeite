using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class FileNode : INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.File;

        public TemplateNode Template { get; private set; }
        public MixinListNode Mixins { get; private set; }

        internal FileNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Template;

            if (Mixins != null)
                yield return Mixins;
        }

        internal void SetTemplate(TemplateNode node)
        {
            ParsingDebug.Assert(Template == null);
            Template = node;
        }

        internal void SetMixins(MixinListNode node)
        {
            ParsingDebug.Assert(Mixins == null);
            Mixins = node;
        }
    }
}