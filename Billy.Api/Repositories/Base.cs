using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;
using System.Linq.Expressions;

namespace Billy.Api.Repositories
{
    public abstract class Base<T, TRoot>
        where T : class, IEntity
        where TRoot : class, new()
    {

        protected readonly RestClient client;
        protected readonly Func<TRoot?, T?> rootToSingle;
        protected readonly Func<TRoot?, IList<T>?> rootToMultiple;
        protected readonly string requestUrl;

        internal abstract class SideloadDescriptor(string includeProperty)
        {

            public string IncludeProperty { get; set; } = includeProperty;

            internal abstract void ApplyAll(TRoot root, IList<T> items);
        }

        private sealed class SingleSideloadDescriptor<S>(
            Func<TRoot, IEnumerable<S>?> getCandidates,
            Func<T, string?> getId,
            Action<T, S> apply, string includeProperty) : SideloadDescriptor(includeProperty)
            where S : class, IEntity
        {
            internal override void ApplyAll(TRoot root, IList<T> items)
            {
                var dict = getCandidates(root)
                    ?.Where(s => s.Id is not null)
                    .ToDictionary(s => s.Id!) ?? [];
                foreach (var item in items)
                {
                    var id = getId(item);
                    if (id is not null && dict.TryGetValue(id, out var match))
                        apply(item, match);
                }
            }
        }

        private sealed class ListSideloadDescriptor<S>(
            Func<TRoot, IEnumerable<S>?> getCandidates,
            Func<T, IEnumerable<string>?> getIds,
            Action<T, S> append,
            string includeProperty) : SideloadDescriptor(includeProperty)
            where S : class, IEntity
        {
            internal override void ApplyAll(TRoot root, IList<T> items)
            {
                var dict = getCandidates(root)
                    ?.Where(s => s.Id is not null)
                    .ToDictionary(s => s.Id!) ?? [];
                foreach (var item in items)
                {
                    var ids = getIds(item);
                    if (ids is null) continue;
                    foreach (var id in ids)
                        if (dict.TryGetValue(id, out var match))
                            append(item, match);
                }
            }
        }

        private readonly List<SideloadDescriptor> sideloads = [];


        protected Base(
            RestClient? client,
            string? key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple)
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
        }

        public Base(
            RestClient client,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple) : this(client, null, requestUrl, rootToSingle, rootToMultiple)
        {
        }

        public Base(
            string key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple) : this(null, key, requestUrl, rootToSingle, rootToMultiple)
        {
        }


        private static string DeriveIncludeProperty(string propertyName)
        {
            var model = typeof(T).Name;
            return $"{char.ToLower(model[0])}{model[1..]}.{char.ToLower(propertyName[0])}{propertyName[1..]}";
        }

        public void AddSideload<S>(Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            AddSideload(itemProperty, DeriveIncludeProperty(itemProperty.GetPropertyName()));
        }

        public void AddSideload<S>(Expression<Func<T, S?>> itemProperty, string includeProperty) where S : class, IEntity, new()
        {
            var listGetter = ReflectionExtensions.CreateListGetter<T, TRoot, S>(itemProperty);
            AddSideload(listGetter, itemProperty, includeProperty);
        }

        /// <summary>
        /// Add a sideloaded object to the repository, which will be loaded when the main object is loaded.
        /// Expects the sideloaded object id to be available in the property [name]Id on the main object.
        /// </summary>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            AddSideload(sideloadedObject, itemProperty, DeriveIncludeProperty(itemProperty.GetPropertyName()));
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty, string includeProperty) where S : class, IEntity, new()
        {
            var idGetter = ReflectionExtensions.CreateSingleIdGetter(itemProperty);
            AddSideload(sideloadedObject, itemProperty, idGetter, includeProperty);
        }

        /// <summary>
        /// Add a sideloaded collection to the repository. Discovers the IDs property by singularizing
        /// the collection property name and appending "Ids" (e.g. Lines → LineIds).
        /// </summary>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Expression<Func<T, IEnumerable<S>?>> itemsProperty) where S : class, IEntity, new()
        {
            AddSideload(sideloadedObjects, itemsProperty, DeriveIncludeProperty(itemsProperty.GetPropertyName()));
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Expression<Func<T, IEnumerable<S>?>> itemsProperty, string includeProperty) where S : class, IEntity, new()
        {
            var idsGetter = ReflectionExtensions.CreateListIdGetter(itemsProperty);
            var appender = ReflectionExtensions.GetCollectionAppender(itemsProperty);
            sideloads.Add(new ListSideloadDescriptor<S>(sideloadedObjects, idsGetter, appender, includeProperty));
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty, Func<T, string?> sideloadId, string includeProperty) where S : class, IEntity, new()
        {
            var setter = ReflectionExtensions.GetSetter(itemProperty);
            AddSideload(sideloadedObject, setter, sideloadId, includeProperty);
        }

        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Action<T, S> setter, Func<T, string?> idGetter, string includeProperty) where S : class, IEntity, new()
        {
            sideloads.Add(new SingleSideloadDescriptor<S>(sideloadedObjects, idGetter, setter, includeProperty));
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

            request.AddIncludes(this.sideloads);

            var root = client.Get<TRoot>(request);

            if (root is null)
                return null;

            var item = rootToSingle(root);
            ApplyAllSideloads(root, item is null ? [] : [item]);

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

            request.AddIncludes(this.sideloads);

            var root = await client.GetAsync<TRoot>(request);

            if (root is null)
                return null;

            var item = rootToSingle(root);
            ApplyAllSideloads(root, item is null ? [] : [item]);

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

            request.AddIncludes(this.sideloads);

            var root = client.Get<TRoot>(request);

            if (root is null)
                return null;

            var items = rootToMultiple(root);
            if (items is null)
                return null;

            ApplyAllSideloads(root, items);
            return items;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Task<IList<T>?> ListAsync() => DoListAsync(null, null, SortOrder.ASC, null, null);

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
        public Task<IList<T>?> ListAsync(Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => DoListAsync(filterDict: null, sortProperty.GetName(), sortOrder, null, null);

        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, null);

        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, null);

        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        public Task<IList<T>?> ListAsync(object? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize) => DoListAsync(filter?.AsDictionary(), sortProperty, sortOrder, page, pageSize);

        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize) => DoListAsync(filter?.GetModifications(), sortProperty, sortOrder, page, pageSize);

        public Task<IList<T>?> ListAsync(IDictionary<string, object?>? filterDict, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize) => DoListAsync(filterDict, sortProperty, sortOrder, page, pageSize);

        private async Task<IList<T>?> DoListAsync(IDictionary<string, object?>? filterDict, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize)
        {
            var request = new RestRequest(requestUrl, Method.Get);
            if (sortProperty != null)
                request.AddSorting(sortProperty, sortOrder);
            if (filterDict != null)
                request.AddFilter(filterDict: filterDict);
            if (page != null && pageSize != null)
                request.AddPaging(page, pageSize);

            request.RequestFormat = DataFormat.Json;

            request.AddIncludes(this.sideloads);

            var root = await client.GetAsync<TRoot>(request);

            if (root is null)
                return null;

            var items = rootToMultiple(root);
            if (items is null)
                return null;

            ApplyAllSideloads(root, items);
            return items;
        }




        private void ApplyAllSideloads(TRoot root, IList<T> items)
        {
            foreach (var descriptor in sideloads)
                descriptor.ApplyAll(root, items);
        }
    }

    public abstract class BaseWithCreate<T, TRoot> : Base<T, TRoot>
        where T : class, IEntity
        where TRoot : class, new()
    {
        protected readonly Func<T, TRoot> singleToRoot;


        protected BaseWithCreate(RestClient? client, string? key, string requestUrl, Func<TRoot?, T?> rootToSingle, Func<TRoot?, IList<T>?> rootToMultiple, Func<T, TRoot> singleToRoot) :
            base(client, key, requestUrl, rootToSingle, rootToMultiple)
        {
            this.singleToRoot = singleToRoot;
        }
        protected BaseWithCreate(RestClient client, string requestUrl, Func<TRoot?, T?> rootToSingle, Func<TRoot?, IList<T>?> rootToMultiple, Func<T, TRoot> singleToRoot) :
            base(client, null, requestUrl, rootToSingle, rootToMultiple)
        {
            this.singleToRoot = singleToRoot;
        }
        protected BaseWithCreate(string key, string requestUrl, Func<TRoot?, T?> rootToSingle, Func<TRoot?, IList<T>?> rootToMultiple, Func<T, TRoot> singleToRoot) :
            base(null, key, requestUrl, rootToSingle, rootToMultiple)
        {
            this.singleToRoot = singleToRoot;
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Create(T item)
        {
            var request = new RestRequest(requestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(singleToRoot(item));

            var result = client.Post<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Creates a new item using asynchronous operations.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string?> CreateAsync(T item)
        {
            var request = new RestRequest(requestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(singleToRoot(item));

            var result = await client.PostAsync<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Update(T item)
        {
            var request = new RestRequest(requestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(singleToRoot(item));
            var result = client.Put<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item using asynchronous operations. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string?> UpdateAsync(T item)
        {
            var request = new RestRequest(requestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(singleToRoot(item));
            var result = await client.PutAsync<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Update(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(requestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = client.Put<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

        public async Task<string?> UpdateAsync(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(requestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = await client.PutAsync<TRoot>(request);
            return rootToMultiple(result)?[0].Id;
        }

    }
}
