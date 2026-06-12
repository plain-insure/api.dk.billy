using System.Linq.Expressions;
using System.Text.Json;

namespace Billy.Api
{
    public class FilterObject<T> : DeltaObject<T> where T : class
    {
        /// <summary>
        /// Records an update for a single property with compile-time type safety.
        /// Calling this method multiple times for the same property overwrites the previous value.
        /// </summary>
        /// <param name="propertyExpression">Expression selecting the property to update.</param>
        /// <param name="value">New value to send in the PUT request.</param>
        /// <returns>This instance, enabling fluent chaining.</returns>
        public DeltaObject<T> Max(Expression<Func<T, DateOnly>> propertyExpression, DateOnly value) 
        {
            var name = GetPropertyNameFromExpression(propertyExpression);

            _modifications[$"max{name}"] = value;
            return this;
        }
    }
}
