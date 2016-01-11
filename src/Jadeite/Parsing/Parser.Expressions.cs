
using System;
using System.Runtime.CompilerServices;
using Jadeite.Parsing.Nodes;

namespace Jadeite.Parsing
{
    public partial class Parser
    {
        private ArgumentListNode ParseArgumentList()
        {
            var list = new ArgumentListNode();

            while (true)
            {
                list.AddArgument(ParseExpression());

                if (Current.Kind == JadeiteKind.Comma)
                    list.AddComma(Advance());
                else 
                    break;
            }

            return list;
        }

        private ISyntaxElement ParseExpression()
        {
            // need to figure out whether this is an assignment expression
            switch (Current.Kind)
            {
                case JadeiteKind.VarKeyword:
                case JadeiteKind.OpenSquareBracket:
                    return ParseAssignment();
                case JadeiteKind.CodeIdentifier:
                    if (LookAhead.Kind.IsAssignmentOperator())
                        return ParseAssignment();
                    goto default;
                default:
                    return ParseNonAssignmentExpression();
            }
        }

        private BinaryExpressionNode ParseAssignment()
        {
            var assignment = new BinaryExpressionNode(JadeiteKind.Assignment);

            assignment.LeftHandSide = ParseAssignmentTarget();

            if (!Current.Kind.IsAssignmentOperator())
                throw new Exception($"Expected an assignment operator at {Current.Position}."); // todo

            assignment.Operator = Advance();
            assignment.RightHandSide = ParseExpression();

            return assignment;
        }

        // This is here because it's part of the grammar, and it's possible there may be more than one non-assignment expression.
        // For now, we'll just make sure it gets inlined.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ISyntaxElement ParseNonAssignmentExpression()
        {
            return ParseConditionalExpression();
        }

        private ISyntaxElement ParseConditionalExpression()
        {
            var exp = ParseNullCoalescingExpression();
            if (Current.Kind != JadeiteKind.QuestionMark)
                return exp;

            var cond = new TernaryNode(JadeiteKind.ConditionalExpression);

            cond.LeftHandSide = exp;
            cond.LeftOperator = Advance();
            cond.Middle = ParseExpression();
            cond.RightOperator = AdvanceKind(JadeiteKind.Colon);
            cond.RightHandSide = ParseExpression();

            return cond;
        }

        private ISyntaxElement ParseNullCoalescingExpression()
        {
            var exp = ParseConditionalOrExpression();
            if (Current.Kind != JadeiteKind.QuestionMarkQuestionMark)
                return exp;

            var nullCoalescing = new BinaryExpressionNode(JadeiteKind.NullCoalescingExpression);

            nullCoalescing.LeftHandSide = exp;
            nullCoalescing.Operator = Advance();
            nullCoalescing.RightHandSide = ParseNullCoalescingExpression();

            return nullCoalescing;
        }

        private ISyntaxElement ParseConditionalOrExpression()
        {
            var exp = ParseConditionalAndExpression();

            while (Current.Kind == JadeiteKind.PipePipe)
            {
                var or = new BinaryExpressionNode(JadeiteKind.ConditionalOrExpression);

                or.LeftHandSide = exp;
                or.Operator = Advance();
                or.RightHandSide = ParseConditionalAndExpression();

                exp = or;
            }

            return exp;
        }

        private ISyntaxElement ParseConditionalAndExpression()
        {
            var exp = ParseOrExpression();

            while (Current.Kind == JadeiteKind.AndAnd)
            {
                var and = new BinaryExpressionNode(JadeiteKind.ConditionalAndExpression);

                and.LeftHandSide = exp;
                and.Operator = Advance();
                and.RightHandSide = ParseOrExpression();

                exp = and;
            }

            return exp;
        }

        private ISyntaxElement ParseOrExpression()
        {
            var exp = ParseXorExpression();

            while (Current.Kind == JadeiteKind.Pipe)
            {
                var or = new BinaryExpressionNode(JadeiteKind.OrExpression);

                or.LeftHandSide = exp;
                or.Operator = Advance();
                or.RightHandSide = ParseXorExpression();

                exp = or;
            }

            return exp;
        }

        private ISyntaxElement ParseXorExpression()
        {
            var exp = ParseAndExpression();

            while (Current.Kind == JadeiteKind.Caret)
            {
                var xor = new BinaryExpressionNode(JadeiteKind.XorExpression);

                xor.LeftHandSide = exp;
                xor.Operator = Advance();
                xor.RightHandSide = ParseAndExpression();

                exp = xor;
            }

            return exp;
        }

        private ISyntaxElement ParseAndExpression()
        {
            var exp = ParseEqualityExpression();

            while (Current.Kind == JadeiteKind.And)
            {
                var and = new BinaryExpressionNode(JadeiteKind.AndExpression);

                and.LeftHandSide = exp;
                and.Operator = Advance();
                and.RightHandSide = ParseEqualityExpression();

                exp = and;
            }

            return exp;
        }

        private ISyntaxElement ParseEqualityExpression()
        {
            var exp = ParseRelationalExpression();

            while (Current.Kind.IsOneOf(JadeiteKind.EqualsEquals, JadeiteKind.BangEquals))
            {
                var equality = new BinaryExpressionNode(JadeiteKind.EqualityExpression);

                equality.LeftHandSide = exp;
                equality.Operator = Advance();
                equality.RightHandSide = ParseRelationalExpression();

                exp = equality;
            }

            return exp;
        }

        private ISyntaxElement ParseRelationalExpression()
        {
            var exp = ParseShiftExpression();

            while (Current.Kind.IsOneOf(JadeiteKind.LessThan, JadeiteKind.GreaterThan, JadeiteKind.LessThanEquals, JadeiteKind.GreaterThanEquals))
            {
                var rel = new BinaryExpressionNode(JadeiteKind.RelationalExpression);

                rel.LeftHandSide = exp;
                rel.Operator = Advance();
                rel.RightHandSide = ParseShiftExpression();

                exp = rel;
            }

            return exp;
        }

        private ISyntaxElement ParseShiftExpression()
        {
            var exp = ParseAdditiveExpression();

            while (Current.Kind.IsOneOf(JadeiteKind.LessThanLessThan, JadeiteKind.GreaterThanGreaterThan))
            {
                var shift = new BinaryExpressionNode(JadeiteKind.ShiftExpression);

                shift.LeftHandSide = exp;
                shift.Operator = Advance();
                shift.RightHandSide = ParseAdditiveExpression();

                exp = shift;
            }

            return exp;
        }

        private ISyntaxElement ParseAdditiveExpression()
        {
            var exp = ParseMultiplicativeExpression();

            while (Current.Kind.IsOneOf(JadeiteKind.Plus, JadeiteKind.Minus))
            {
                var add = new BinaryExpressionNode(JadeiteKind.AdditiveExpression);

                add.LeftHandSide = exp;
                add.Operator = Advance();
                add.RightHandSide = ParseMultiplicativeExpression();

                exp = add;
            }

            return exp;
        }

        private ISyntaxElement ParseMultiplicativeExpression()
        {
            var exp = ParseUnaryExpression();

            while (Current.Kind.IsOneOf(JadeiteKind.Asterisk, JadeiteKind.ForwardSlash, JadeiteKind.Percent))
            {
                var mult = new BinaryExpressionNode(JadeiteKind.MultiplicativeExpression);

                mult.LeftHandSide = exp;
                mult.Operator = Advance();
                mult.RightHandSide = ParseUnaryExpression();

                exp = mult;
            }

            return exp;
        }

        private ISyntaxElement ParseUnaryExpression()
        {
            switch (Current.Kind)
            {
                case JadeiteKind.Plus:
                case JadeiteKind.Minus:
                case JadeiteKind.Bang:
                case JadeiteKind.Tilde:
                    var exp = new UnaryNode(JadeiteKind.UnaryExpression);
                    exp.Operator = Advance();
                    exp.RightHandSide = ParseUnaryExpression();
                    return exp;
                case JadeiteKind.PlusPlus:
                    return ParsePreIncrementExpression();
                case JadeiteKind.MinusMinus:
                    return ParsePreDecrementExpression();
                case JadeiteKind.OpenParen:
                    if (LookAhead.Kind.IsNumericTypeKeyword()) // cast expression
                    {
                        var cast = new CastExpressionNode();

                        cast.Open = Advance();
                        cast.Type = Advance();
                        cast.Close = AdvanceKind(JadeiteKind.CloseParen);
                        cast.Expression = ParseUnaryExpression();

                        return cast;
                    }
                    goto default; // parenthesized expression
                default:
                    return ParsePrimaryExpression();
            }
        }

        private UnaryNode ParsePreIncrementExpression()
        {
            AssertCurrentKind(JadeiteKind.PlusPlus);

            var exp = new UnaryNode(JadeiteKind.PreIncrementExpression);

            exp.Operator = Advance();
            exp.RightHandSide = ParseUnaryExpression();

            return exp;
        }

        private UnaryNode ParsePreDecrementExpression()
        {
            AssertCurrentKind(JadeiteKind.MinusMinus);

            var exp = new UnaryNode(JadeiteKind.PreDecrementExpression);

            exp.Operator = Advance();
            exp.RightHandSide = ParseUnaryExpression();

            return exp;
        }

        private ISyntaxElement ParsePrimaryExpression()
        {
            var currentKind = Current.Kind;

            if (currentKind == JadeiteKind.OpenParen)
                return ParseParenthesizedExpression();

            ISyntaxElement exp;
            switch (Current.Kind)
            {
                case JadeiteKind.TrueKeyword:
                case JadeiteKind.FalseKeyword:
                case JadeiteKind.CharLiteral:
                case JadeiteKind.IntegerLiteral:
                case JadeiteKind.FloatingPointLiteral:
                case JadeiteKind.StringLiteral:
                case JadeiteKind.NullKeyword:
                case JadeiteKind.CodeIdentifier:
                case JadeiteKind.ModelKeyword:
                case JadeiteKind.AttributesKeyword:
                    exp = Advance();
                    break;
                default:
                    throw new Exception($"Expected an identifier or literal at {Current.Position}."); // todo
            }

            while (true)
            {
                switch (Current.Kind)
                {
                    case JadeiteKind.Dot:
                        var memberAccess = new BinaryExpressionNode(JadeiteKind.MemberAccess);
                        memberAccess.LeftHandSide = exp;
                        memberAccess.Operator = Advance();
                        memberAccess.RightHandSide = AdvanceKind(JadeiteKind.CodeIdentifier);
                        exp = memberAccess;
                        break;
                    case JadeiteKind.OpenParen:
                        var invoke = new InvocationNode(JadeiteKind.InvocationExpression);
                        invoke.LeftHandSide = exp;
                        invoke.Open = Advance();
                        if (Current.Kind != JadeiteKind.CloseParen)
                            invoke.ArgumentList = ParseArgumentList();
                        invoke.Close = AdvanceKind(JadeiteKind.CloseParen);
                        exp = invoke;
                        break;
                    case JadeiteKind.OpenSquareBracket:
                        var elementAccess = new InvocationNode(JadeiteKind.ElementAccess);
                        elementAccess.LeftHandSide = exp;
                        elementAccess.Open = Advance();
                        elementAccess.ArgumentList = ParseArgumentList();
                        elementAccess.Close = AdvanceKind(JadeiteKind.CloseSquareBracket);
                        exp = elementAccess;
                        break;
                    case JadeiteKind.PlusPlus:
                    case JadeiteKind.MinusMinus:
                        exp = ParsePostfixExpression(exp);
                        break;
                    default:
                        return exp;
                }
            }
        }

        private PostfixExpression ParsePostfixExpression(ISyntaxElement primaryExpression)
        {
            AssertCurrentKind(JadeiteKind.PlusPlus, JadeiteKind.MinusMinus);

            var op = Advance();
            var kind = op.Kind == JadeiteKind.PlusPlus ? JadeiteKind.PostIncrementExpression : JadeiteKind.PostDecrementExpression;

            var exp = new PostfixExpression(kind);
            exp.LeftHandSide = primaryExpression;
            exp.Operator = op;

            return exp;
        }

        private BracketedNode ParseParenthesizedExpression()
        {
            AssertCurrentKind(JadeiteKind.OpenParen);

            var parenExp = new BracketedNode(JadeiteKind.ParenthesizedExpression);

            parenExp.Open = Advance();
            parenExp.Body = ParseExpression();
            parenExp.Close = AdvanceKind(JadeiteKind.CloseParen);

            return parenExp;
        }

        private ExpressionListNode ParseExpressionList()
        {
            var list = new ExpressionListNode();

            do
            {
                list.AddExpression(ParseExpression());

            } while (Current.Kind == JadeiteKind.Comma);

            return list;
        }
    }
}
