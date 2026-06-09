using RestSharp;

namespace Billy.Api.Repositories
{
    /// <summary>
    /// Extends <see cref="BaseWithCreate{T,TRoot}"/> with Delete operations (DELETE).
    /// Inherit from this class when the resource supports full CRUD.
    /// </summary>
    public abstract class BaseWithDelete<T, TRoot>(
        RestClient? client, string? key) : BaseWithCreate<T, TRoot>(client, key)
       where T : class, IEntity
       where TRoot : Models.Root, new()
    {
        public BaseWithDelete(
            RestClient client) : this(client, null)
        {
        }

        public BaseWithDelete(
            string key) : this(null, key)
        {
        }

        /// <summary>
        /// Deletes the entity with the specified ID via <c>DELETE /v2/{resource}/:id</c>
        /// and returns the ID of the deleted record as confirmed by the API.
        /// </summary>
        /// <param name="id">ID of the record to delete.</param>
        /// <returns>The deleted record's ID, or <c>null</c> if the API did not confirm the deletion.</returns>
        public string? Delete(string id)
        {
            var request = new RestRequest(RequestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Delete<TRoot>(request);

            if (response?.Meta?.DeletedRecords is null)
                return null;

            if (!response.Meta.DeletedRecords.TryGetValue(JsonNamePlural, out var deletedId))
                return null;

            return deletedId.FirstOrDefault();
        }

        /// <summary>
        /// Deletes the entity with the specified ID asynchronously via <c>DELETE /v2/{resource}/:id</c>
        /// and returns the ID of the deleted record as confirmed by the API.
        /// </summary>
        /// <param name="id">ID of the record to delete.</param>
        /// <returns>The deleted record's ID, or <c>null</c> if the API did not confirm the deletion.</returns>
        public async Task<string?> DeleteAsync(string id)
        {
            TRoot? response = await DeleteRawAsync(id);
            if (response?.Meta?.DeletedRecords is null)
                return null;

            if (!response.Meta.DeletedRecords.TryGetValue(JsonNamePlural, out var deletedId))
                return null;

            return deletedId.FirstOrDefault();
        }

        /// <summary>
        /// Deletes the entity with the specified ID asynchronously and returns the raw API response envelope,
        /// including any sideloaded changes triggered by the deletion.
        /// </summary>
        /// <param name="id">ID of the record to delete.</param>
        /// <returns>The raw <typeparamref name="TRoot"/> response, or <c>null</c> on failure.</returns>
        public async Task<TRoot?> DeleteRawAsync(string id)
        {
            var request = new RestRequest(RequestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = await client.DeleteAsync<TRoot>(request);
            return response;
        }

        /// <summary>
        /// Deletes multiple entities in a single request via <c>DELETE /v2/{resource}?ids[]=…</c>
        /// and returns the IDs of all deleted records as confirmed by the API.
        /// </summary>
        /// <param name="ids">IDs of the records to delete.</param>
        /// <returns>IDs of the records that were deleted.</returns>
        public IEnumerable<string> DeleteBulk(string[] ids)
        {
            var request = new RestRequest(RequestUrl, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };
            foreach (var id in ids)
                request.AddQueryParameter("ids[]", id);

            var response = client.Delete<TRoot>(request);
            if (response?.Meta?.DeletedRecords is null)
                return [];

            if (!response.Meta.DeletedRecords.TryGetValue(JsonNamePlural, out var deletedId))
                return [];

            return deletedId;
        }

        /// <summary>
        /// Deletes multiple entities asynchronously in a single request via
        /// <c>DELETE /v2/{resource}?ids[]=…</c> and returns the IDs of all deleted records.
        /// </summary>
        /// <param name="ids">IDs of the records to delete.</param>
        /// <returns>IDs of the records that were deleted.</returns>
        public async Task<IEnumerable<string>> DeleteBulkAsync(string[] ids)
        {
            var request = new RestRequest(RequestUrl, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };
            foreach (var id in ids)
                request.AddQueryParameter("ids[]", id);

            var response = await client.DeleteAsync<TRoot>(request);

            if (response?.Meta?.DeletedRecords is null)
                return [];

            if (!response.Meta.DeletedRecords.TryGetValue(JsonNamePlural, out var deletedId))
                return [];

            return deletedId;
        }
    }
}
