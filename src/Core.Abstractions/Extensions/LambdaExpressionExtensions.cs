using System;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Extensions
{
    public static class LambdaExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlsoOrDefault<T>(this Expression<Func<T, bool>> @this, Expression<Func<T, bool>> that)
        {
            return (Expression<Func<T, bool>>)AndAlsoOrDefault(@this, right: that);
        }

        public static LambdaExpression AndAlsoOrDefault(this LambdaExpression left, LambdaExpression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return left.Combine(right, Expression.AndAlso);
        }
        public static LambdaExpression AndAlso(this LambdaExpression left, LambdaExpression right)
        {
            return left.Combine(right, Expression.AndAlso);
        }

        public static LambdaExpression OrElse(this LambdaExpression left, LambdaExpression right)
        {
            return left.Combine(right, Expression.OrElse);
        }

        private static LambdaExpression Combine(this LambdaExpression left, LambdaExpression right, Func<Expression, Expression, Expression> operationAction)
        {
            if (left.Parameters.Count > 1 || right.Parameters.Count > 1)
            {
                throw new NotSupportedException("Not supported that the expression contains more than one parameters");
            }
            var leftParamFirst = left.Parameters.First();
            var rightParamFirst = right.Parameters.First();
            if (leftParamFirst.Type != rightParamFirst.Type)
            {
                throw new NotSupportedException("Not supported that the expressions contains different type of two parameters");
            }
            var visitor = new ParameterUpdateVisitor(rightParamFirst, leftParamFirst);
            var newRightBody = visitor.Visit(right.Body);
            var lambdaBody = operationAction(left.Body, newRightBody);
            var lambda = Expression.Lambda(lambdaBody, leftParamFirst);
            return lambda;
        }

        private class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }

    }
}
