
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

            template.ModelDefinition = ParseModelDefinition();
            template.Document = ParseDocument();

            return template;
        }
        
        private ModelDefinitionNode ParseModelDefinition()
        {
            var model = new ModelDefinitionNode();

            model.ModelKeyword = AdvanceKind(JadeiteSyntaxKind.ModelKeyword);
            model.TypeIdentifier = ParseTypeIdentifier();
            model.EndOfLine = AdvanceKind(JadeiteSyntaxKind.EndOfLine);

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

                ident.AddToken(Advance());

                if (Current.Kind == JadeiteSyntaxKind.Dot)
                {
                    ident.AddToken(Advance());
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
            if (Current.Kind == JadeiteSyntaxKind.EndOfLine)
                doc.EndOfLines = ParseEndOfLineList();

            if (Current.Kind == JadeiteSyntaxKind.ExtendsKeyword)
                doc.Extends = ParseExtendsDefinition();

            doc.Body = ParseDocumentBody();

            return doc;
        }
        
        private InvocationNode ParseExtendsDefinition()
        {
            AssertCurrentKind(JadeiteSyntaxKind.ExtendsKeyword);

            var extends = new InvocationNode(JadeiteSyntaxKind.ExtendsDefinition);

            extends.LeftHandSide = Advance();
            extends.Open = AdvanceKind(JadeiteSyntaxKind.OpenParen);
            extends.ArgumentList = ParseArgumentList(optional: false);
            extends.Close = AdvanceKind(JadeiteSyntaxKind.CloseParen);

            return extends;
        }
        
        private DocumentBodyNode ParseDocumentBody()
        {
            var body = new DocumentBodyNode();
            ISyntaxElement e;
            while ((e = ParseOptionalDocumentBodyElement()) != null)
            {
                body.Add(e);
            }

            // grammar requires at least one element
            if (body.ChildrenCount == 0)
            {
                var pos = Current.Position;
                throw new Exception($"Document body didn't have at least one element at Line {pos.Line}."); // todo
            }

            return body;
        }
        
        private ISyntaxElement ParseOptionalDocumentBodyElement()
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
            
            return (ISyntaxElement)ParseOptionalUnbufferedCode() ?? ParseTag(optional: true);
        }

        private DoctypeDefinitionNode ParseDoctypeDefinition()
        {
            var doctype = new DoctypeDefinitionNode();

            doctype.DoctypeKeyword = AdvanceKind(JadeiteSyntaxKind.DoctypeKeyword);
            doctype.Body = ParseTextBodyElementList();
            doctype.EndOfLine = AdvanceKind(JadeiteSyntaxKind.EndOfLine);

            return doctype;
        }

        private NamedBlockNode ParseNamedBlock()
        {
            AssertCurrentKind(JadeiteSyntaxKind.BlockKeyword, JadeiteSyntaxKind.AppendKeyword, JadeiteSyntaxKind.PrependKeyword);

            var namedBlock = new NamedBlockNode();

            // parse the NamedBlockPrefix
            var tok = Advance();
            if (tok.Kind == JadeiteSyntaxKind.BlockKeyword)
            {
                namedBlock.BlockKeyword = tok;

                var kind = Current.Kind;
                if (kind == JadeiteSyntaxKind.AppendKeyword)
                    namedBlock.AppendKeyword = Advance();
                else if (kind == JadeiteSyntaxKind.PrependKeyword)
                    namedBlock.PrependKeyword = Advance();
            }
            else if (tok.Kind == JadeiteSyntaxKind.AppendKeyword)
            {
                namedBlock.AppendKeyword = tok;
            }
            else
            {
                namedBlock.PrependKeyword = tok;
            }

            namedBlock.Name = AdvanceKind(JadeiteSyntaxKind.HtmlIdentifier);
            namedBlock.EndOfLine = AdvanceKind(JadeiteSyntaxKind.EndOfLine);

            if (Current.Kind == JadeiteSyntaxKind.Indent)
                namedBlock.Body = ParseDocumentBlock();

            return namedBlock;
        }

        private InvocationNode ParseIncludeDefinition()
        {
            var include = new InvocationNode(JadeiteSyntaxKind.IncludeDefinition);

            include.LeftHandSide = AdvanceKind(JadeiteSyntaxKind.IncludeKeyword);
            include.Open = AdvanceKind(JadeiteSyntaxKind.OpenParen);
            include.ArgumentList = ParseArgumentList(optional: false);
            include.Close = AdvanceKind(JadeiteSyntaxKind.CloseParen);

            return include;
        }

        private BufferedCodeNode ParseBufferedCode()
        {
            AssertCurrentKind(JadeiteSyntaxKind.Equals, JadeiteSyntaxKind.BangEquals);

            var begin = Advance();
            var kind = begin.Kind == JadeiteSyntaxKind.Equals ? JadeiteSyntaxKind.EscapedBufferedCode : JadeiteSyntaxKind.UnescapedBufferedCode;
            var buffered = new BufferedCodeNode(kind);

            buffered.BeginningToken = begin;
            buffered.Expression = ParseExpression();
            buffered.EndOfLine = AdvanceKind(JadeiteSyntaxKind.EndOfLine);

            return buffered;
        }

        private DocumentBlockNode ParseDocumentBlock()
        {
            var block = new DocumentBlockNode();

            block.Indent = AdvanceKind(JadeiteSyntaxKind.Indent);
            block.Body = ParseDocumentBody();
            block.Outdent = AdvanceKind(JadeiteSyntaxKind.Outdent);

            return block;
        }

        private HtmlCommentNode ParseHtmlComment()
        {
            AssertCurrentKind(JadeiteSyntaxKind.BufferedHtmlComment, JadeiteSyntaxKind.UnbufferedHtmlComment);

            var comment = new HtmlCommentNode();

            comment.Comment = Advance();
            comment.EndOfLine = AdvanceKind(JadeiteSyntaxKind.EndOfLine);

            return comment;
        }

        private UnbufferedCodeNode ParseOptionalUnbufferedCode()
        {
            var kind = Current.Kind;
            UnbufferedCodeNode code;
            if (kind == JadeiteSyntaxKind.Minus)
            {
                code = new UnbufferedCodeNode();
                code.PrefixHyphen = Advance();
            }
            else if (SyntaxInfo.IsOfCategory(kind, SyntaxCategory.CodeKeyword))
            {
                code = new UnbufferedCodeNode();
            }
            else
            {
                return null;
            }

            code.Statement = ParseStatement();

            return code;
        }

        private TagNode ParseTag(bool optional)
        {
            TagNode tag = null;

            var kind = Current.Kind;
            if (kind == JadeiteSyntaxKind.HtmlIdentifier)
            {
                tag = new TagNode();
                tag.ElementName = Advance();
            }

            if (kind == JadeiteSyntaxKind.Dot || kind == JadeiteSyntaxKind.Hash)
            {
                if (tag == null)
                    tag = new TagNode();

                tag.ClassOrIdList = ParseClassOrIdList();
            }

            if (tag == null)
            {
                if (optional)
                    return null;
                
                throw new Exception($"Expected a tag at line {Current.Position.Line} column {Current.Position.Column}."); // todo
            }

            if (Current.Kind == JadeiteSyntaxKind.OpenParen)
                tag.Attributes = ParseTagAttributes();

            if (Current.Kind == JadeiteSyntaxKind.And)
                tag.AndAttributes = ParseAndAttributes();

            tag.Body = ParseTagBody();

            return tag;
        }

        private ClassOrIdListNode ParseClassOrIdList()
        {
            AssertCurrentKind(JadeiteSyntaxKind.Dot, JadeiteSyntaxKind.Hash);

            var list = new ClassOrIdListNode();
            JadeiteSyntaxKind kind;
            do
            {
                var classOrId = new ClassOrIdNode();
                classOrId.Prefix = Advance();
                classOrId.Name = AdvanceKind(JadeiteSyntaxKind.HtmlIdentifier);

                list.Add(classOrId);

                kind = Current.Kind;

            } while (kind == JadeiteSyntaxKind.Dot || kind == JadeiteSyntaxKind.Hash);

            return list;
        }

        private BracketedNode ParseTagAttributes()
        {
            AssertCurrentKind(JadeiteSyntaxKind.OpenParen);

            var attributes = new BracketedNode(JadeiteSyntaxKind.TagAttributes);

            attributes.Open = Advance();
            attributes.Body = ParseTagAttributesList();
            attributes.Close = AdvanceKind(JadeiteSyntaxKind.CloseParen);

            return attributes;
        }

        private TagAttributeListNode ParseTagAttributesList()
        {
            var list = new TagAttributeListNode();

            while (true)
            {
                list.AddTagAttribute(ParseTagAttribute());

                if (Current.Kind == JadeiteSyntaxKind.Comma)
                    list.AddComma(Advance());
                else 
                    break;
            }

            return list;
        }

        private TagAttributeNode ParseTagAttribute()
        {
            var attribute = new TagAttributeNode();

            attribute.LeftHandSide = AdvanceKind(JadeiteSyntaxKind.HtmlIdentifier);

            var kind = Current.Kind;
            if (kind == JadeiteSyntaxKind.Equals || kind == JadeiteSyntaxKind.BangEquals)
            {
                attribute.Operator = Advance();
                attribute.RightHandSide = ParseExpression();
            }

            return attribute;
        }

        private AndAttributesNode ParseAndAttributes()
        {
            AssertCurrentKind(JadeiteSyntaxKind.And);

            var and = new AndAttributesNode();

            and.And = Advance();
            and.AttributesKeyword = AdvanceKind(JadeiteSyntaxKind.AttributesKeyword);
            and.OpenParen = AdvanceKind(JadeiteSyntaxKind.OpenParen);
            and.Arguments = ParseArgumentList(optional: true);
            and.CloseParen = AdvanceKind(JadeiteSyntaxKind.CloseParen);

            return and;
        }

        private ISyntaxElement ParseTagBody()
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
