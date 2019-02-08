using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace BillyService.Repositories
{
    public abstract class BaseWithDelete<T, TRoot> : Base<T, TRoot>
        where T : class, new()
        where TRoot : class, new()
    {
        public BaseWithDelete(
           string key,
           string requestUrl,
           Func<TRoot, T> rootToSingle,
           Func<TRoot, IList<T>> rootToMultiple,
           Func<T, string> itemToId) : base(key, requestUrl, rootToSingle, rootToMultiple, itemToId)
        {
        }

        public bool Delete(string id)
        {
            try
            {
                var request = new RestRequest(requestUrl + id, Method.DELETE);

                request.RequestFormat = DataFormat.Json;

                var response = client.Delete<TRoot>(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Data;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}