using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Billy.Api.Utils
{
    public sealed class IgnoreOnWriteTypeInfoResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var ti = base.GetTypeInfo(type, options);

            if (ti.Kind != JsonTypeInfoKind.Object)
                return ti;

            // Build a lookup of "json name" -> member (property/field), for this type including base types
            var memberByJsonName = BuildMemberMap(type, options);

            foreach (var jsonProp in ti.Properties)
            {
                if (!memberByJsonName.TryGetValue(jsonProp.Name, out var member))
                    continue;

                if (member.IsDefined(typeof(JsonIgnoreOnWriteAttribute), inherit: true))
                {
                    // Skip during serialization, keep deserialization working
                    jsonProp.ShouldSerialize = static (_, __) => false;
                }
            }

            return ti;
        }

        private static Dictionary<string, MemberInfo> BuildMemberMap(Type type, JsonSerializerOptions options)
        {
            // Use Ordinal because JSON property names are case-sensitive in System.Text.Json
            var map = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);

            foreach (var t in EnumerateTypeHierarchy(type))
            {
                const BindingFlags flags =
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

                // Properties
                foreach (var p in t.GetProperties(flags))
                {
                    if (p.GetIndexParameters().Length != 0) continue; // skip indexers

                    foreach (var jsonName in GetJsonNamesForProperty(p, options))
                        map.TryAdd(jsonName, p);
                }

                // Fields (optional; only matters if you serialize fields in options)
                foreach (var f in t.GetFields(flags))
                {
                    foreach (var jsonName in GetJsonNamesForField(f, options))
                        map.TryAdd(jsonName, f);
                }
            }

            return map;
        }

        private static IEnumerable<Type> EnumerateTypeHierarchy(Type type)
        {
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
                yield return t;
        }

        private static IEnumerable<string> GetJsonNamesForProperty(PropertyInfo p, JsonSerializerOptions options)
        {
            // 1) Explicit [JsonPropertyName] wins
            var explicitName = p.GetCustomAttribute<JsonPropertyNameAttribute>(inherit: true)?.Name;
            if (!string.IsNullOrWhiteSpace(explicitName))
                yield return explicitName;

            // 2) Otherwise naming policy (or identity)
            yield return options.PropertyNamingPolicy?.ConvertName(p.Name) ?? p.Name;

            // 3) Also allow the raw CLR name as a fallback (helps “drop-in” matching)
            // This is useful when options.PropertyNamingPolicy is set but some callers
            // still produce CLR-cased JSON.
            yield return p.Name;
        }

        private static IEnumerable<string> GetJsonNamesForField(FieldInfo f, JsonSerializerOptions options)
        {
            var explicitName = f.GetCustomAttribute<JsonPropertyNameAttribute>(inherit: true)?.Name;
            if (!string.IsNullOrWhiteSpace(explicitName))
                yield return explicitName;

            yield return options.PropertyNamingPolicy?.ConvertName(f.Name) ?? f.Name;
            yield return f.Name;
        }
    }
}

