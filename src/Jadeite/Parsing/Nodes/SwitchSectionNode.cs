using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.SwitchSection)]
    public sealed class SwitchSectionNode : INode
    {
        [AssertKind(JadeiteKind.SwitchLabelList)]
        public SwitchLabelListNode LabelList { get; internal set; }
        [AssertKind(JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.SwitchSection;

        internal SwitchSectionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return LabelList;
            yield return Block;
        }
    }
}