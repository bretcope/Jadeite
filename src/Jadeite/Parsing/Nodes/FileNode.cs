using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class FileNode : INode
    {
        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.File;

        [AssertKind(JadeiteSyntaxKind.Template)]
        public TemplateNode Template { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.MixinList)]
        public MixinListNode Mixins { get; internal set; }

        internal FileNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Template;

            if (Mixins != null)
                yield return Mixins;
        }
    }
}