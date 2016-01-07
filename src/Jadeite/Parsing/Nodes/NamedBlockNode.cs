using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public class NamedBlockNode : INode, ICustomDebugNode
    {
        [AssertKind(true, JadeiteSyntaxKind.BlockKeyword)]
        public Token BlockKeyword { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.AppendKeyword)]
        public Token AppendKeyword { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.PrependKeyword)]
        public Token PrependKeyword { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.HtmlIdentifier)]
        public Token Name { get; internal set; }
        [AssertKind(JadeiteSyntaxKind.EndOfLine)]
        public Token EndOfLine { get; internal set; }
        [AssertKind(true, JadeiteSyntaxKind.DocumentBlock)]
        public DocumentBlockNode Body { get; internal set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.NamedBlock;
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

            if (Body != null)
                yield return Body;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            ParsingDebug.Assert(!(AppendKeyword != null && PrependKeyword != null)); // shouldn't be both an append and a prepend
        }
    }
}