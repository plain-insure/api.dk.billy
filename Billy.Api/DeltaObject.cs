using System.Linq.Expressions;
using System.Reflection;

namespace Billy.Api
{

    public class DeltaObject<T> where T : class
    {
        // Tracks the property name and the explicitly assigned value
        private readonly Dictionary<string, object?> _modifications = new(StringComparer.Ordinal);

        public DeltaObject() { }

        public DeltaObject(params PropertyUpdate<T>[] updates)
        {

            foreach (var update in updates)
            {
                Add(update);
            }
        }

        public DeltaObject<T> Add(PropertyUpdate<T> update)
        {
            ArgumentNullException.ThrowIfNull(update);

            var propInfo = DeltaObject<T>.GetPropertyInfo(update.Expression);
            _modifications[propInfo.Name] = update.Value;
            return this;
        }

        /// <summary>
        /// Safely queues a property update with strict compile-time type checking.
        /// </summary>
        public DeltaObject<T> Set<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty value)
        {
            ArgumentNullException.ThrowIfNull(propertyExpression);

            if (propertyExpression.Body is not MemberExpression memberExpression)
            {
                // Handle cases where value types are implicitly boxed to object expressions
                if (propertyExpression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression innerMember)
                {
                    memberExpression = innerMember;
                }
                else
                {
                    throw new ArgumentException("Expression must point directly to a valid property.", nameof(propertyExpression));
                }
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Member expression must be a property.", nameof(propertyExpression));
            }

            // Store the property name and the new value (can be null)
            _modifications[propertyInfo.Name] = value;
            return this;
        }


        internal Dictionary<string, object?> GetModifications() => _modifications;

        /// <summary>
        /// Returns a list of property names that were modified. Useful for EF Core tracking or logging.
        /// </summary>
        public IEnumerable<string> GetChangedProperties() => _modifications.Keys;


        private static PropertyInfo GetPropertyInfo(LambdaExpression expression)
        {
            var body = expression.Body;
            if (body is UnaryExpression unary) body = unary.Operand;
            if (body is MemberExpression member && member.Member is PropertyInfo prop) return prop;
            throw new ArgumentException("Expression must point to a valid property.");
        }
    }


    public class PropertyUpdate<TEntity> where TEntity : class
    {
        public LambdaExpression Expression { get; }
        public object? Value { get; }

        // Private constructor forces usage via the clean tuple syntax
        private PropertyUpdate(LambdaExpression expression, object? value)
        {
            Expression = expression;
            Value = value;
        }

        // This implicit operator converts (Expression, Value) tuples into PropertyUpdate objects automatically
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, object?>> Expr, object? Val) tuple)
        {
            return new PropertyUpdate<TEntity>(tuple.Expr, tuple.Val);
        }

        // Overload to handle strongly typed value types (like int, bool, DateTime) without manual boxing
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, string?>> Expr, string? Val) tuple) => new(tuple.Expr, tuple.Val);
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, int>> Expr, int Val) tuple) => new(tuple.Expr, tuple.Val);
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, bool>> Expr, bool Val) tuple) => new(tuple.Expr, tuple.Val);
    }
}
