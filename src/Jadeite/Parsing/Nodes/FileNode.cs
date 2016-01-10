using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.File)]
    public sealed class FileNode : INode
    {
        [AssertKind(JadeiteKind.Template)]
        public TemplateNode Template { get; internal set; }
        [AssertKind(true, JadeiteKind.MixinList)]
        public MixinListNode Mixins { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.File;

        internal FileNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Template;

            if (Mixins != null)
                yield return Mixins;
        }
    }
}