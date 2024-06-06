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
    }
}
