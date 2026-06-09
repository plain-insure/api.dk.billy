namespace Billy.Api.Models
{
    /// <summary>
    /// Sort direction passed to <c>List</c> and <c>ListAsync</c> calls.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>Ascending order (A → Z, oldest → newest, smallest → largest).</summary>
        ASC,

        /// <summary>Descending order (Z → A, newest → oldest, largest → smallest).</summary>
        DESC
    }
}
