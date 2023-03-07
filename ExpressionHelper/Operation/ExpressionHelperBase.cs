using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionHelper.Helper;
using ExpressionHelper.Model;

namespace ExpressionHelper.Operation;

public static class ExpressionHelperBase
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Create Expression for query.
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="queryRuleSet"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Expression? CallExpression<T>(Expression parameter, QueryRuleSet queryRuleSet)
    {
        if (queryRuleSet.Values.Count == 0)
        {
            throw new Exception("Value can not be empty");
        }

        try
        {
            var tupleList = new List<Tuple<Type?, Expression, Expression>>();
            var fieldNameList = queryRuleSet.Field.Split('.').ToList();
            var typeOfGeneric = typeof(T);
            var firstParameter = parameter;
            const string suffix = "Param_";

            for (var i = 0; i < fieldNameList.Count; i++)
            {
                if (typeOfGeneric == null) continue;
                typeOfGeneric = GetTypeOfField(typeOfGeneric, fieldNameList[i].FirstCharToUpperBySeparator());
                Expression property;
                if (CheckIsAssignableFromList(typeOfGeneric))
                {
                    property = Expression.PropertyOrField(parameter, fieldNameList[i]);
                    var type = GetType(typeOfGeneric);
                    parameter = Expression.Parameter(type, $"{suffix}{i}");
                    tupleList.Add(new Tuple<Type?, Expression, Expression>(typeOfGeneric, firstParameter,
                        (MemberExpression)property));
                    firstParameter = parameter;
                }
                else if (CheckDictionary(typeOfGeneric))
                {
                    property = Expression.PropertyOrField(parameter,
                        fieldNameList[i].FirstCharToUpperBySeparator());
                    var field = Expression.Constant(fieldNameList[++i]);
                    property = Expression.Call(property,
                        typeOfGeneric?.GetMethod("get_Item") ?? throw new InvalidOperationException(),
                        field);
                    if (i == fieldNameList.Count - 1)
                    {
                        tupleList.Add(
                            new Tuple<Type?, Expression, Expression>(typeOfGeneric, firstParameter, property));
                    }

                    parameter = property;
                    typeOfGeneric = typeOfGeneric.GetGenericArguments().LastOrDefault();
                }
                else if (typeOfGeneric != null && CheckIsClass(typeOfGeneric))
                {
                    property = Expression.PropertyOrField(parameter,
                        fieldNameList[i].FirstCharToUpperBySeparator());
                    parameter = property;
                }
                else
                {
                    property = Expression.PropertyOrField(parameter,
                        fieldNameList[i].FirstCharToUpperBySeparator());
                    tupleList.Add(new Tuple<Type?, Expression, Expression>(typeOfGeneric, firstParameter,
                        (MemberExpression)property));
                }
            }

            var newTupleList = tupleList.AsEnumerable().Reverse().ToList();

            var callExpression = CallLastPredicate(queryRuleSet.Values, queryRuleSet.Comparison,
                newTupleList.FirstOrDefault()?.Item3);

            var j = 0;
            for (var i = 1; i < newTupleList.Count; i++, j++)
            {
                var lambda = Expression.Lambda(callExpression, (ParameterExpression)newTupleList[j].Item2);
                callExpression = CallMethod(nameof(Enumerable.Any), newTupleList[i].Item1, newTupleList[i].Item3,
                    lambda);
            }

            return callExpression;
        }
        catch (Exception ex) when (ex is InvalidOperationException
                                       or InvalidCastException
                                       or ArgumentException
                                       or MethodAccessException
                                       or ArgumentNullException)
        {
            throw;
        }
    }

    // private static bool CheckIsEnum(Type typeOfGeneric)
    // {
    //      
    // }

    private static Type? GetType(Type? typeOfGeneric)
    {
        return CheckIsAssignableFromList(typeOfGeneric)
            ? typeOfGeneric?.GetGenericArguments().FirstOrDefault()
            : typeOfGeneric;
    }

    private static bool CheckIsClass(Type typeOfGeneric)
    {
        return typeOfGeneric is { IsClass: true } && typeOfGeneric != typeof(string);
    }

    private static bool CheckDictionary(Type? type)
    {
        return type != null && (type.GetInterfaces().Contains(typeof(IDictionary)));
    }

    private static Type? GetTypeOfField(Type? type, string fieldName)
    {
        return CheckIsAssignableFromList(type)
            ? type?.GetGenericArguments().FirstOrDefault()?.GetProperty(fieldName)?.PropertyType
            : type?.GetProperty(fieldName)?.PropertyType;
    }

    private static Expression? CallLastPredicate(IReadOnlyCollection<QueryRuleValueType> queryRuleSet,
        Comparison comparison,
        Expression predicate)
    {
        if (queryRuleSet == null) throw new ArgumentNullException(nameof(queryRuleSet));
        var conditionList = queryRuleSet
            .Select(query => CallExpressionCondition(comparison, predicate, Expression.Constant(query.Value))).ToList();
        var result = conditionList.FirstOrDefault();
        for (var i = 1; i < conditionList.Count; i++)
        {
            if (result != null) result = Expression.Or(result, conditionList[i]);
        }

        return result;
    }

    private static Expression CallExpressionCondition(Comparison comparison, Expression property, Expression value)
    {
        var wordToExpression =
            new Dictionary<Comparison, Func<Expression, Expression, BinaryExpression>>
            {
                { Comparison.Eq, Expression.Equal },
                { Comparison.Ne, Expression.NotEqual },
                { Comparison.Gt, Expression.GreaterThan },
                { Comparison.Gte, Expression.GreaterThanOrEqual },
                { Comparison.Lt, Expression.LessThan },
                { Comparison.Lte, Expression.LessThanOrEqual },
            };

        wordToExpression.TryGetValue(comparison, out var condition);
        if (condition is null)
        {
            throw new Exception("The conditioner not found. Please check your data");
        }

        var type = property.Type.IsGenericType
            ? property.Type.GetGenericArguments().FirstOrDefault()
            : property.Type;
        if (type != null)
        {
            if (CheckIsAssignableFromEnum(type))
            {
                var enums = Enum.Parse(type, GetExpressionValue(value).ToString() ?? string.Empty, true);
                value = Expression.Constant(enums, property.Type);
            }
            else
            {
                if (type != value.Type)
                {
                    if (!IsNumericType(property.Type) && !IsNumericType(value.Type))
                    {
                        throw new InvalidOperationException(
                            $"The binary operator {comparison} is not defined for the types {property.Type} and {value.Type}.");
                    }
                }
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"The {type} field  type not matched");
        }


        var conditionResult = condition(property, Expression.Convert(value, property.Type));
        return conditionResult;
    }

    private static object GetExpressionValue(Expression member)
    {
        var objectMember = Expression.Convert(member, typeof(object));

        var getterLambda = Expression.Lambda<Func<object>>(objectMember);

        var getter = getterLambda.Compile();

        return getter();
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(decimal) || type == typeof(decimal?) ||
               type == typeof(int) || type == typeof(int?) ||
               type == typeof(float) || type == typeof(float?) ||
               type == typeof(long) || type == typeof(long?);
    }

    private static bool CheckIsAssignableFromList(Type? type)
    {
        return type is { IsGenericType: true } &&
               type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
    }

    private static bool CheckIsAssignableFromEnum(Type type)
    {
        return type?.IsEnum ?? false;
    }

    private static Expression? CallMethod(string methodName, Type? type, Expression property, Expression predicate)
    {
        var cType = methodName.Equals(nameof(Enumerable.Any))
            ? GetListImpl(Expression.Constant(Activator.CreateInstance(type)).Type)
            : GetListImpl(type);
        var elemType = cType?.GetGenericArguments()[0];
        var predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));
        var method = (MethodInfo)GetGenericMethod(typeof(Enumerable), methodName,
            new[] { elemType },
            new[] { cType, predType }, BindingFlags.Static);
        var call = Expression.Call(method, property, predicate);
        return call;
    }

    private static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs,
        Type?[] argTypes, BindingFlags flags)
    {
        var typeArity = typeArgs.Length;
        var methods = type.GetMethods()
            .Where(m => m.Name.Equals(name))
            .Where(m => m.GetGenericArguments().Length.Equals(typeArity))
            .Select(m => m.MakeGenericMethod(typeArgs));
        Debug.Assert(methods != null, nameof(methods) + " != null");
        return Type.DefaultBinder?.SelectMethod(flags, methods?.ToArray(), argTypes, null);
    }

    private static Type? GetListImpl(Type? type)
    {
        if (CheckIsAssignableFromList(type))
            return type;
        Type?[] t = type.FindInterfaces((m, _) => CheckIsAssignableFromList(m), null);
        return t[0];
    }

    public static TAttribute? GetAttribute<TAttribute>(this Enum value)
        where TAttribute : System.Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);
        return enumType.GetField(name ?? string.Empty)?.GetCustomAttributes(false).OfType<TAttribute>()
            .SingleOrDefault();
    }
}