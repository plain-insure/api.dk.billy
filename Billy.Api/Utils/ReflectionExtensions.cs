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

        internal static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> exp)
        {
            if (exp.Body is MemberExpression body)
                return body.Member.Name;
            if (exp.Body is UnaryExpression ubody && ubody.Operand is MemberExpression ubodyMember)
                return ubodyMember.Member.Name;
            throw new ArgumentException("Expression must be a member access", nameof(exp));
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

        public static Action<T, TProperty?> GetSetterWithListSupport<T, TProperty>(this Expression<Func<T, TProperty?>> exp)
        {
            var parameterExp1 = Expression.Parameter(typeof(T), "entity");

            // This is the incoming item you want to append/add to the collection
            var parameterExp2 = Expression.Parameter(typeof(TProperty), "value");

            var memberExp = (MemberExpression)exp.Body;

            MemberExpression memberAccessExp = memberExp.Member switch
            {
                PropertyInfo propertyInfo => Expression.Property(parameterExp1, propertyInfo),
                FieldInfo fieldInfo => Expression.Field(parameterExp1, fieldInfo),
                _ => throw new ArgumentException("Expression must refer to a property or field", nameof(exp))
            };

            Expression finalBodyExpression;

            // 1. Detect if the property is a collection (IEnumerable) but not a string
            var propType = memberAccessExp.Type;
            bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(propType) && propType != typeof(string);

            if (isCollection)
            {
                // 2. Extract the inner generic type (e.g., gets 'int' from 'List<int>' or 'IEnumerable<int>')
                var itemType = propType.IsGenericType
                    ? propType.GetGenericArguments()[0]
                    : typeof(object); // Fallback for non-generic collections

                // Define a concrete List<TItem> type to instantiate if the collection is null
                var concreteListType = typeof(List<>).MakeGenericType(itemType);
                var listAddMethod = concreteListType.GetMethod("Add", [itemType])!;

                // 3. Create the block for the "Else" path (when the collection already exists)
                // We must cast the property to something with an .Add() method (like the concrete list or ICollection<T>)
                var castTargetType = typeof(ICollection<>).MakeGenericType(itemType);
                var collectionAddMethod = castTargetType.GetMethod("Add", [itemType])!;

                var existingCollectionAdd = Expression.Call(
                    Expression.Convert(memberAccessExp, castTargetType),
                    collectionAddMethod,
                    parameterExp2
                );

                // 4. Create the block for the "Then" path (when the collection IS null)
                // Local variable to hold the newly created list instance
                var newListVar = Expression.Variable(concreteListType, "newList");

                var createAndAddBlock = Expression.Block(
                    [newListVar],
                    Expression.Assign(newListVar, Expression.New(concreteListType)), // newList = new List<TItem>()
                    Expression.Call(newListVar, listAddMethod, parameterExp2),       // newList.Add(value)
                    Expression.Assign(memberAccessExp, Expression.Convert(newListVar, propType)) // entity.Prop = newList
                );

                // 5. Tie them together: if (entity.Prop == null) { Then } else { Else }
                finalBodyExpression = Expression.IfThenElse(
                    Expression.Equal(memberAccessExp, Expression.Constant(null, propType)),
                    createAndAddBlock,
                    existingCollectionAdd
                );
            }
            else
            {
                // Fallback for standard non-collection properties (original behavior)
                finalBodyExpression = Expression.Assign(memberAccessExp, parameterExp2);
            }

            // Compile the final lambda expression
            var setterExp = Expression.Lambda<Action<T, TProperty>>(
               finalBodyExpression,
               parameterExp1,
               parameterExp2
            );

            return setterExp.Compile();
        }

        /// <summary>
        /// Appends an item to a collection property. If the collection is null, initializes it first.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="TItem">The type of the item inside the collection (e.g., 'string' for List&lt;string&gt;)</typeparam>
        public static Action<T, TItem> GetCollectionAppender<T, TItem>(this Expression<Func<T, IEnumerable<TItem>?>> exp)
        {
            var parameterEntity = Expression.Parameter(typeof(T), "entity");
            var parameterItem = Expression.Parameter(typeof(TItem), "item");

            var memberExp = (MemberExpression)exp.Body;
            var propType = memberExp.Type; // e.g., List<string> or ICollection<string>

            // Create the property access expression (entity.PropertyName)
            MemberExpression memberAccessExp = memberExp.Member switch
            {
                PropertyInfo propertyInfo => Expression.Property(parameterEntity, propertyInfo),
                FieldInfo fieldInfo => Expression.Field(parameterEntity, fieldInfo),
                _ => throw new ArgumentException("Expression must refer to a property or field", nameof(exp))
            };

            // Types and methods needed for instantiation and appending
            var concreteListType = typeof(List<TItem>);
            var listAddMethod = concreteListType.GetMethod("Add", [typeof(TItem)])!;
            var collectionInterfaceType = typeof(ICollection<TItem>);
            var collectionAddMethod = collectionInterfaceType.GetMethod("Add", [typeof(TItem)])!;

            // Path A: Collection is null -> Initialize it and add the item
            // newList = new List<TItem>(); newList.Add(item); entity.Property = newList;
            var newListVar = Expression.Variable(concreteListType, "newList");
            var createAndAddBlock = Expression.Block(
                new[] { newListVar },
                Expression.Assign(newListVar, Expression.New(concreteListType)),
                Expression.Call(newListVar, listAddMethod, parameterItem),
                Expression.Assign(memberAccessExp, Expression.Convert(newListVar, propType))
            );

            // Path B: Collection already exists -> Cast to ICollection<TItem> and call .Add()
            // ((ICollection<TItem>)entity.Property).Add(item);
            var existingCollectionAdd = Expression.Call(
                Expression.Convert(memberAccessExp, collectionInterfaceType),
                collectionAddMethod,
                parameterItem
            );

            // Tie it together into an if-then-else block
            var finalBodyExpression = Expression.IfThenElse(
                Expression.Equal(memberAccessExp, Expression.Constant(null, propType)),
                createAndAddBlock,
                existingCollectionAdd
            );

            // Compile and return the action lambda
            return Expression.Lambda<Action<T, TItem>>(
                finalBodyExpression,
                parameterEntity,
                parameterItem
            ).Compile();
        }

        public static Func<T, string?> CreateSingleIdGetter<T, TProperty>(this Expression<Func<T, TProperty?>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member ?? throw new ArgumentException("The expression must be a member", nameof(expression));

            var normalizedName = NormalizeMemberName(memberInfo.Name);
            var propIdInfo = typeof(T).GetProperty($"{normalizedName}Id")
                ?? throw new ArgumentException($"No property '{normalizedName}Id' found on type '{typeof(T).Name}'", nameof(expression));
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<T, string?>>(property, parameter);

            return lambda.Compile();
        }

        public static Func<T, IEnumerable<string>?> CreateListIdGetter<T, TItem>(this Expression<Func<T, IEnumerable<TItem>?>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var memberInfo = memberExpression.Member ?? throw new ArgumentException("The expression must be a member", nameof(expression));

            var normalizedName = NormalizeMemberName(memberInfo.Name);
            var singular = normalizedName.EndsWith('s') ? normalizedName[..^1] : normalizedName;
            var propIdInfo = typeof(T).GetProperty($"{singular}Ids")
                ?? throw new ArgumentException($"No property '{singular}Ids' found on type '{typeof(T).Name}'", nameof(expression));
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propIdInfo);
            var lambda = Expression.Lambda<Func<T, IEnumerable<string>?>>(property, parameter);

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
