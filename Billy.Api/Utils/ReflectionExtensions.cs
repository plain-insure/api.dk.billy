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

            var memberExp = (MemberExpression)exp.Body;

            MemberExpression memberAccessExp = memberExp.Member switch
            {
                PropertyInfo propertyInfo => Expression.Property(parameterExp1, propertyInfo),
                FieldInfo fieldInfo => Expression.Field(parameterExp1, fieldInfo),
                _ => throw new ArgumentException("Expression must refer to a property or field", nameof(exp))
            };

            var assignmentExp = Expression.Assign(memberAccessExp, parameterExp2);

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
            var memberInfo = memberExpression.Member ?? throw new ArgumentException("The expression must be a member", nameof(expression));

            var normalizedName = NormalizeMemberName(memberInfo.Name);
            var propIdInfo = typeof(T).GetProperty($"{normalizedName}Id")
                ?? throw new ArgumentException($"No property '{normalizedName}Id' found on type '{typeof(T).Name}'", nameof(expression));
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<T, string>>(property, parameter);

            return lambda.Compile();
        }

        public static Func<S, IEnumerable<TProperty>> CreateListGetter<T, S, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member ?? throw new ArgumentException("The expression must be a member", nameof(expression));

            var normalizedName = NormalizeMemberName(memberInfo.Name);
            var propIdInfo = typeof(S).GetProperty($"{normalizedName}s")
                ?? throw new ArgumentException($"No property '{normalizedName}s' found on type '{typeof(S).Name}'", nameof(expression));
            var parameter = Expression.Parameter(typeof(S));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<S, IEnumerable<TProperty>>>(property, parameter);

            return lambda.Compile();
        }

        private static string NormalizeMemberName(string name)
        {
            name = name.TrimStart('_');
            return name.Length > 0 ? char.ToUpper(name[0]) + name[1..] : name;
        }

    }
}
