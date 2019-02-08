using BillyService.Models;
using BillyService.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;

namespace BillyService.Repositories
{
    public abstract class Base<T,TRoot> 
        where T : class, new()
        where TRoot  : class, new()
    {
        private RestClient client;
        private Func<TRoot, T> rootToSingle;
        private Func<TRoot, IList<T>> rootToMultiple;
        private Func<T, string> itemToId;
        private string requestUrl;

        public Base(
            string key, 
            string requestUrl, 
            Func<TRoot, T> rootToSingle, 
            Func<TRoot, IList<T>> rootToMultiple,
            Func<T, string> itemToId)
        {
            //Todo: reuse restclient across repositories
            client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddDefaultHeader("X-Access-Token", key);

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
        public T Get(string id)
        {
            try
            {
                var request = new RestRequest(requestUrl + id, Method.GET);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get<TRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return rootToSingle(result);
                }
                else
                {
                    return null;
                }
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
        public IList<T> List()
        {

            return List(null, null, SortOrder.ASC);
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
        public IList<T> List(Expression<Func<T, object>> sortProperty, SortOrder sortOrder)
        {
            return List(null, Utils.PropertyHelper.GetName(sortProperty), SortOrder.ASC);
        }

        public IList<T> List(object filter, string sortProperty, SortOrder sortOrder)
        {
            try
            {
                var request = new RestRequest(requestUrl, Method.GET);

                request.AddSortingAndFilter(filter, sortProperty, sortOrder);

                request.RequestFormat = DataFormat.Json;

                var response = client.Get<TRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return rootToMultiple(result)   ;
                }
                else
                {
                    return null;
                }
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
        public string Create(T item)
        {
            try
            {
                var request = new RestRequest(requestUrl, Method.POST);

                request.RequestFormat = DataFormat.Json;

                request.AddJsonBody(item);

                var response = client.Post<TRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;
                    
                    return itemToId(rootToMultiple(result)[0]);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
