using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;

namespace Billy.Api.Repositories
{
    /// <summary>
    /// Extends <see cref="Base{T,TRoot}"/> with Create and Update operations (POST and PUT).
    /// Inherit from this class when the resource supports reading and writing but not deletion.
    /// </summary>
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
        /// Creates a new resource record via <c>POST /v2/{resource}</c> and returns the
        /// server-assigned version (with populated <c>Id</c>, <c>CreatedTime</c>, etc.).
        /// </summary>
        /// <param name="item">The entity to create. Its <see cref="IEntity.Id"/> must be <c>null</c>.</param>
        /// <returns>The created entity as returned by the API, or <c>null</c> on failure.</returns>
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
        /// Creates a new resource record asynchronously via <c>POST /v2/{resource}</c> and returns
        /// the server-assigned version (with populated <c>Id</c>, <c>CreatedTime</c>, etc.).
        /// </summary>
        /// <param name="item">The entity to create. Its <see cref="IEntity.Id"/> must be <c>null</c>.</param>
        /// <returns>The created entity as returned by the API, or <c>null</c> on failure.</returns>
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
        /// Replaces an existing resource record via <c>PUT /v2/{resource}/:id</c>.
        /// The entity's <see cref="IEntity.Id"/> is used to identify the record to update.
        /// </summary>
        /// <param name="item">The entity with updated values. Must have a non-null <c>Id</c>.</param>
        /// <returns>The updated entity as returned by the API, or <c>null</c> on failure.</returns>
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
        /// Replaces an existing resource record asynchronously via <c>PUT /v2/{resource}/:id</c>.
        /// The entity's <see cref="IEntity.Id"/> is used to identify the record to update.
        /// </summary>
        /// <param name="item">The entity with updated values. Must have a non-null <c>Id</c>.</param>
        /// <returns>The updated entity as returned by the API, or <c>null</c> on failure.</returns>
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
        /// Partially updates an existing resource record via <c>PUT /v2/{resource}/:id</c>,
        /// sending only the fields tracked in <paramref name="item"/>. Use this to avoid
        /// overwriting fields you did not intend to change.
        /// </summary>
        /// <param name="id">ID of the record to update.</param>
        /// <param name="item">A <see cref="DeltaObject{T}"/> containing only the changed fields.</param>
        /// <returns>The updated entity as returned by the API, or <c>null</c> on failure.</returns>
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

        /// <summary>
        /// Partially updates an existing resource record asynchronously via <c>PUT /v2/{resource}/:id</c>,
        /// sending only the fields tracked in <paramref name="item"/>. Use this to avoid
        /// overwriting fields you did not intend to change.
        /// </summary>
        /// <param name="id">ID of the record to update.</param>
        /// <param name="item">A <see cref="DeltaObject{T}"/> containing only the changed fields.</param>
        /// <returns>The updated entity as returned by the API, or <c>null</c> on failure.</returns>
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
