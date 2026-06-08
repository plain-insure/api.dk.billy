using RestSharp;

namespace Billy.Api.Repositories
{
    public abstract class BaseWithDelete<T, TRoot> : BaseWithCreate<T, TRoot>
       where T : class, IEntity
       where TRoot : class, new()
    {
        private readonly Func<TRoot?, string?> DeletedToString;

        public BaseWithDelete(
            RestClient? client, string? key) : base(client, key)
        {
            DeletedToString ??= BaseHelpers<T, TRoot>.CompileDeletedToString(typeof(T));
        }
        public BaseWithDelete(
            RestClient client) : this(client, null)
        {
        }
        public BaseWithDelete(
            string key) : this(null, key)
        {
        }

        public string? Delete(string id)
        {
            var request = new RestRequest(RequestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Delete<TRoot>(request);
            if (response == null)
                return null;

            return DeletedToString(response);
        }

        public async Task<string?> DeleteAsync(string id)
        {
            var request = new RestRequest(RequestUrl + id, Method.Delete)
            {
                RequestFormat = DataFormat.Json
            };

            var response = await client.DeleteAsync<TRoot>(request);
            if (response == null)
                return null;

            return DeletedToString(response);
        }
    }
}