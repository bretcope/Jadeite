
using System;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        private TemplateNode ParseTemplate()
        {
            var template = new TemplateNode();
            template.SetModelDefinition(ParseModelDefinition());
            template.SetDocument(ParseDocument());

            return template;
        }

        private ModelDefinitionNode ParseModelDefinition()
        {
            var model = new ModelDefinitionNode();
            model.SetModelKeyword(_lexer.AdvanceKind(JadeiteSyntaxKind.ModelKeyword));
            model.SetTypeIdentifier(ParseTypeIdentifier());
            model.SetEndOfLine(_lexer.AdvanceKind(JadeiteSyntaxKind.EndOfLine));

            return model;
        }

        private TypeIdentifierNode ParseTypeIdentifier()
        {
            var ident = new TypeIdentifierNode();

            while (true)
            {
                var tok = _lexer.Current();
                if (tok.Kind != JadeiteSyntaxKind.CodeIdentifier && !SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword))
                {
                    throw new Exception($"Expected a type at Line {tok.Position.Line} Column {tok.Position.Column}."); // todo
                }

                ident.AddIdentifier(_lexer.Advance());

                if (_lexer.Current().Kind == JadeiteSyntaxKind.Dot)
                {
                    ident.AddDot(_lexer.Advance());
                }
                else
                {
                    break;
                }
            }

            return ident;
        }

        private DocumentNode ParseDocument()
        {
            //
        }
    }
}
