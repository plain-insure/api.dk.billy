using RestSharp;

namespace Billy.Api.Repositories
{
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
        /// Deletes the entity with the specified id and returns id of the deleted entity.
        /// </summary>
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
        /// Deletes the entity with the specified id and returns id of the deleted entity.
        /// </summary>
        public async Task<string?> DeleteAsync(string id)
        {
            TRoot? response = await DeleteRawAsync(id);
            if (response?.Meta?.DeletedRecords is null)
                return null;

            if (!response.Meta.DeletedRecords.TryGetValue(JsonNamePlural, out var deletedId))
                return null;

            return deletedId.FirstOrDefault();
        }

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
        /// Deletes the entities with the specified ids and returns the ids of the deleted entities.
        /// </summary>
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
        /// Deletes the entities with the specified ids and returns the ids of the deleted entities.
        /// </summary>
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