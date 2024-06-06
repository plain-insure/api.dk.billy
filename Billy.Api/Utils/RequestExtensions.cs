
using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Utils
{
    public static class RequestExtensions
    {

        public static void AddSorting(this RestRequest request, string sortProperty, SortOrder sortOrder)
        {

            if (!string.IsNullOrWhiteSpace(sortProperty))
            {
                request.AddQueryParameter("sortProperty", sortProperty);
                request.AddQueryParameter("sortOrder", sortOrder.ToString());
            }
        }

        public static void AddFilter(this RestRequest request, object filter)
        {
            if (filter != null)
            {
                var filterValues = filter.AsDictionary();
                foreach (var f in filterValues)
                {
                    // we need to check string explicitly because it also is a IEnumerable
                    if (f.Value is string stringFilter)
                    {
                        request.AddQueryParameter(f.Key, stringFilter);
                    }
                    else if (f.Value is System.Collections.IEnumerable multiFilter)
                    {
                        foreach (var mf in multiFilter)
                        {
                            request.AddQueryParameter(f.Key + "[]", mf.ToString());
                        }
                    }
                    else
                    {
                        request.AddQueryParameter(f.Key, f.Value?.ToString());
                    }
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
