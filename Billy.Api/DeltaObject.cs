using Billy.Api.Utils;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billy.Api
{

    /// <summary>
    /// Tracks a set of explicitly assigned property changes for a partial PUT request.
    /// Only the properties you call <see cref="Set{TProperty}"/> on are included in the request body,
    /// so unset properties are never overwritten on the server.
    /// <para>
    /// A <see cref="DeltaObject{T}"/> can also be passed as a filter to <c>ListAsync</c> — only its
    /// tracked modifications are sent as query parameters.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The entity type whose properties are being tracked.</typeparam>
    /// <example>
    /// <code>
    /// var delta = new DeltaObject&lt;Bill&gt;()
    ///     .Set(b => b.State, BillStates.approved)
    ///     .Set(b => b.DueDate, DateTime.Today.AddDays(30));
    ///
    /// await bills.UpdateAsync(id, delta);
    /// </code>
    /// </example>
    public class DeltaObject<T> where T : class
    {
        // Tracks the property name and the explicitly assigned value
        protected readonly Dictionary<string, object?> _modifications = new(StringComparer.Ordinal);

        // Per-T cache of camelCase property name → PropertyInfo, built once per concrete type
        private static readonly Dictionary<string, PropertyInfo> _typePropertyCache =
            typeof(T).GetProperties()
                     .ToDictionary(p => JsonNamingPolicy.CamelCase.ConvertName(p.Name));

        // Options cache keyed by converter type so we don't allocate new JsonSerializerOptions repeatedly
        private static readonly ConcurrentDictionary<Type, JsonSerializerOptions> _converterOptionsCache = new();

        private static JsonSerializerOptions GetConverterOptions(Type converterType) =>
            _converterOptionsCache.GetOrAdd(converterType, t =>
            {
                var opts = new JsonSerializerOptions(RestJsonOptions.Instance);
                opts.Converters.Insert(0, (JsonConverter)Activator.CreateInstance(t)!);
                return opts;
            });

        /// <summary>Initializes an empty delta with no tracked changes.</summary>
        public DeltaObject() { }

        /// <summary>
        /// Initializes a delta pre-populated with one or more property updates expressed as tuples.
        /// </summary>
        /// <param name="updates">
        /// Property updates using implicit tuple syntax:
        /// <c>new DeltaObject&lt;Bill&gt;((b => b.State, BillStates.approved))</c>.
        /// </param>
        public DeltaObject(params PropertyUpdate<T>[] updates)
        {

            foreach (var update in updates)
            {
                Add(update);
            }
        }

        /// <summary>
        /// Adds a property update from a <see cref="PropertyUpdate{TEntity}"/> (tuple) value.
        /// </summary>
        /// <param name="update">The property update to record.</param>
        /// <returns>This instance, enabling fluent chaining.</returns>
        public DeltaObject<T> Add(PropertyUpdate<T> update)
        {
            ArgumentNullException.ThrowIfNull(update);

            var propInfo = DeltaObject<T>.GetPropertyInfo(update.Expression);
            _modifications[JsonNamingPolicy.CamelCase.ConvertName(propInfo.Name)] = update.Value;
            return this;
        }

        /// <summary>
        /// Records an update for a single property with compile-time type safety.
        /// Calling this method multiple times for the same property overwrites the previous value.
        /// </summary>
        /// <typeparam name="TProperty">Type of the property being set.</typeparam>
        /// <param name="propertyExpression">Expression selecting the property to update.</param>
        /// <param name="value">New value to send in the PUT request.</param>
        /// <returns>This instance, enabling fluent chaining.</returns>
        public DeltaObject<T> Set<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty value)
        {
            var name = GetPropertyNameFromExpression(propertyExpression);
            // Store the property name and the new value (can be null)
            _modifications[JsonNamingPolicy.CamelCase.ConvertName(name)] = value;
            return this;
        }

        protected string GetPropertyNameFromExpression<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
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
            return propertyInfo.Name;
        }

        internal Dictionary<string, object?> GetModifications() => _modifications;

        /// <summary>
        /// Returns modifications serialized to JsonElements, respecting [JsonConverter] attributes on T's properties.
        /// Use this when building a JSON body so per-property converters (e.g. BillyDateConverter) are applied.
        /// </summary>
        internal Dictionary<string, JsonElement> GetSerializedModifications()
        {
            var result = new Dictionary<string, JsonElement>(_modifications.Count, StringComparer.Ordinal);
            foreach (var (key, value) in _modifications)
            {
                if (_typePropertyCache.TryGetValue(key, out var propInfo))
                {
                    var converterType = propInfo.GetCustomAttribute<JsonConverterAttribute>()?.ConverterType;
                    var opts = converterType is not null ? GetConverterOptions(converterType) : RestJsonOptions.Instance;
                    result[key] = JsonSerializer.SerializeToElement(value, propInfo.PropertyType, opts);
                }
                else
                {
                    result[key] = JsonSerializer.SerializeToElement(value, RestJsonOptions.Instance);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the camelCase names of all properties that have been set on this delta.
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


    /// <summary>
    /// Pairs a property selector expression with a new value, used to populate a
    /// <see cref="DeltaObject{TEntity}"/> via the tuple constructor syntax.
    /// </summary>
    /// <typeparam name="TEntity">The entity type the property belongs to.</typeparam>
    public class PropertyUpdate<TEntity> where TEntity : class
    {
        /// <summary>Lambda expression that selects the property to update.</summary>
        public LambdaExpression Expression { get; }

        /// <summary>New value to assign to the property.</summary>
        public object? Value { get; }

        // Private constructor forces usage via the clean tuple syntax
        private PropertyUpdate(LambdaExpression expression, object? value)
        {
            Expression = expression;
            Value = value;
        }

        /// <summary>
        /// Converts a <c>(Expression, Value)</c> tuple into a <see cref="PropertyUpdate{TEntity}"/>.
        /// Enables the concise tuple syntax: <c>(e => e.Prop, value)</c>.
        /// </summary>
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, object?>> Expr, object? Val) tuple)
        {
            return new PropertyUpdate<TEntity>(tuple.Expr, tuple.Val);
        }

        /// <summary>Overload for strongly-typed <c>string?</c> values without manual boxing.</summary>
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, string?>> Expr, string? Val) tuple) => new(tuple.Expr, tuple.Val);

        /// <summary>Overload for strongly-typed <c>int</c> values without manual boxing.</summary>
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, int>> Expr, int Val) tuple) => new(tuple.Expr, tuple.Val);

        /// <summary>Overload for strongly-typed <c>bool</c> values without manual boxing.</summary>
        public static implicit operator PropertyUpdate<TEntity>((Expression<Func<TEntity, bool>> Expr, bool Val) tuple) => new(tuple.Expr, tuple.Val);
    }
}
