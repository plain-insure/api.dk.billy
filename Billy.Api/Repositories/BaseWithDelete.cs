using RestSharp;

namespace Billy.Api.Repositories
{
    public abstract class BaseWithDelete<T, TRoot> : BaseWithCreate<T, TRoot>
       where T : class, IEntity
       where TRoot : class, new()
    {
        private readonly Func<TRoot?, string?> deletedToString;

        public BaseWithDelete(
            RestClient? client, string? key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId,
            Func<T, TRoot> singleToRoot,
            Func<TRoot?, string?> deletedToString) : base(client, key, requestUrl, rootToSingle, rootToMultiple, itemToId, singleToRoot)
        {
            this.deletedToString = deletedToString;
        }
        public BaseWithDelete(
            RestClient client,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId,
            Func<T, TRoot> singleToRoot,
            Func<TRoot?, string?> deletedToString) : base(client, requestUrl, rootToSingle, rootToMultiple, itemToId, singleToRoot)
        {
            this.deletedToString = deletedToString;
        }
        public BaseWithDelete(
            string key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId,
            Func<T, TRoot> singleToRoot,
            Func<TRoot?, string?> deletedToString) : base(key, requestUrl, rootToSingle, rootToMultiple, itemToId, singleToRoot)
        {
            this.deletedToString = deletedToString;
        }

        public string Delete(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Delete<TRoot>(request);

            return deletedToString(response);
        }

        public async Task<string> DeleteAsync(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = await client.DeleteAsync<TRoot>(request);
            return deletedToString(response);
        }
    }
}