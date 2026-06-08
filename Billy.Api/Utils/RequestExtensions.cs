
using Billy.Api.Models;
using RestSharp;
using System.Text.Json;

namespace Billy.Api.Utils
{
    public static class RequestExtensions
    {

        public static RestRequest AddUpdateBodyWithSharedOptions<T>(this RestRequest request, DeltaObject<T> updates) where T : class
        {

            var updatesJson = JsonSerializer.Serialize(updates.GetModifications(), RestJsonOptions.Instance);
            var root = "{\"" + typeof(T).Name.ToLowerInvariant() + "\": " + updatesJson + "}"; //Manually build this
            request.AddStringBody(root, DataFormat.Json);
            return request;
        }

        public static RestRequest AddJsonBodyWithSharedOptions(this RestRequest request, object body)
        {
            var json = JsonSerializer.Serialize(body, RestJsonOptions.Instance);
            request.AddStringBody(json, DataFormat.Json);
            return request;
        }

        public static void AddSorting(this RestRequest request, string sortProperty, SortOrder sortOrder)
        {

            if (!string.IsNullOrWhiteSpace(sortProperty))
            {
                request.AddQueryParameter("sortProperty", sortProperty);
                request.AddQueryParameter("sortOrder", sortOrder.ToString());
            }
        }

        public static void AddFilter<T>(this RestRequest request, DeltaObject<T> filter) where T : class
        {
            if (filter != null)
                request.AddFilter(filter.GetModifications());
        }

        public static void AddFilter(this RestRequest request, object? filterObject)
        {
            if (filterObject != null)
                request.AddFilter(filterDict: filterObject.AsDictionary());
        }

        public static void AddFilter(this RestRequest request, IDictionary<string, object?> filterDict)
        {
            if (filterDict != null)
                foreach (var f in filterDict)
                {
                    var key = JsonNamingPolicy.CamelCase.ConvertName(f.Key);

                    // we need to check string explicitly because it also is a IEnumerable
                    if (f.Value is string stringFilter)
                    {
                        request.AddQueryParameter(key, stringFilter);
                    }
                    else if (f.Value is System.Collections.IEnumerable multiFilter)
                    {
                        foreach (var mf in multiFilter)
                        {
                            request.AddQueryParameter(key + "[]", mf.ToString());
                        }
                    }
                    else
                    {
                        request.AddQueryParameter(key, f.Value?.ToString());
                    }
                }
        }

        public static void AddPaging(this RestRequest request, int? page, int? pageSize)
        {
            if (pageSize.HasValue)
            {
                if (page.HasValue)
                {
                    request.AddQueryParameter("page", page.Value.ToString());
                }
                request.AddQueryParameter("pageSize", pageSize.Value.ToString());
            }

        }
    }
}
