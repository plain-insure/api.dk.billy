namespace Billy.Api.Models
{
    /// <summary>
    /// Base response envelope returned by all Billy API endpoints.
    /// Subclasses add typed resource properties (e.g. <see cref="BillRoot.Bill"/>).
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Response metadata including HTTP status, timing, paging info, and deleted record IDs.
        /// </summary>
        public Meta? Meta { get; set; }
    }

    /// <summary>
    /// Metadata included in every Billy API response.
    /// </summary>
    public class Meta
    {
        /// <summary>HTTP status code returned by the API (e.g. 200, 201, 404).</summary>
        public int StatusCode { get; set; }

        /// <summary><c>true</c> when the request completed successfully.</summary>
        public bool Success { get; set; }

        /// <summary>Server-side processing time in seconds.</summary>
        public double Time { get; set; }

        /// <summary>Pagination state. Only populated on list responses.</summary>
        public Paging Paging { get; set; }

        /// <summary>
        /// IDs of records deleted as a side-effect of the request, keyed by resource type (plural camelCase).
        /// Only populated on delete responses.
        /// </summary>
        public Dictionary<string, List<string>>? DeletedRecords { get; set; }
    }

    /// <summary>
    /// Pagination information returned in list responses.
    /// The API supports a maximum of 1 000 records per page (<c>pageSize</c> defaults to 1 000).
    /// </summary>
    public class Paging
    {
        /// <summary>Current page number (1-based).</summary>
        public int Page { get; set; }

        /// <summary>Total number of pages available for the current query.</summary>
        public int PageCount { get; set; }

        /// <summary>Number of records returned per page.</summary>
        public int PageSize { get; set; }

        /// <summary>Total number of records matching the query across all pages.</summary>
        public int Total { get; set; }

        /// <summary>Absolute URL to the first page of results.</summary>
        public string FirstUrl { get; set; }

        /// <summary>Absolute URL to the previous page, or <c>null</c> when on the first page.</summary>
        public string PreviousUrl { get; set; }

        /// <summary>Absolute URL to the next page, or <c>null</c> when on the last page.</summary>
        public string NextUrl { get; set; }

        /// <summary>Absolute URL to the last page of results.</summary>
        public string LastUrl { get; set; }
    }
}
