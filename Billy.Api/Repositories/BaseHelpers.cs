using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Billy.Api.Repositories
{
    internal static class BaseHelpers<T, TRoot>
        where T : class, IEntity
        where TRoot : class, new()
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


        internal static Func<TRoot?, string?> CompileDeletedToString(Type childType)
        {
            var rootType = typeof(TRoot);
            var param = Expression.Parameter(rootType, "root");

            // Step 1: Resolve the property path by convention
            // Extracts "Bill" from "BillRepository". Fallback to type T name if suffix missing.
            var baseEntityName = childType.Name.EndsWith("Repository")
                ? childType.Name[..^10]
                : typeof(T).Name;

            var pluralRecordsName = $"{baseEntityName}s"; // "Bills"

            // Build path: root.Meta.DeletedRecords.Bills
            var metaProp = rootType.GetProperty("Meta", BindingFlags.Public | BindingFlags.Instance);
            var deletedRecordsProp = metaProp?.PropertyType.GetProperty("DeletedRecords", BindingFlags.Public | BindingFlags.Instance);
            var billsProp = deletedRecordsProp?.PropertyType.GetProperty(pluralRecordsName, BindingFlags.Public | BindingFlags.Instance);

            // If your architecture doesn't fit the expected nested Meta strategy, return a fallback null lambda
            if (metaProp == null || deletedRecordsProp == null || billsProp == null)
            {
                return _ => null;
            }

            // Step 2: Build safe null-conditional checks manually using conditional expressions
            // local variables to track expression building
            var nullConst = Expression.Constant(null, typeof(object));
            var stringNullConst = Expression.Constant(null, typeof(string));

            // root == null ? null : root.Meta
            var rootNotNull = Expression.NotEqual(param, Expression.Constant(null, rootType));
            var metaExpr = Expression.Property(param, metaProp);
            var safeMeta = Expression.Condition(rootNotNull, metaExpr, Expression.Convert(nullConst, metaProp.PropertyType));

            // safeMeta == null ? null : safeMeta.DeletedRecords
            var metaNotNull = Expression.NotEqual(safeMeta, Expression.Constant(null, metaProp.PropertyType));
            var deletedExpr = Expression.Property(safeMeta, deletedRecordsProp);
            var safeDeleted = Expression.Condition(metaNotNull, deletedExpr, Expression.Convert(nullConst, deletedRecordsProp.PropertyType));

            // safeDeleted == null ? null : safeDeleted.Bills
            var deletedNotNull = Expression.NotEqual(safeDeleted, Expression.Constant(null, deletedRecordsProp.PropertyType));
            var billsExpr = Expression.Property(safeDeleted, billsProp);
            var safeBills = Expression.Condition(deletedNotNull, billsExpr, Expression.Convert(nullConst, billsProp.PropertyType));

            // Step 3: Append the .FirstOrDefault() equivalent call
            // Enumerable.FirstOrDefault(IEnumerable<string>)
            var firstOrDefaultMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == nameof(Enumerable.FirstOrDefault) && m.GetParameters().Length == 1)
                .MakeGenericMethod(typeof(string));

            // safeBills == null ? null : safeBills.FirstOrDefault()
            var billsNotNull = Expression.NotEqual(safeBills, Expression.Constant(null, billsProp.PropertyType));
            var firstOrDefaultCall = Expression.Call(firstOrDefaultMethod, safeBills);
            var finalBody = Expression.Condition(billsNotNull, firstOrDefaultCall, stringNullConst);

            var lambda = Expression.Lambda<Func<TRoot?, string?>>(finalBody, param);
            return lambda.Compile();
        }

        internal static string DeriveIncludeProperty(string propertyName)
        {
            var model = typeof(T).Name;

            return $"{JsonNamingPolicy.CamelCase.ConvertName(model)}.{JsonNamingPolicy.CamelCase.ConvertName(propertyName)}";
        }
    }
}