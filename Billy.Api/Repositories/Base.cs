using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;
using System.Linq.Expressions;

namespace Billy.Api.Repositories
{
    public abstract class Base<T, TRoot>
        where T : class, IEntity, new()
        where TRoot : class, new()
    {

        protected readonly RestClient client;
        protected readonly Func<TRoot?, T?> rootToSingle;
        protected readonly Func<TRoot?, IList<T>?> rootToMultiple;
        protected readonly Func<T?, string?> itemToId;
        protected readonly string requestUrl;

        private readonly List<Tuple<Func<TRoot, IEnumerable<IEntity>?>, Action<T?, IEntity>, Func<T, string?>>> sideloads = [];


        protected Base(
            RestClient? client,
            string? key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId)
        {
            if (client is null && key is null)
                throw new ArgumentNullException(nameof(client), "Either a client or a key must be provided");

            if (client is not null && key is not null)
                throw new ArgumentException("Either a client or a key must be provided, not both");

            if (client is null)
                this.client = ClientExtensions.CreateBillyClient(key);
            else
                this.client = client;


            //Todo: check for correctly formed url
            this.requestUrl = requestUrl;
            this.rootToSingle = rootToSingle;
            this.rootToMultiple = rootToMultiple;
            this.itemToId = itemToId;
        }

        public Base(
            RestClient client,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId) : this (client, null, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }

        public Base(
            string key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId) : this (null, key, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }


        public void AddSideload<S>( Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            var listGetter = ReflectionExtensions.CreateListGetter<T, TRoot, S>(itemProperty);
            AddSideload(listGetter, itemProperty);
        }

        /// <summary>
        /// Add a sideloaded object to the repository, which will be loaded when the main object is loaded.
        /// Expects the sideloaded object id to be available in the property [name]Id on the main object.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="sideloadedObject"></param>
        /// <param name="itemProperty"></param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            var idGetter = ReflectionExtensions.CreateIdGetter(itemProperty);
            AddSideload(sideloadedObject, itemProperty, idGetter);
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty, Func<T, string?> sideloadId) where S : class, IEntity, new()
        {            
            var setter = ReflectionExtensions.GetSetter(itemProperty);
            AddSideload(sideloadedObject, setter, sideloadId);
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Action<T, S> itemProperty, Func<T, string?> sideloadId) where S : class, IEntity, new()
        {
            sideloads.Add(new Tuple<Func<TRoot, IEnumerable<IEntity>?>, Action<T?, IEntity>, Func<T, string?>>(sideloadedObjects, (b, i) =>
            {
                if (b is null || i is null)
                    return;
                else if (i is S s)
                    itemProperty(b, s);
                else
                    throw new ArgumentOutOfRangeException(nameof(itemProperty), "The itemProperty must be of the same type as the sideloaded object");
            }, sideloadId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public T? Get(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Get)
            {
                RequestFormat = DataFormat.Json
            };

            var root = client.Get<TRoot>(request);

            if (root is null)
                return null;

            var item = rootToSingle(root);
            LoadSideloads(root, item);

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public async Task<T?> GetAsync(string id)
        {
            var request = new RestRequest(requestUrl + id, Method.Get)
            {
                RequestFormat = DataFormat.Json
            };

            var root = await client.GetAsync<TRoot>(request);

            if (root is null)
                return null;

            var item = rootToSingle(root);
            LoadSideloads(root, item);

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<T>? List() => List(null, null, SortOrder.ASC, null, null);

        public IList<T>? List(object filter) => List(filter, null, SortOrder.ASC, null, null);

        public IList<T>? List(object filter, int page, int pageSize) => List(filter, null, SortOrder.ASC, page, pageSize);

        /// <summary>
        /// Lists the invoices sorted by the sort property, which is specified as an expression 
        /// 
        /// </summary>
        /// <code>
        /// List(m => m.dueDate, SortOrder.ASC);
        /// </code>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public IList<T>? List(Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => List(null, sortProperty.GetName(), sortOrder, null, null);

        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => List(filter, sortProperty.GetName(), sortOrder, null, null);

        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => List(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        public IList<T>? List(object? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize)
        {
            var request = new RestRequest(requestUrl, Method.Get);
            if (sortProperty != null)
                request.AddSorting(sortProperty, sortOrder);
            if (filter != null)
                request.AddFilter(filter);
            if (page != null && pageSize != null)
                request.AddPaging(page, pageSize);

            request.RequestFormat = DataFormat.Json;

            var root = client.Get<TRoot>(request);

            if (root is null)
                return null;

            var items = rootToMultiple(root);
            if (items is null)
                return null;

            foreach (var item in items)
            {
                LoadSideloads(root, item);
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IList<T>?> ListAsync() => ListAsync(null, null, SortOrder.ASC, null, null);

        public Task<IList<T>?> ListAsync(object filter) => ListAsync(filter, null, SortOrder.ASC, null, null);

        public Task<IList<T>?> ListAsync(object filter, int page, int pageSize) => ListAsync(filter, null, SortOrder.ASC, page, pageSize);

        /// <summary>
        /// Lists the invoices sorted by the sort property, which is specified as an expression 
        /// 
        /// </summary>
        /// <code>
        /// List(m => m.dueDate, SortOrder.ASC);
        /// </code>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public Task<IList<T>?> ListAsync(Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(null, sortProperty.GetName(), sortOrder, null, null);

        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, null);

        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        public async Task<IList<T>?> ListAsync(object? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize)
        {
            var request = new RestRequest(requestUrl, Method.Get);
            if (sortProperty != null)
                request.AddSorting(sortProperty, sortOrder);
            if (filter != null)
                request.AddFilter(filter);
            if (page != null && pageSize != null)
                request.AddPaging(page, pageSize);

            request.RequestFormat = DataFormat.Json;

            var root = await client.GetAsync<TRoot>(request);

            if (root is null)
                return null;

            var items = rootToMultiple(root);
            if (items is null)
                return null;

            foreach (var item in items)
            {
                LoadSideloads(root, item);
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Create(T item)
        {
            var request = new RestRequest(requestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(item);

            var result = client.Post<TRoot>(request);
            return itemToId(rootToMultiple(result)?[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string?> CreateAsync(T item)
        {
            var request = new RestRequest(requestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(item);

            var result = await client.PostAsync<TRoot>(request);
            return itemToId(rootToMultiple(result)?[0]);
        }



        private void LoadSideloads(TRoot root, T? item)
        {
            if (root is null)
                return;

            if (item is null)
                return;

            foreach (var sideload in sideloads)
            {
                var sideloadItemId = sideload.Item3(item);
                if (sideloadItemId is null)
                    continue;

                var sideloaded = sideload.Item1(root)?.FirstOrDefault((i) => i.Id == sideloadItemId);

                if (sideloaded is null)
                    continue;

                sideload.Item2(item, sideloaded);
            }
        }
    }
}
