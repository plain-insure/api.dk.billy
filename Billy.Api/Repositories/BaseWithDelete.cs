using RestSharp;

namespace Billy.Api.Repositories
{
    public abstract class BaseWithDelete<T, TRoot> : Base<T, TRoot>
       where T : class, IEntity, new()
       where TRoot : class, new()
    {
        public BaseWithDelete(
            RestClient? client, string? key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId) : base(client, key, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }

        public T Delete(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Delete<TRoot>(request);
            return rootToSingle(response);
        }

        public async Task<T> DeleteAsync(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = await  client.DeleteAsync<TRoot>(request);
            return rootToSingle(response);
        }
    }
}