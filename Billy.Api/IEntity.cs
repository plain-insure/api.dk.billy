namespace Billy.Api
{
    /// <summary>
    /// Marker interface implemented by all Billy API entity models.
    /// Provides the unique string identifier that the Billy API assigns to every resource.
    /// </summary>
    public interface IEntity
    {
        /// <summary>The unique identifier assigned by the Billy API.</summary>
        string Id { get; set; }
    }
}
