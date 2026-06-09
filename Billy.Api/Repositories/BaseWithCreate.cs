using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;

namespace Billy.Api.Repositories
{
    public abstract class BaseWithCreate<T, TRoot> : Base<T, TRoot>
        where T : class, IEntity
        where TRoot : Root, new()
    {
        protected virtual Func<T, TRoot> SingleToRoot { get; private set; }


        protected BaseWithCreate(RestClient? client, string? key) :
            base(client, key)
        {
            SingleToRoot ??= BaseHelpers<T, TRoot>.CompileSingleToRoot();
        }
        protected BaseWithCreate(RestClient client) :
            this(client, null)
        {
        }
        protected BaseWithCreate(string key) :
            this(null, key)
        {
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T? Create(T item)
        {
            var request = new RestRequest(RequestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));

            var result = client.Post<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

        /// <summary>
        /// Creates a new item using asynchronous operations.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<T?> CreateAsync(T item)
        {
            var request = new RestRequest(RequestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));

            var result = await client.PostAsync<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T? Update(T item)
        {
            var request = new RestRequest(RequestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));
            var result = client.Put<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

        /// <summary>
        /// Updates an existing item using asynchronous operations. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<T?> UpdateAsync(T item)
        {
            var request = new RestRequest(RequestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));
            var result = await client.PutAsync<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public T? Update(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(RequestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = client.Put<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

        public async Task<T?> UpdateAsync(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(RequestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = await client.PutAsync<TRoot>(request);
            return RootToMultiple(result)?.FirstOrDefault();
        }

    }
}
