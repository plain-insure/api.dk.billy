using RestSharp;

namespace BillyService.Repositories
{
    public abstract class BaseWithDelete<T, TRoot> : Base<T, TRoot>
        where T : class, new()
        where TRoot : class, new()
    {

        public BaseWithDelete(
           RestSharp.RestClient client,
           string requestUrl,
           Func<TRoot?, T?> rootToSingle,
           Func<TRoot?, IList<T>?> rootToMultiple,
           Func<T?, string?> itemToId) : base(client, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }

        public BaseWithDelete(
           string key,
           string requestUrl,
           Func<TRoot?, T?> rootToSingle,
           Func<TRoot?, IList<T>?> rootToMultiple,
           Func<T?, string?> itemToId) : base(key, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }

        public bool Delete(string id)
        {
            try
            {
                var request = new RestRequest(requestUrl + id, Method.Delete)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Delete<TRoot>(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}