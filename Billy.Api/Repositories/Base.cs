using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;
using System.Linq.Expressions;
using System.Text.Json;

namespace Billy.Api.Repositories
{
    public abstract partial class Base<T, TRoot>
        where T : class, IEntity
        where TRoot : class, new()
    {

        protected readonly RestClient client;
        protected virtual Func<TRoot?, T?> RootToSingle { get; private set; }

        protected virtual Func<TRoot?, IList<T>?> RootToMultiple { get; private set; }

        protected virtual string RequestUrl { get; private set; }

        private readonly List<SideloadDescriptor> sideloads = [];


        protected Base(
            RestClient? client,
            string? key)
        {
            if (client is null && key is null)
                throw new ArgumentNullException(nameof(client), "Either a client or a key must be provided");

            if (client is not null && key is not null)
                throw new ArgumentException("Either a client or a key must be provided, not both");

            if (client is null)
                this.client = ClientExtensions.CreateBillyClient(key);
            else
                this.client = client;


            if (string.IsNullOrWhiteSpace(RequestUrl))
                RequestUrl = $"{JsonNamingPolicy.CamelCase.ConvertName(this.GetType().Name)}/";

            RootToMultiple ??= BaseHelpers<T, TRoot>.CompileDefaultMultiple(this.GetType().Name);
            RootToSingle ??= BaseHelpers<T, TRoot>.CompileDefaultSingle();
        }

        public Base(
            RestClient client) : this(client, null)
        {
        }

        public Base(
            string key) : this(null, key)
        {
        }

        public void AddSideload<S>(Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            AddSideload(itemProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(itemProperty.GetPropertyName()));
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
            AddSideload(sideloadedObject, itemProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(itemProperty.GetPropertyName()));
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
            AddSideload(sideloadedObjects, itemsProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(itemsProperty.GetPropertyName()));
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
            var request = new RestRequest(RequestUrl + id, Method.Get)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddIncludes(this.sideloads);

            var root = client.Get<TRoot>(request);

            if (root is null)
                return null;

            var item = RootToSingle(root);
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
            var request = new RestRequest(RequestUrl + id, Method.Get)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddIncludes(this.sideloads);

            var root = await client.GetAsync<TRoot>(request);

            if (root is null)
                return null;

            var item = RootToSingle(root);
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
            var request = new RestRequest(RequestUrl, Method.Get);
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

            var items = RootToMultiple(root);
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
            var request = new RestRequest(RequestUrl, Method.Get);
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

            var items = RootToMultiple(root);
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
        public string? Create(T item)
        {
            var request = new RestRequest(RequestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));

            var result = client.Post<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Creates a new item using asynchronous operations.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string?> CreateAsync(T item)
        {
            var request = new RestRequest(RequestUrl, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));

            var result = await client.PostAsync<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Update(T item)
        {
            var request = new RestRequest(RequestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));
            var result = client.Put<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item using asynchronous operations. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<string?> UpdateAsync(T item)
        {
            var request = new RestRequest(RequestUrl + item.Id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBodyWithSharedOptions(SingleToRoot(item));
            var result = await client.PutAsync<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

        /// <summary>
        /// Updates an existing item. The item must have a valid Id property, which is used to identify the item to update.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Update(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(RequestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = client.Put<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

        public async Task<string?> UpdateAsync(string id, DeltaObject<T> item)
        {
            var request = new RestRequest(RequestUrl + id, Method.Put)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddUpdateBodyWithSharedOptions(item);
            var result = await client.PutAsync<TRoot>(request);
            return RootToMultiple(result)?[0].Id;
        }

    }
}
