using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    [NodeKind(
        JadeiteKind.TagExpansion,
        JadeiteKind.InterpolatedTagExpansion,
        JadeiteKind.InterpolatedEscapedBufferedCode,
        JadeiteKind.InterpolatedUnescapedBufferedCode,
        JadeiteKind.SingleVariableDeclaration,
        JadeiteKind.MultipleVariableDeclaration
    )]
    public sealed class UnaryNode : INode, ICustomDebugNode
    {
        [AssertNotNull]
        public Token Operator { get; internal set; }
        [AssertNotNull]
        public ISyntaxElement RightHandSide { get; internal set; }

        public JadeiteKind Kind { get; }

        internal UnaryNode(JadeiteKind kind)
        {
            Kind = kind;
        }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return Operator;
            yield return RightHandSide;
        }

        void ICustomDebugNode.AssertIsValid()
        {
            if (Kind == JadeiteKind.SingleVariableDeclaration)
                ParsingDebug.AssertKindIsOneOf(RightHandSide.Kind, JadeiteKind.CodeIdentifier);
            else if (Kind == JadeiteKind.MultipleVariableDeclaration)
                ParsingDebug.AssertKindIsOneOf(RightHandSide.Kind, JadeiteKind.CodeIdentifierList);
        }
    }
}