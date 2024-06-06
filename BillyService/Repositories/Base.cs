using BillyService.Models;
using BillyService.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;

namespace BillyService.Repositories
{
    public abstract class Base<T, TRoot>
        where T : class, new()
        where TRoot : class, new()
    {
        protected RestClient client;
        protected Func<TRoot?, T?> rootToSingle;
        protected Func<TRoot?, IList<T>?> rootToMultiple;
        protected Func<T?, string?> itemToId;
        protected string requestUrl;


        public Base(
            RestClient client,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId)
        {
            this.client = client;

            //Todo: check for correctly formed url
            this.requestUrl = requestUrl;
            this.rootToSingle = rootToSingle;
            this.rootToMultiple = rootToMultiple;
            this.itemToId = itemToId;
        }

        public Base(
            string key,
            string requestUrl,
            Func<TRoot?, T?> rootToSingle,
            Func<TRoot?, IList<T>?> rootToMultiple,
            Func<T?, string?> itemToId)
        {
            client = ClientExtensions.CreateBillyClient(key);

            //Todo: check for correctly formed url
            this.requestUrl = requestUrl;
            this.rootToSingle = rootToSingle;
            this.rootToMultiple = rootToMultiple;
            this.itemToId = itemToId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? Get(string id)
        {
            try
            {
                var request = new RestRequest(requestUrl + id, Method.Get)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<TRoot>(request);

                if (!response.IsSuccessful)
                    return null;

                Debug.WriteLine(response.StatusCode);
                Debug.WriteLine(response.Content);

                return rootToSingle(response.Data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<T>? List()
        {

            return List(null, null, SortOrder.ASC, null, null);
        }

        public IList<T>? List(object filter)
        {
            return List(filter, null, SortOrder.ASC, null, null);
        }

        public IList<T>? List(object filter, int page, int pageSize)
        {
            return List(filter, null, SortOrder.ASC, page, pageSize);
        }

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
        public IList<T>? List(Expression<Func<T, object>> sortProperty, SortOrder sortOrder)
        {
            return List(null, Utils.PropertyHelper.GetName(sortProperty), sortOrder, null, null);
        }

        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder)
        {
            return List(filter, Utils.PropertyHelper.GetName(sortProperty), sortOrder, null, null);
        }

        public IList<T>? List(object filter, Expression<Func<T, object>> sortProperty, SortOrder sortOrder, int pageSize)
        {
            return List(filter, Utils.PropertyHelper.GetName(sortProperty), sortOrder, null, pageSize);
        }

        public IList<T>? List(object? filter, string? sortProperty, SortOrder sortOrder, int? page, int? pageSize)
        {
            try
            {
                var request = new RestRequest(requestUrl, Method.Get);
                if (sortProperty != null)
                    request.AddSorting(sortProperty, sortOrder);
                if (filter != null)
                    request.AddFilter(filter);
                if (page != null && pageSize != null)
                    request.AddPaging(page, pageSize);

                request.RequestFormat = DataFormat.Json;

                return rootToMultiple(client.Get<TRoot>(request));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string? Create(T item)
        {
            try
            {
                var request = new RestRequest(requestUrl, Method.Post)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(item);

                var result = client.Post<TRoot>(request);
                return itemToId(rootToMultiple(result)?[0]);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
