
using System;
using System.Diagnostics;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        /*
        Template
	        ModelDefinition Document
        */
        private TemplateNode ParseTemplate()
        {
            var template = new TemplateNode();
            template.SetModelDefinition(ParseModelDefinition());
            template.SetDocument(ParseDocument());

            return template;
        }

        /*
        ModelDefinition
	        model TypeIdentifier endOfLine
        */
        private ModelDefinitionNode ParseModelDefinition()
        {
            var model = new ModelDefinitionNode();
            model.SetModelKeyword(_lexer.AdvanceKind(JadeiteSyntaxKind.ModelKeyword));
            model.SetTypeIdentifier(ParseTypeIdentifier());
            model.SetEndOfLine(_lexer.AdvanceKind(JadeiteSyntaxKind.EndOfLine));

            return model;
        }

        /*
        TypeIdentifier
	        codeIdentifier . TypeIdentifier
	        BuiltInType . TypeIdentifier
	        codeIdentifier
	        BuiltInType
        */
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

        /*
        Document
	        EndOfLineList_opt ExtendsDefinition_opt DocumentBody
        */
        private DocumentNode ParseDocument()
        {
            var doc = new DocumentNode();
            while (_lexer.Current().Kind == JadeiteSyntaxKind.EndOfLine)
                doc.AddEndOfLine(_lexer.Advance());

            if (_lexer.Current().Kind == JadeiteSyntaxKind.ExtendsKeyword)
                doc.SetExtendsDefinition(ParseExtendsDefinition());

            doc.SetDocumentBody(ParseDocumentBody());

            return doc;
        }

        /*
        ExtendsDefinition
	        extends ( ArgumentList )  // expecting params (string viewName, object viewModel)
        */
        private InvocationNode ParseExtendsDefinition()
        {
            Debug.Assert(_lexer.Current().Kind == JadeiteSyntaxKind.ExtendsKeyword);

            var node = new InvocationNode(JadeiteSyntaxKind.ExtendsDefinition);
            node.SetLeftHandSide(_lexer.Advance());
            node.SetOpen(_lexer.AdvanceKind(JadeiteSyntaxKind.OpenParen));
            node.SetArgumentList(ParseArgumentList());
            node.SetClose(_lexer.AdvanceKind(JadeiteSyntaxKind.CloseParen));

            return node;
        }

        /*
        DocumentBody
	        DocumentBodyElement DocumentBody
	        DocumentBodyElement
        */
        private DocumentBodyNode ParseDocumentBody()
        {
            var body = new DocumentBodyNode();
            ISyntaxElement e;
            while ((e = TryParseDocumentBodyElement()) != null)
            {
                body.Add(e);
            }

            // grammar requires at least one element
            if (body.Count == 0)
            {
                var pos = _lexer.Current().Position;
                throw new Exception($"Document body didn't have at least one element at Line {pos.Line}."); // todo
            }

            return body;
        }

        /*
        DocumentBodyElement
	        DoctypeDefinition
	        NamedBlock
	        IncludeDefinition
	        BufferedCode
	        UnescapedBufferedCode
	        UnbufferedCode
	        DocumentBlock
	        HtmlComment
	        Tag
	        endOfLine
        */
        private ISyntaxElement TryParseDocumentBodyElement()
        {
            switch (_lexer.Current().Kind)
            {
                case JadeiteSyntaxKind.DoctypeKeyword:
                    //
                    break;
            }
        }

        private DoctypeDefinitionNode ParseDoctypeDefinition()
        {
            Debug.Assert(_lexer.Current().Kind == JadeiteSyntaxKind.DoctypeKeyword);

            //
        }
    }
}
