using System.Linq.Expressions;
using ExpressionHelper.Model;

namespace ExpressionHelper.Operation;

public class ExpressionCall
{

    public Expression<Func<T, bool>> ToExpression<T>(IReadOnlyCollection<QueryRuleSet> queryRuleSets)
    {
        try
        {
            var parameter = Expression.Parameter(typeof(T), "p");

            var expressionList = queryRuleSets.Select(queryRuleSet => ExpressionHelperBase.CallExpression<T>(parameter, queryRuleSet)).ToList();
            if (!expressionList.Any())
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Constant(false), parameter);
            }
            var firstExpression = expressionList.First();
            if (expressionList.Count == 1)
            {
                return Expression.Lambda<Func<T, bool>>(firstExpression, parameter);
            }

            var result = firstExpression;
            for (var i = 1; i < expressionList.Count; i++)
            {
                result = Expression.AndAlso(result, expressionList[i]);
            }

            return Expression.Lambda<Func<T, bool>>(result, parameter);
        }
        catch (Exception e) when(e is InvalidOperationException or InvalidCastException or ArgumentNullException or MethodAccessException)
        {
            throw;
        }

    }
}