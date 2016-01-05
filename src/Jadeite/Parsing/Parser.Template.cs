
using System;
using System.Diagnostics;
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

            model.SetModelKeyword(AdvanceKind(JadeiteSyntaxKind.ModelKeyword));
            model.SetTypeIdentifier(ParseTypeIdentifier());
            model.SetEndOfLine(AdvanceKind(JadeiteSyntaxKind.EndOfLine));

            return model;
        }
        
        private TypeIdentifierNode ParseTypeIdentifier()
        {
            var ident = new TypeIdentifierNode();

            while (true)
            {
                var tok = Current;
                if (tok.Kind != JadeiteSyntaxKind.CodeIdentifier && !SyntaxInfo.IsOfCategory(tok.Kind, SyntaxCategory.TypeKeyword))
                {
                    throw new Exception($"Expected a type at Line {tok.Position.Line} Column {tok.Position.Column}."); // todo
                }

                ident.AddIdentifier(Advance());

                if (Current.Kind == JadeiteSyntaxKind.Dot)
                {
                    ident.AddDot(Advance());
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
            var doc = new DocumentNode();
            while (Current.Kind == JadeiteSyntaxKind.EndOfLine)
                doc.AddEndOfLine(Advance());

            if (Current.Kind == JadeiteSyntaxKind.ExtendsKeyword)
                doc.SetExtendsDefinition(ParseExtendsDefinition());

            doc.SetDocumentBody(ParseDocumentBody());

            return doc;
        }
        
        private InvocationNode ParseExtendsDefinition()
        {
            AssertCurrentKind(JadeiteSyntaxKind.ExtendsKeyword);

            var extends = new InvocationNode(JadeiteSyntaxKind.ExtendsDefinition);

            extends.SetLeftHandSide(Advance());
            extends.SetOpen(AdvanceKind(JadeiteSyntaxKind.OpenParen));
            extends.SetArgumentList(ParseArgumentList());
            extends.SetClose(AdvanceKind(JadeiteSyntaxKind.CloseParen));

            return extends;
        }
        
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
                var pos = Current.Position;
                throw new Exception($"Document body didn't have at least one element at Line {pos.Line}."); // todo
            }

            return body;
        }
        
        private ISyntaxElement TryParseDocumentBodyElement()
        {
            // all the obvious easy cases
            switch (Current.Kind)
            {
                case JadeiteSyntaxKind.DoctypeKeyword:
                    return ParseDoctypeDefinition();
                case JadeiteSyntaxKind.BlockKeyword:
                case JadeiteSyntaxKind.AppendKeyword:
                case JadeiteSyntaxKind.PrependKeyword:
                    return ParseNamedBlock();
                case JadeiteSyntaxKind.IncludeKeyword:
                    return ParseIncludeDefinition();
                case JadeiteSyntaxKind.Equals:
                case JadeiteSyntaxKind.BangEquals:
                    return ParseBufferedCode();
                case JadeiteSyntaxKind.Indent:
                    return ParseDocumentBlock();
                case JadeiteSyntaxKind.BufferedHtmlComment:
                case JadeiteSyntaxKind.UnbufferedHtmlComment:
                    return ParseHtmlComment();
                case JadeiteSyntaxKind.EndOfLine:
                    return Advance();
            }
            
            return (ISyntaxElement)TryParseUnbufferedCode() ?? ParseTag();
        }

        private DoctypeDefinitionNode ParseDoctypeDefinition()
        {
            var doctype = new DoctypeDefinitionNode();

            doctype.SetDoctypeKeyword(AdvanceKind(JadeiteSyntaxKind.DoctypeKeyword));
            doctype.SetTextBody(ParseTextBodyElementList());
            doctype.SetEndOfLine(AdvanceKind(JadeiteSyntaxKind.EndOfLine));

            return doctype;
        }

        private NamedBlockNode ParseNamedBlock()
        {
            AssertCurrentKind(JadeiteSyntaxKind.BlockKeyword, JadeiteSyntaxKind.AppendKeyword, JadeiteSyntaxKind.PrependKeyword);

            var namedBlock = new NamedBlockNode();

            // parse the NamedBlockPrefix
            var tok = Advance();
            namedBlock.AddPrefix(tok);
            if (tok.Kind == JadeiteSyntaxKind.BlockKeyword)
            {
                var kind = Current.Kind;
                if (kind == JadeiteSyntaxKind.AppendKeyword || kind == JadeiteSyntaxKind.PrependKeyword)
                {
                    namedBlock.AddPrefix(Advance());
                }
            }

            namedBlock.SetName(AdvanceKind(JadeiteSyntaxKind.HtmlIdentifier));
            namedBlock.SetEndOfLine(AdvanceKind(JadeiteSyntaxKind.EndOfLine));

            if (Current.Kind == JadeiteSyntaxKind.Indent)
                namedBlock.SetBody(ParseDocumentBlock());

            return namedBlock;
        }

        private InvocationNode ParseIncludeDefinition()
        {
            var include = new InvocationNode(JadeiteSyntaxKind.IncludeDefinition);

            include.SetLeftHandSide(AdvanceKind(JadeiteSyntaxKind.IncludeKeyword));
            include.SetOpen(AdvanceKind(JadeiteSyntaxKind.OpenParen));
            include.SetArgumentList(ParseArgumentList());
            include.SetClose(AdvanceKind(JadeiteSyntaxKind.CloseParen));

            return include;
        }

        private BufferedCodeNode ParseBufferedCode()
        {
            AssertCurrentKind(JadeiteSyntaxKind.Equals, JadeiteSyntaxKind.BangEquals);
            throw new NotImplementedException();
        }

        private DocumentBlockNode ParseDocumentBlock()
        {
            throw new NotImplementedException();
        }

        private HtmlCommentNode ParseHtmlComment()
        {
            throw new NotImplementedException();
        }

        private UnbufferedCodeNode TryParseUnbufferedCode()
        {
            throw new NotImplementedException();
        }

        private TagNode ParseTag()
        {
            throw new NotImplementedException();
        }

        private TextBodyElementListNode ParseTextBodyElementList()
        {
            throw new NotImplementedException();
        }

        private MixinListNode ParseMixinList()
        {
            AssertCurrentKind(JadeiteSyntaxKind.MixinKeyword);
            throw new NotImplementedException();
        }

        private MixinDefinitionNode ParseMixinDefinition()
        {
            throw new NotImplementedException();
        }
    }
}
