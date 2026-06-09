using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json;

namespace Billy.Api.Repositories
{
    /// <summary>
    /// Base repository providing read-only access (Get and List) to a Billy API resource.
    /// All other repository base classes inherit from this one.
    /// </summary>
    /// <typeparam name="T">Entity type (e.g. <see cref="Bill"/>).</typeparam>
    /// <typeparam name="TRoot">API response envelope type (e.g. <see cref="BillRoot"/>).</typeparam>
    public abstract partial class Base<T, TRoot>
        where T : class, IEntity
        where TRoot : Root, new()
    {

        protected readonly RestClient client;
        protected virtual Func<TRoot?, T?> RootToSingle { get; private set; }

        protected virtual Func<TRoot?, IList<T>?> RootToMultiple { get; private set; }

        protected virtual string RequestUrl { get; private set; }

        protected virtual string JsonNameSingular { get; private set;  }

        protected virtual string JsonNamePlural { get; private set; }

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
                this.client = ClientExtensions.CreateBillyClient(key!);
            else
                this.client = client;

            var singularName = typeof(T).Name;
            var pluralName = this.GetType().Name;

            if (string.IsNullOrWhiteSpace(JsonNameSingular))
                JsonNameSingular = JsonNamingPolicy.CamelCase.ConvertName(singularName);

            if (string.IsNullOrWhiteSpace(JsonNamePlural))
                JsonNamePlural = JsonNamingPolicy.CamelCase.ConvertName(this.GetType().Name);

            if (string.IsNullOrWhiteSpace(RequestUrl))
                RequestUrl = $"{JsonNamePlural}/";

            RootToMultiple ??= BaseHelpers<T, TRoot>.CompileDefaultMultiple(this.GetType().Name);
            RootToSingle ??= BaseHelpers<T, TRoot>.CompileDefaultSingle();
        }

        /// <summary>
        /// Initializes the repository with a shared <see cref="RestClient"/>.
        /// Use this constructor when sharing one client across multiple repositories (recommended for DI).
        /// </summary>
        /// <param name="client">
        /// A <see cref="RestClient"/> pre-configured for the Billy API.
        /// Create one via <see cref="ClientExtensions.CreateBillyClient(string)"/>.
        /// </param>
        public Base(
            RestClient client) : this(client, null)
        {
        }

        /// <summary>
        /// Initializes the repository with a Billy API access token.
        /// A new <see cref="RestClient"/> is created internally and owned by this instance.
        /// </summary>
        /// <param name="key">Billy API access token from Settings → Access tokens in mit.billy.dk.</param>
        public Base(
            string key) : this(null, key)
        {
        }

        /// <summary>
        /// Registers a single-object sideload using convention-based discovery.
        /// The root list property is found by appending <c>s</c> to the entity property name
        /// (e.g. <c>item.Country</c> → root.<c>Countries</c>), and the include key is derived as
        /// <c>{entityName}.{propertyName}</c> (e.g. <c>"contact.country"</c>).
        /// The ID property is resolved by appending <c>Id</c> to the property name on <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="itemProperty">Expression pointing to the navigation property on <typeparamref name="T"/> to populate.</param>
        public void AddSideload<S>(Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            AddSideload(itemProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(JsonNameSingular, itemProperty.GetPropertyName()));
        }

        /// <summary>
        /// Registers a single-object sideload with a custom Billy API include key.
        /// The root list property and ID property are discovered by convention (same rules as
        /// <see cref="AddSideload{S}(Expression{Func{T, S}})"/>).
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="itemProperty">Expression pointing to the navigation property on <typeparamref name="T"/> to populate.</param>
        /// <param name="includeProperty">
        /// Billy API include parameter value (e.g. <c>"contact.country"</c>).
        /// Passed as the <c>include</c> query parameter to request sideloaded data.
        /// </param>
        public void AddSideload<S>(Expression<Func<T, S?>> itemProperty, string includeProperty) where S : class, IEntity, new()
        {
            var listGetter = ReflectionExtensions.CreateListGetter<T, TRoot, S>(itemProperty);
            AddSideload(listGetter, itemProperty, includeProperty);
        }

        /// <summary>
        /// Registers a single-object sideload with an explicit root list accessor.
        /// The include key is derived by convention as <c>{entityName}.{propertyName}</c>.
        /// The ID property on <typeparamref name="T"/> is resolved by convention (property name + <c>Id</c>).
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="sideloadedObject">Function that extracts the list of candidates from the API root response.</param>
        /// <param name="itemProperty">Expression pointing to the navigation property on <typeparamref name="T"/> to populate.</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty) where S : class, IEntity, new()
        {
            AddSideload(sideloadedObject, itemProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(JsonNameSingular, itemProperty.GetPropertyName()));
        }

        /// <summary>
        /// Registers a single-object sideload with an explicit root list accessor and include key.
        /// The ID property on <typeparamref name="T"/> is resolved by convention (property name + <c>Id</c>).
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="sideloadedObject">Function that extracts the list of candidates from the API root response.</param>
        /// <param name="itemProperty">Expression pointing to the navigation property on <typeparamref name="T"/> to populate.</param>
        /// <param name="includeProperty">Billy API include parameter value (e.g. <c>"bill.contact"</c>).</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty, string includeProperty) where S : class, IEntity, new()
        {
            var idGetter = ReflectionExtensions.CreateSingleIdGetter(itemProperty);
            AddSideload(sideloadedObject, itemProperty, idGetter, includeProperty);
        }

        /// <summary>
        /// Registers a collection sideload using convention-based discovery.
        /// The IDs list on <typeparamref name="T"/> is found by singularizing the collection property name
        /// and appending <c>Ids</c> (e.g. <c>Lines</c> → <c>LineIds</c>).
        /// The include key is derived as <c>{entityName}.{propertyName}</c>.
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded child entity.</typeparam>
        /// <param name="sideloadedObjects">Function that extracts the flat list of candidates from the API root response.</param>
        /// <param name="itemsProperty">Expression pointing to the collection navigation property on <typeparamref name="T"/> to populate.</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Expression<Func<T, IEnumerable<S>?>> itemsProperty) where S : class, IEntity, new()
        {
            AddSideload(sideloadedObjects, itemsProperty, BaseHelpers<T, TRoot>.DeriveIncludeProperty(JsonNameSingular, itemsProperty.GetPropertyName()));
        }

        /// <summary>
        /// Registers a collection sideload with a custom include key.
        /// The IDs list on <typeparamref name="T"/> is found by convention (singularized property name + <c>Ids</c>).
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded child entity.</typeparam>
        /// <param name="sideloadedObjects">Function that extracts the flat list of candidates from the API root response.</param>
        /// <param name="itemsProperty">Expression pointing to the collection navigation property on <typeparamref name="T"/> to populate.</param>
        /// <param name="includeProperty">Billy API include parameter value (e.g. <c>"bill.lines"</c>).</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Expression<Func<T, IEnumerable<S>?>> itemsProperty, string includeProperty) where S : class, IEntity, new()
        {
            var idsGetter = ReflectionExtensions.CreateListIdGetter(itemsProperty);
            var appender = ReflectionExtensions.GetCollectionAppender(itemsProperty);
            sideloads.Add(new ListSideloadDescriptor<S>(sideloadedObjects, idsGetter, appender, includeProperty));
        }

        /// <summary>
        /// Registers a single-object sideload with explicit root list accessor, ID getter, and include key.
        /// Use this overload when convention-based discovery cannot find the right property.
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="sideloadedObject">Function that extracts the list of candidates from the API root response.</param>
        /// <param name="itemProperty">Expression pointing to the navigation property on <typeparamref name="T"/> to populate.</param>
        /// <param name="sideloadId">Function that returns the foreign key value from an entity instance.</param>
        /// <param name="includeProperty">Billy API include parameter value.</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObject, Expression<Func<T, S?>> itemProperty, Func<T, string?> sideloadId, string includeProperty) where S : class, IEntity, new()
        {
            var setter = ReflectionExtensions.GetSetter(itemProperty);
            AddSideload(sideloadedObject, setter, sideloadId, includeProperty);
        }

        /// <summary>
        /// Registers a single-object sideload with fully explicit root list accessor, setter, ID getter,
        /// and include key. This is the most flexible overload; all others ultimately delegate here.
        /// </summary>
        /// <typeparam name="S">Type of the sideloaded entity.</typeparam>
        /// <param name="sideloadedObjects">Function that extracts the list of candidates from the API root response.</param>
        /// <param name="setter">Action that assigns the resolved sideloaded object to the entity.</param>
        /// <param name="idGetter">Function that returns the foreign key value from an entity instance.</param>
        /// <param name="includeProperty">Billy API include parameter value.</param>
        public void AddSideload<S>(Func<TRoot, IEnumerable<S>?> sideloadedObjects, Action<T, S> setter, Func<T, string?> idGetter, string includeProperty) where S : class, IEntity, new()
        {
            sideloads.Add(new SingleSideloadDescriptor<S>(sideloadedObjects, idGetter, setter, includeProperty));
        }

        /// <summary>
        /// Retrieves a single resource by ID via <c>GET /v2/{resource}/:id</c>.
        /// Any registered sideloads are requested and applied automatically.
        /// </summary>
        /// <param name="id">The unique ID of the resource to retrieve.</param>
        /// <returns>The entity, or <c>null</c> if not found.</returns>        
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
        /// Retrieves a single resource by ID asynchronously via <c>GET /v2/{resource}/:id</c>.
        /// Any registered sideloads are requested and applied automatically.
        /// </summary>
        /// <param name="id">The unique ID of the resource to retrieve.</param>
        /// <returns>The entity, or <c>null</c> if not found.</returns>
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
        /// Lists all resources via <c>GET /v2/{resource}</c>. Returns up to 1 000 records (API default).
        /// Any registered sideloads are requested and applied automatically.
        /// </summary>
        /// <returns>All matching entities, or <c>null</c> on failure.</returns>
        public IList<T>? List() => List(null, null, SortOrder.ASC, null, null);

        /// <summary>
        /// Lists resources matching the given filter via <c>GET /v2/{resource}</c>.
        /// Filter properties are camelCased and sent as query parameters.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        public IList<T>? List(object filter) => List(filter, null, SortOrder.ASC, null, null);

        /// <summary>
        /// Lists a page of resources matching the given filter.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="page">1-based page number to retrieve.</param>
        /// <param name="pageSize">Number of records per page (max 1 000).</param>
        public IList<T>? List(object filter, int page, int pageSize) => List(filter, null, SortOrder.ASC, page, pageSize);

        /// <summary>
        /// Lists all resources sorted by the specified property.
        /// </summary>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        public IList<T>? List(Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => List(null, sortProperty.GetName(), sortOrder, null, null);

        /// <summary>
        /// Lists resources matching the given filter, sorted by the specified property.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => List(filter, sortProperty.GetName(), sortOrder, null, null);

        /// <summary>
        /// Lists a page of resources matching the given filter, sorted by the specified property.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="pageSize">Number of records per page (max 1 000).</param>
        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => List(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        /// <summary>
        /// Lists resources with full control over filter, sort, and paging.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters, or <c>null</c>.</param>
        /// <param name="sortProperty">camelCase property name to sort by, or <c>null</c>.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="page">1-based page number, or <c>null</c> for the first page.</param>
        /// <param name="pageSize">Records per page (max 1 000), or <c>null</c> for the API default.</param>
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
        /// Lists all resources asynchronously via <c>GET /v2/{resource}</c>. Returns up to 1 000 records.
        /// Any registered sideloads are requested and applied automatically.
        /// </summary>
        /// <returns>All matching entities, or <c>null</c> on failure.</returns>
        public Task<IList<T>?> ListAsync() => DoListAsync(null, null, SortOrder.ASC, null, null);

        /// <summary>
        /// Lists resources matching the given filter asynchronously.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        public Task<IList<T>?> ListAsync(object filter) => ListAsync(filter, null, SortOrder.ASC, null, null);

        /// <summary>
        /// Lists a page of resources matching the given filter asynchronously.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="page">1-based page number to retrieve.</param>
        /// <param name="pageSize">Number of records per page (max 1 000).</param>
        public Task<IList<T>?> ListAsync(object filter, int page, int pageSize) => ListAsync(filter, null, SortOrder.ASC, page, pageSize);

        /// <summary>
        /// Lists all resources asynchronously, sorted by the specified property.
        /// </summary>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        public Task<IList<T>?> ListAsync(Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => DoListAsync(filterDict: null, sortProperty.GetName(), sortOrder, null, null);

        /// <summary>
        /// Lists resources matching a <see cref="DeltaObject{T}"/> filter asynchronously, sorted by the specified property.
        /// Only the properties set on the delta object are used as filter criteria.
        /// </summary>
        /// <param name="filter">Delta object whose tracked modifications are used as query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, null);

        /// <summary>
        /// Lists a page of resources matching a <see cref="DeltaObject{T}"/> filter asynchronously.
        /// </summary>
        /// <param name="filter">Delta object whose tracked modifications are used as query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="pageSize">Number of records per page (max 1 000).</param>
        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        /// <summary>
        /// Lists resources matching the given filter asynchronously, sorted by the specified property.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, null);

        /// <summary>
        /// Lists a page of resources matching the given filter asynchronously, sorted by the specified property.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters.</param>
        /// <param name="sortProperty">Expression selecting the property to sort by.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="pageSize">Number of records per page (max 1 000).</param>
        public Task<IList<T>?> ListAsync(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize) => ListAsync(filter, sortProperty.GetName(), sortOrder, null, pageSize);

        /// <summary>
        /// Lists resources asynchronously with full control over filter, sort, and paging.
        /// </summary>
        /// <param name="filter">Anonymous object or dictionary whose properties become query parameters, or <c>null</c>.</param>
        /// <param name="sortProperty">camelCase property name to sort by, or <c>null</c>.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="page">1-based page number, or <c>null</c> for the first page.</param>
        /// <param name="pageSize">Records per page (max 1 000), or <c>null</c> for the API default.</param>
        public Task<IList<T>?> ListAsync(object? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize) => DoListAsync(filter?.AsDictionary(), sortProperty, sortOrder, page, pageSize);

        /// <summary>
        /// Lists resources asynchronously using a <see cref="DeltaObject{T}"/> as the filter,
        /// with full control over sort and paging.
        /// </summary>
        /// <param name="filter">Delta object whose tracked modifications are used as query parameters, or <c>null</c>.</param>
        /// <param name="sortProperty">camelCase property name to sort by, or <c>null</c>.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="page">1-based page number, or <c>null</c> for the first page.</param>
        /// <param name="pageSize">Records per page (max 1 000), or <c>null</c> for the API default.</param>
        public Task<IList<T>?> ListAsync(DeltaObject<T>? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize) => DoListAsync(filter?.GetModifications(), sortProperty, sortOrder, page, pageSize);

        /// <summary>
        /// Lists resources asynchronously using a pre-built filter dictionary,
        /// with full control over sort and paging.
        /// </summary>
        /// <param name="filterDict">Key/value pairs sent as query parameters, or <c>null</c>.</param>
        /// <param name="sortProperty">camelCase property name to sort by, or <c>null</c>.</param>
        /// <param name="sortOrder">Sort direction.</param>
        /// <param name="page">1-based page number, or <c>null</c> for the first page.</param>
        /// <param name="pageSize">Records per page (max 1 000), or <c>null</c> for the API default.</param>
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
}
