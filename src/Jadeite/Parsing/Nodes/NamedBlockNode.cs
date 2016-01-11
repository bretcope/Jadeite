using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(JadeiteKind.NamedBlock)]
    public sealed class NamedBlockNode : INode, ICustomDebugNode
    {
        [AssertKind(true, JadeiteKind.BlockKeyword)]
        public Token BlockKeyword { get; internal set; }
        [AssertKind(true, JadeiteKind.AppendKeyword)]
        public Token AppendKeyword { get; internal set; }
        [AssertKind(true, JadeiteKind.PrependKeyword)]
        public Token PrependKeyword { get; internal set; }
        [AssertKind(JadeiteKind.HtmlIdentifier)]
        public Token Name { get; internal set; }
        [AssertKind(JadeiteKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteKind.DocumentBlock)]
        public BlockNode Block { get; internal set; }

        public JadeiteKind Kind => JadeiteKind.NamedBlock;
        public bool IsAppend => AppendKeyword != null;
        public bool IsPrepend => PrependKeyword != null;

        internal NamedBlockNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            if (BlockKeyword != null)
                yield return BlockKeyword;
            if (AppendKeyword != null)
                yield return AppendKeyword;
            if (PrependKeyword != null)
                yield return PrependKeyword;

            yield return Name;
            yield return EndOfLine;

            if (Block != null)
                yield return Block;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(!(AppendKeyword != null && PrependKeyword != null)); // shouldn't be both an append and a prepend
        }
    }
}