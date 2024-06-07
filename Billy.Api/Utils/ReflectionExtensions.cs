using System.Linq.Expressions;
using System.Reflection;

namespace Billy.Api.Utils
{
    internal static class ReflectionExtensions
    {

        public static IDictionary<string, object?> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        public static string GetName<T>(this Expression<Func<T, object>> exp)
        {
            if (exp.Body is not MemberExpression body)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression ?? throw new NullReferenceException("Invalid expression");
            }

            return body.Member.Name;
        }

        // optionally or additionally put in a class<T> to capture the object type once
        // and then you don't have to repeat it if you have a lot of properties
        public static Action<T, TProperty?> GetSetter<T, TProperty>(this Expression<Func<T, TProperty?>> exp)
        {
            var parameterExp1 = Expression.Parameter(typeof(T));
            var parameterExp2 = Expression.Parameter(typeof(TProperty));

            // turning an expression body into a PropertyInfo is common enough
            // that it's a good idea to extract this to a reusable method
            var memberExp = (MemberExpression)exp.Body;
            var propertyInfo = (PropertyInfo)memberExp.Member;

            // use the PropertyInfo to make a property expression
            // for the first parameter (the object)
            var propertyExp = Expression.Property(parameterExp1, propertyInfo);

            // assignment expression that assigns the second parameter (value) to the property
            var assignmentExp = Expression.Assign(propertyExp, parameterExp2);

            // then just build the lambda, which takes 2 parameters, and has the assignment
            // expression for its body
            var setterExp = Expression.Lambda<Action<T, TProperty>>(
               assignmentExp,
               parameterExp1,
               parameterExp2
            );

            return setterExp.Compile();
        }


        public static Func<T, string> CreateIdGetter<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propInfo = (PropertyInfo)memberExpression.Member ?? throw new ArgumentException("The expression must be a property", nameof(expression));

            var propIdInfo = propInfo.DeclaringType.GetProperty($"{propInfo.Name}Id");
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<T, string>>(property, parameter);

            return lambda.Compile();
        }

        public static Func<S, IEnumerable<TProperty>> CreateListGetter<T, S, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propInfo = (PropertyInfo)memberExpression.Member ?? throw new ArgumentException("The expression must be a property", nameof(expression));

            var propIdInfo = (typeof (S)).GetProperty($"{propInfo.Name}s");
            var parameter = Expression.Parameter(typeof(S));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<S, IEnumerable<TProperty>>>(property, parameter);

            return lambda.Compile();
        }

    }
}
