
using System;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        private ISyntaxElement ParseStatement()
        {
            switch (Current.Kind)
            {
                case JadeiteKind.IfKeyword:
                    return ParseIfStatement();
                case JadeiteKind.SwitchKeyword:
                    return ParseSwitchStatement();
                case JadeiteKind.WhileKeyword:
                    return ParseWhileStatement();
                case JadeiteKind.ForeachKeyword:
                    return ParseForeachStatement();
                case JadeiteKind.ForKeyword:
                    return ParseForStatement();
                case JadeiteKind.BreakKeyword:
                case JadeiteKind.ContinueKeyword:
                case JadeiteKind.ReturnKeyword:
                    return ParseJumpStatement();
                default:
                    var exp = new LineTerminatedNode(JadeiteKind.Statement);
                    exp.LeftHandSide = ParseStatementExpression();
                    exp.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
                    return exp;
            }
        }

        private ISyntaxElement ParseStatementExpression()
        {
            // handle the easy to identify productions
            switch (Current.Kind)
            {
                case JadeiteKind.PlusPlus:
                    return ParsePreIncrementExpression();
                case JadeiteKind.MinusMinus:
                    return ParsePreDecrementExpression();
                case JadeiteKind.OpenSquareBracket:
                case JadeiteKind.VarKeyword:
                    return ParseAssignment();
                case JadeiteKind.CodeIdentifier:
                    if (LookAhead.Kind.IsAssignmentOperator())
                        return ParseAssignment();
                    break;
            }

            // at this point, it should be an invocation or postfix expression
            var pos = Current.Position;
            var exp = ParsePrimaryExpression();
            switch (exp.Kind)
            {
                case JadeiteKind.InvocationExpression:
                case JadeiteKind.PostIncrementExpression:
                case JadeiteKind.PostDecrementExpression:
                    return exp;
                default:
                    throw new Exception($"Expected a function call or postfix expression at {pos}."); // todo
            }
        }

        private IfNode ParseIfStatement()
        {
            AssertCurrentKind(JadeiteKind.IfKeyword);

            var ifNode = new IfNode();

            ifNode.IfKeyword = Advance();
            ifNode.Condition = ParseExpression();
            ifNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
            ifNode.Block = ParseDocumentBlock();

            if (Current.Kind == JadeiteKind.ElseKeyword)
                ifNode.ElseStatement = ParseElseStatement();

            return ifNode;
        }

        private ElseNode ParseElseStatement()
        {
            AssertCurrentKind(JadeiteKind.ElseKeyword);

            var elseNode = new ElseNode();

            elseNode.ElseKeyword = Advance();

            if (Current.Kind == JadeiteKind.IfKeyword)
            {
                elseNode.IfStatement = ParseIfStatement();
            }
            else
            {
                elseNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
                elseNode.Block = ParseDocumentBlock();
            }

            return elseNode;
        }

        private SwitchNode ParseSwitchStatement()
        {
            AssertCurrentKind(JadeiteKind.SwitchKeyword);

            var switchNode = new SwitchNode();

            switchNode.SwitchKeyword = Advance();
            switchNode.Expression = ParseExpression();
            switchNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            var block = new BlockNode(JadeiteKind.SwitchBody);

            block.Indent = AdvanceKind(JadeiteKind.Indent);
            block.Body = ParseSwitchSectionList();
            block.Outdent = AdvanceKind(JadeiteKind.Outdent);

            switchNode.Block = block;

            return switchNode;
        }

        private SwitchSectionListNode ParseSwitchSectionList()
        {
            var list = new SwitchSectionListNode();

            do
            {
                list.Add(ParseSwitchSection());

            } while (Current.Kind != JadeiteKind.Outdent);

            return list;
        }

        private SwitchSectionNode ParseSwitchSection()
        {
            var labelList = new SwitchLabelListNode();
            while (Current.Kind.IsOneOf(JadeiteKind.CaseKeyword, JadeiteKind.DefaultKeyword))
            {
                labelList.Add(ParseSwitchLabel());
            }

            if (labelList.ChildrenCount == 0)
                throw new Exception($"Expected a case or default statement at {Current.Position}.");

            var section = new SwitchSectionNode();

            section.LabelList = labelList;
            section.Block = ParseDocumentBlock();

            return section;
        }

        private SwitchLabelNode ParseSwitchLabel()
        {
            AssertCurrentKind(JadeiteKind.CaseKeyword, JadeiteKind.DefaultKeyword);

            var label = new SwitchLabelNode();
            var tok = Advance();

            label.CaseOrDefault = tok;

            if (tok.Kind == JadeiteKind.CaseKeyword)
                label.Expressions = ParseExpressionList();

            label.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);

            return label;
        }

        private WhileNode ParseWhileStatement()
        {
            AssertCurrentKind(JadeiteKind.WhileKeyword);

            var whileNode = new WhileNode();

            whileNode.WhileKeyword = Advance();

            if (Current.Kind != JadeiteKind.EndOfLine)
                whileNode.Condition = ParseExpression();

            whileNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
            whileNode.Block = ParseDocumentBlock();

            return whileNode;
        }

        private ForeachNode ParseForeachStatement()
        {
            AssertCurrentKind(JadeiteKind.ForeachKeyword);

            var foreachNode = new ForeachNode();

            foreachNode.ForeachKeyword = Advance();
            foreachNode.AssignmentTarget = ParseAssignmentTarget();
            foreachNode.InKeyword = AdvanceKind(JadeiteKind.InKeyword);
            foreachNode.Expression = ParseExpression();
            foreachNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
            foreachNode.Block = ParseDocumentBlock();

            return foreachNode;
        }

        private ForNode ParseForStatement()
        {
            AssertCurrentKind(JadeiteKind.ForKeyword);

            var forNode = new ForNode();

            forNode.ForKeyword = Advance();

            if (Current.Kind != JadeiteKind.SemiColon)
                forNode.Initializer = ParseStatementExpressionList();

            forNode.FirstSeparator = AdvanceKind(JadeiteKind.SemiColon);
            forNode.Condition = ParseExpression();
            forNode.SecondSeparator = AdvanceKind(JadeiteKind.SemiColon);

            if (Current.Kind != JadeiteKind.EndOfLine)
                forNode.Iterator = ParseStatementExpressionList();

            forNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
            forNode.Block = ParseDocumentBlock();

            return forNode;
        }

        private StatementExpressionListNode ParseStatementExpressionList()
        {
            var list = new StatementExpressionListNode();

            do
            {
                list.AddStatementExpression(ParseStatementExpression());

            } while (Current.Kind == JadeiteKind.Comma);

            return list;
        }

        private ISyntaxElement ParseJumpStatement()
        {
            AssertCurrentKind(JadeiteKind.BreakKeyword, JadeiteKind.ContinueKeyword, JadeiteKind.ReturnKeyword);

            switch (Current.Kind)
            {
                case JadeiteKind.BreakKeyword:
                    var breakNode = new LineTerminatedNode(JadeiteKind.BreakStatement);
                    breakNode.LeftHandSide = Advance();
                    breakNode.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
                    return breakNode;
                case JadeiteKind.ContinueKeyword:
                    var cont = new LineTerminatedNode(JadeiteKind.ContinueStatement);
                    cont.LeftHandSide = Advance();
                    cont.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
                    return cont;
                case JadeiteKind.ReturnKeyword:
                    var ret = new ReturnNode();
                    ret.ReturnKeyword = Advance();
                    if (Current.Kind != JadeiteKind.EndOfLine)
                        ret.Expressions = ParseExpressionList();
                    ret.EndOfLine = AdvanceKind(JadeiteKind.EndOfLine);
                    return ret;
                default:
                    throw new NotImplementedException();
            }
        }

        private ISyntaxElement ParseAssignmentTarget()
        {
            switch (Current.Kind)
            {
                case JadeiteKind.CodeIdentifier:
                    return Advance();
                case JadeiteKind.VarKeyword:
                    if (LookAhead.Kind == JadeiteKind.OpenSquareBracket)
                        return ParseMultipleVariableDeclaration();
                    return ParseSingleVariableDeclaration();
                case JadeiteKind.OpenSquareBracket:
                    return ParseBracketedAssignmentTarget();
                default:
                    throw new Exception($"Expected an assignment target at {Current.Position}.");
            }
        }

        private UnaryNode ParseSingleVariableDeclaration()
        {
            AssertCurrentKind(JadeiteKind.VarKeyword);

            var dec = new UnaryNode(JadeiteKind.SingleVariableDeclaration);

            dec.Operator = Advance();
            dec.RightHandSide = AdvanceKind(JadeiteKind.CodeIdentifier);

            return dec;
        }

        private UnaryNode ParseMultipleVariableDeclaration()
        {
            AssertCurrentKind(JadeiteKind.VarKeyword);

            var dec = new UnaryNode(JadeiteKind.MultipleVariableDeclaration);

            dec.Operator = Advance();

            var bracketed = new BracketedNode(JadeiteKind.BracketedCodeIdentifierList);
            bracketed.Open = AdvanceKind(JadeiteKind.OpenSquareBracket);
            bracketed.Body = ParseCodeIdentifierList();
            bracketed.Close = AdvanceKind(JadeiteKind.CloseSquareBracket);

            dec.RightHandSide = bracketed;

            return dec;
        }

        private BracketedNode ParseBracketedAssignmentTarget()
        {
            AssertCurrentKind(JadeiteKind.OpenSquareBracket);

            var target = new BracketedNode(JadeiteKind.BracketedAssignmentTarget);

            target.Open = Advance();

            var list = new IdentifierOrDeclarationListNode();
            while (true)
            {
                if (Current.Kind == JadeiteKind.VarKeyword)
                    list.AddIdentifierOrDeclaration(ParseSingleVariableDeclaration());
                else
                    list.AddIdentifierOrDeclaration(AdvanceKind(JadeiteKind.CodeIdentifier));

                if (Current.Kind == JadeiteKind.Comma)
                    list.AddComma(Advance());
                else
                    break;
            }

            target.Body = list;
            target.Close = AdvanceKind(JadeiteKind.CloseSquareBracket);

            return target;
        }

        private CodeIdentifierListNode ParseCodeIdentifierList()
        {
            var list = new CodeIdentifierListNode();

            while (true)
            {
                list.AddCodeIdentifier(AdvanceKind(JadeiteKind.CodeIdentifier));

                if (Current.Kind == JadeiteKind.Comma)
                    list.AddComma(Advance());
                else 
                    break;
            }

            return list;
        }
    }
}
