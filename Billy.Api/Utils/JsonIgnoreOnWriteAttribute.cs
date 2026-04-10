namespace Billy.Api.Utils
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class JsonIgnoreOnWriteAttribute : Attribute
    {
    }
}
