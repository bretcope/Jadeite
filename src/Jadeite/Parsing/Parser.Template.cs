
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

            model.ModelKeyword = AdvanceKind(JadeiteKind.ModelKeyword);
            model.TypeIdentifier = ParseTypeIdentifier();
            model.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return model;
        }
        
        private TypeIdentifierNode ParseTypeIdentifier()
        {
            var ident = new TypeIdentifierNode();

            while (true)
            {
                var tok = Current;
                if (tok.Kind != JadeiteKind.CodeIdentifier && !tok.Kind.IsOfCategory(SyntaxCategory.TypeKeyword))
                {
                    throw new Exception($"Expected a type at {tok.Position}."); // todo
                }

                ident.AddToken(Advance());

                if (Current.Kind == JadeiteKind.Dot)
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
            if (Current.Kind == JadeiteKind.EndOfLine)
                doc.EndOfLines = ParseEndOfLineList();

            if (Current.Kind == JadeiteKind.ExtendsKeyword)
                doc.Extends = ParseExtendsDefinition();

            doc.Body = ParseDocumentBody();

            return doc;
        }
        
        private InvocationNode ParseExtendsDefinition()
        {
            AssertCurrentKind(JadeiteKind.ExtendsKeyword);

            var extends = new InvocationNode(JadeiteKind.ExtendsDefinition);

            extends.LeftHandSide = Advance();
            extends.Open = AdvanceKind(JadeiteKind.OpenParen);
            extends.ArgumentList = ParseArgumentList();
            extends.Close = AdvanceKind(JadeiteKind.CloseParen);

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
                throw new Exception($"Document body didn't have at least one element at {Current.Position}."); // todo

            return body;
        }
        
        private ISyntaxElement ParseOptionalDocumentBodyElement()
        {
            // all the obvious easy cases
            switch (Current.Kind)
            {
                case JadeiteKind.DoctypeKeyword:
                    return ParseDoctypeDefinition();
                case JadeiteKind.BlockKeyword:
                case JadeiteKind.AppendKeyword:
                case JadeiteKind.PrependKeyword:
                    return ParseNamedBlock();
                case JadeiteKind.IncludeKeyword:
                    return ParseIncludeDefinition();
                case JadeiteKind.Equals:
                case JadeiteKind.BangEquals:
                    return ParseBufferedCode(interpolated: false);
                case JadeiteKind.Indent:
                    return ParseDocumentBlock();
                case JadeiteKind.BufferedHtmlComment:
                case JadeiteKind.UnbufferedHtmlComment:
                    return ParseHtmlComment();
                case JadeiteKind.EndOfLine:
                    return Advance();
            }
            
            return (ISyntaxElement)ParseOptionalUnbufferedCode() ?? ParseTag(optional: true);
        }

        private DoctypeDefinitionNode ParseDoctypeDefinition()
        {
            var doctype = new DoctypeDefinitionNode();

            doctype.DoctypeKeyword = AdvanceKind(JadeiteKind.DoctypeKeyword);
            doctype.Body = ParseTextBodyElementList();
            doctype.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return doctype;
        }

        private NamedBlockNode ParseNamedBlock()
        {
            AssertCurrentKind(JadeiteKind.BlockKeyword, JadeiteKind.AppendKeyword, JadeiteKind.PrependKeyword);

            var namedBlock = new NamedBlockNode();

            // parse the NamedBlockPrefix
            var tok = Advance();
            if (tok.Kind == JadeiteKind.BlockKeyword)
            {
                namedBlock.BlockKeyword = tok;

                var kind = Current.Kind;
                if (kind == JadeiteKind.AppendKeyword)
                    namedBlock.AppendKeyword = Advance();
                else if (kind == JadeiteKind.PrependKeyword)
                    namedBlock.PrependKeyword = Advance();
            }
            else if (tok.Kind == JadeiteKind.AppendKeyword)
            {
                namedBlock.AppendKeyword = tok;
            }
            else
            {
                namedBlock.PrependKeyword = tok;
            }

            namedBlock.Name = AdvanceKind(JadeiteKind.HtmlIdentifier);
            namedBlock.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            if (Current.Kind == JadeiteKind.Indent)
                namedBlock.Block = ParseDocumentBlock();

            return namedBlock;
        }

        private InvocationNode ParseIncludeDefinition()
        {
            var include = new InvocationNode(JadeiteKind.IncludeDefinition);

            include.LeftHandSide = AdvanceKind(JadeiteKind.IncludeKeyword);
            include.Open = AdvanceKind(JadeiteKind.OpenParen);
            include.ArgumentList = ParseArgumentList();
            include.Close = AdvanceKind(JadeiteKind.CloseParen);

            return include;
        }

        private BufferedCodeNode ParseBufferedCode(bool interpolated)
        {
            AssertCurrentKind(JadeiteKind.Equals, JadeiteKind.BangEquals);

            var begin = Advance();
            JadeiteKind kind;
            if (begin.Kind == JadeiteKind.Equals)
                kind = interpolated ? JadeiteKind.InterpolatedEscapedBufferedCode : JadeiteKind.EscapedBufferedCode;
            else
                kind = interpolated ? JadeiteKind.InterpolatedUnescapedBufferedCode : JadeiteKind.UnescapedBufferedCode;

            var buffered = new BufferedCodeNode(kind);

            buffered.BeginningToken = begin;
            buffered.Expression = ParseExpression();

            if (!interpolated)
                buffered.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return buffered;
        }

        private BlockNode ParseDocumentBlock()
        {
            var block = new BlockNode(JadeiteKind.DocumentBlock);

            block.Indent = AdvanceKind(JadeiteKind.Indent);
            block.Body = ParseDocumentBody();
            block.Outdent = AdvanceKind(JadeiteKind.Outdent);

            return block;
        }

        private HtmlCommentNode ParseHtmlComment()
        {
            AssertCurrentKind(JadeiteKind.BufferedHtmlComment, JadeiteKind.UnbufferedHtmlComment);

            var comment = new HtmlCommentNode();

            comment.Comment = Advance();
            comment.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return comment;
        }

        private UnbufferedCodeNode ParseOptionalUnbufferedCode()
        {
            var kind = Current.Kind;
            UnbufferedCodeNode code;
            if (kind == JadeiteKind.Minus)
            {
                code = new UnbufferedCodeNode();
                code.PrefixHyphen = Advance();
            }
            else if (kind.IsOfCategory(SyntaxCategory.CodeKeyword))
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

        private TagNode ParseTag(bool optional, bool interpolated = false)
        {
            TagNode tag = null;

            var currentKind = Current.Kind;
            if (currentKind == JadeiteKind.HtmlIdentifier)
            {
                tag = new TagNode(interpolated);
                tag.ElementName = Advance();
                currentKind = Current.Kind;
            }

            if (currentKind == JadeiteKind.Dot || currentKind == JadeiteKind.Hash)
            {
                if (tag == null)
                    tag = new TagNode(interpolated);

                tag.ClassOrIdList = ParseClassOrIdList();
            }

            if (tag == null)
            {
                if (optional)
                    return null;
                
                throw new Exception($"Expected a tag at {Current.Position}."); // todo
            }

            if (Current.Kind == JadeiteKind.OpenParen)
                tag.Attributes = ParseTagAttributes();

            if (Current.Kind == JadeiteKind.And)
                tag.AndAttributes = ParseAndAttributes();

            tag.Body = ParseTagBody(interpolated);

            return tag;
        }

        private ClassOrIdListNode ParseClassOrIdList()
        {
            AssertCurrentKind(JadeiteKind.Dot, JadeiteKind.Hash);

            var list = new ClassOrIdListNode();
            JadeiteKind kind;
            do
            {
                var classOrId = new ClassOrIdNode();
                classOrId.Prefix = Advance();
                classOrId.Name = AdvanceKind(JadeiteKind.HtmlIdentifier);

                list.Add(classOrId);

                kind = Current.Kind;

            } while (kind == JadeiteKind.Dot || kind == JadeiteKind.Hash);

            return list;
        }

        private BracketedNode ParseTagAttributes()
        {
            AssertCurrentKind(JadeiteKind.OpenParen);

            var attributes = new BracketedNode(JadeiteKind.TagAttributes);

            attributes.Open = Advance();
            attributes.Body = ParseTagAttributesList();
            attributes.Close = AdvanceKind(JadeiteKind.CloseParen);

            return attributes;
        }

        private TagAttributeListNode ParseTagAttributesList()
        {
            var list = new TagAttributeListNode();

            while (true)
            {
                list.AddTagAttribute(ParseTagAttribute());

                if (Current.Kind == JadeiteKind.Comma)
                    list.AddComma(Advance());
                else 
                    break;
            }

            return list;
        }

        private TagAttributeNode ParseTagAttribute()
        {
            var attribute = new TagAttributeNode();

            attribute.LeftHandSide = AdvanceKind(JadeiteKind.HtmlIdentifier);

            var kind = Current.Kind;
            if (kind == JadeiteKind.Equals || kind == JadeiteKind.BangEquals)
            {
                attribute.Operator = Advance();
                attribute.RightHandSide = ParseExpression();
            }

            return attribute;
        }

        private AndAttributesNode ParseAndAttributes()
        {
            AssertCurrentKind(JadeiteKind.And);

            var and = new AndAttributesNode();

            and.And = Advance();
            and.AttributesKeyword = AdvanceKind(JadeiteKind.AttributesKeyword);
            and.OpenParen = AdvanceKind(JadeiteKind.OpenParen);
            if (Current.Kind != JadeiteKind.CloseParen)
                and.Arguments = ParseArgumentList();
            and.CloseParen = AdvanceKind(JadeiteKind.CloseParen);

            return and;
        }

        private ISyntaxElement ParseTagBody(bool interpolated)
        {
            switch (Current.Kind)
            {
                case JadeiteKind.Colon:
                    return ParseTagExpansion(interpolated);
                case JadeiteKind.Dot:
                    if (interpolated)
                        throw new Exception($"Cannot use pipeless text inside an interpolated tag. {Current.Position}"); // todo
                    return ParsePipelessText();
                case JadeiteKind.Equals:
                case JadeiteKind.BangEquals:
                    return ParseBufferedCodeBody(interpolated);
                case JadeiteKind.ForwardSlash:
                    if (interpolated)
                        return Advance(); // with interpolation, self-closing isn't followed up an end-of-line.
                    return ParseSelfClosingBody();
                default:
                    if (interpolated)
                        return ParseTextBodyElementList(optional: true);
                    return ParseTextBody();
            }
        }

        private UnaryNode ParseTagExpansion(bool interpolated)
        {
            AssertCurrentKind(JadeiteKind.Colon);

            var tagExpansion = new UnaryNode(JadeiteKind.TagExpansion);

            tagExpansion.Operator = Advance();
            tagExpansion.RightHandSide = ParseTag(optional: false, interpolated: interpolated);

            return tagExpansion;
        }

        private PipelessTextNode ParsePipelessText()
        {
            AssertCurrentKind(JadeiteKind.Dot);

            var pipeless = new PipelessTextNode();

            pipeless.Dot = Advance();
            pipeless.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            if (Current.Kind == JadeiteKind.Indent)
            {
                var block = new BlockNode(JadeiteKind.PipelessTextBlock);

                block.Indent = Advance();
                block.Body = ParseTextBodyElementList();
                block.Outdent = AdvanceKind(JadeiteKind.Outdent);

                pipeless.Block = block;
            }

            return pipeless;
        }

        private BufferedCodeBodyNode ParseBufferedCodeBody(bool interpolated)
        {
            var node = new BufferedCodeBodyNode();
            node.BufferedCode = ParseBufferedCode(interpolated);

            if (Current.Kind == JadeiteKind.Indent)
                node.Block = ParseDocumentBlock();

            return node;
        }

        private LineTerminatedNode ParseSelfClosingBody()
        {
            AssertCurrentKind(JadeiteKind.ForwardSlash);

            var closing = new LineTerminatedNode(JadeiteKind.SelfClosingBody);

            closing.LeftHandSide = Advance();
            closing.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return closing;
        }

        private TextBodyNode ParseTextBody()
        {
            var textBody = new TextBodyNode();

            textBody.TextBodyElementList = ParseTextBodyElementList(optional:true);
            textBody.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            if (Current.Kind == JadeiteKind.Indent)
                textBody.Block = ParseDocumentBlock();

            return textBody;
        }

        private TextBodyElementListNode ParseTextBodyElementList(bool optional = false)
        {
            var list = new TextBodyElementListNode();

            while (true)
            {
                switch (Current.Kind)
                {
                    case JadeiteKind.HtmlText:
                        list.Add(Advance());
                        break;
                    case JadeiteKind.OpenTagInterpolation:
                        list.Add(ParseTagInterpolation());
                        break;
                    case JadeiteKind.OpenEscapedInterpolation:
                    case JadeiteKind.OpenUnscapedInterpolation:
                        list.Add(ParseInterpolatedExpression());
                        break;
                    default:
                        goto EXIT_LOOP;
                }
            }

            EXIT_LOOP:
            ;

            if (!optional && list.ChildrenCount == 0)
                throw new Exception($"Expected a text body element such as text or an interpolation expression. {Current.Position}"); // todo

            return list;
        }

        private BracketedNode ParseTagInterpolation()
        {
            AssertCurrentKind(JadeiteKind.OpenTagInterpolation);

            var node = new BracketedNode(JadeiteKind.InterpolatedTag);
            node.Open = Advance();
            node.Body = ParseTag(optional: false, interpolated: true);
            node.Close = AdvanceKind(JadeiteKind.CloseSquareBracket);

            return node;
        }

        private BracketedNode ParseInterpolatedExpression()
        {
            AssertCurrentKind(JadeiteKind.OpenEscapedInterpolation, JadeiteKind.OpenUnscapedInterpolation);

            var open = Advance();
            var kind = open.Kind == JadeiteKind.OpenEscapedInterpolation
                ? JadeiteKind.EscapedInterpolatedExpression
                : JadeiteKind.UnescapedInterpolatedExpression;

            var node = new BracketedNode(kind);

            node.Open = open;
            node.Body = ParseExpression();
            node.Close = AdvanceKind(JadeiteKind.CloseCurly);

            return node;
        }

        private MixinListNode ParseMixinList()
        {
            AssertCurrentKind(JadeiteKind.MixinKeyword);

            var list = new MixinListNode();

            do
            {
                list.Add(ParseMixinDefinition());

            } while (Current.Kind == JadeiteKind.MixinKeyword);

            return list;
        }

        private MixinDefinitionNode ParseMixinDefinition()
        {
            AssertCurrentKind(JadeiteKind.MixinKeyword);

            var mixin = new MixinDefinitionNode();

            mixin.MixinKeyword = Advance();
            mixin.Name = AdvanceKind(JadeiteKind.HtmlIdentifier);

            if (Current.Kind == JadeiteKind.OpenParen)
            {
                var parameters = new BracketedNode(JadeiteKind.MixinParametersDefinition);

                parameters.Open = Advance();

                if (Current.Kind == JadeiteKind.CodeIdentifierList)
                    parameters.Body = ParseCodeIdentifierList();

                parameters.Close = AdvanceKind(JadeiteKind.CloseParen);

                mixin.ParametersDefinition = parameters;
            }

            mixin.Block = ParseDocumentBlock();

            return mixin;
        }
    }
}
