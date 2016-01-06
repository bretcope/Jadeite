using System.Collections.Generic;

namespace Jadeite.Parsing.Nodes
{
    public sealed class MixinDefinitionNode : INode
    {
        public Token MixinKeyword { get; private set; }
        public Token Name { get; private set; }
        public BracketedNode ParametersDefinition { get; private set; }
        public DocumentBlockNode Body { get; private set; }

        public JadeiteSyntaxKind Kind => JadeiteSyntaxKind.MixinDefinition;

        internal MixinDefinitionNode() { }

        public IEnumerable<ISyntaxElement> GetChildren()
        {
            yield return MixinKeyword;
            yield return Name;
            if (ParametersDefinition != null)
                yield return ParametersDefinition;
            yield return Body;
        }

        internal void SetMixinKeyword(Token mixin)
        {
            ParsingDebug.AssertKindIsOneOf(mixin.Kind, JadeiteSyntaxKind.MixinKeyword);
            ParsingDebug.Assert(MixinKeyword == null);
            MixinKeyword = mixin;
        }

        internal void SetName(Token name)
        {
            ParsingDebug.AssertKindIsOneOf(name.Kind, JadeiteSyntaxKind.HtmlIdentifier);
            ParsingDebug.Assert(Name == null);
            Name = name;
        }

        internal void SetMixinParameters(BracketedNode parameters)
        {
            ParsingDebug.AssertKindIsOneOf(parameters.Kind, JadeiteSyntaxKind.MixinParametersDefinition);
            ParsingDebug.Assert(ParametersDefinition == null);
            ParametersDefinition = parameters;
        }

        internal void SetBody(DocumentBlockNode body)
        {
            ParsingDebug.Assert(Body == null);
            Body = body;
        }
    }
}