using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Billy.Api.Repositories
{
    internal static class BaseHelpers<T, TRoot>
        where T : class, IEntity
        where TRoot : Models.Root, new()
    {
        internal static Func<TRoot?, T?> CompileDefaultSingle()
        {
            var rootType = typeof(TRoot);

            // Strategy: Look for a property matching the exact name of type T (e.g., "Bill")
            var targetPropertyName = typeof(T).Name;

            var property = rootType.GetProperty(targetPropertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null || !typeof(T).IsAssignableFrom(property.PropertyType))
            {
                // Fallback if the property doesn't exist or type doesn't match
                return _ => null;
            }

            var param = Expression.Parameter(typeof(TRoot), "src");
            Expression body = Expression.Property(param, property);

            var lambda = Expression.Lambda<Func<TRoot?, T?>>(body, param);
            return lambda.Compile();
        }

        internal static Func<TRoot?, IList<T>?> CompileDefaultMultiple(string multipleName)
        {
            var rootType = typeof(TRoot);

            var property = rootType.GetProperty(multipleName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property '{multipleName}' not found on type '{rootType.Name}'.");
            }

            var param = Expression.Parameter(typeof(TRoot), "src");

            // Handle nullability safely if TRoot can be null
            Expression body = Expression.Property(param, property);

            // Cast safely to the required interface if types aren't an exact match
            if (!typeof(IList<T>).IsAssignableFrom(property.PropertyType))
            {
                body = Expression.TypeAs(body, typeof(IList<T>));
            }

            var lambda = Expression.Lambda<Func<TRoot?, IList<T>?>>(body, param);
            return lambda.Compile();
        }

        internal static Func<T, TRoot> CompileSingleToRoot()
        {
            var rootType = typeof(TRoot);
            var itemType = typeof(T);

            // Convention: Find the property on TRoot that matches the name of T (e.g., "Product")
            var targetPropertyName = itemType.Name;
            var property = rootType.GetProperty(targetPropertyName, BindingFlags.Public | BindingFlags.Instance);

            // Fallback: If TRoot doesn't have a parameterless constructor or missing matching property
            var ctor = rootType.GetConstructor(Type.EmptyTypes);
            if (ctor == null || property == null || !property.CanWrite)
            {
                return _ => throw new InvalidOperationException(
                    $"Cannot auto-generate SingleToRoot. {rootType.Name} must have a public parameterless constructor and a writable '{targetPropertyName}' property.");
            }

            // Input parameter: (item)
            var itemParam = Expression.Parameter(itemType, "item");

            // Step 1: new TRoot()
            var newRootExpr = Expression.New(ctor);

            // Step 2: Member initialization { Product = item }
            var memberBinding = Expression.Bind(property, itemParam);
            var memberInitExpr = Expression.MemberInit(newRootExpr, memberBinding);

            // Compile lambda: (item) => new TRoot { Property = item }
            var lambda = Expression.Lambda<Func<T, TRoot>>(memberInitExpr, itemParam);
            return lambda.Compile();
        }



        internal static string DeriveIncludeProperty(string singleName, string propertyName)
        {
            return $"{singleName}.{JsonNamingPolicy.CamelCase.ConvertName(propertyName)}";
        }
    }
}