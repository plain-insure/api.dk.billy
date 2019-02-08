
using BillyService.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillyService.Utils
{
    public static class RequestHelper
    {

        public static void AddSortingAndFilter(this RestRequest request, object filter, string sortProperty, SortOrder sortOrder)
        {
            AddSorting(request, sortProperty, sortOrder);
            AddFilter(request, filter);
        }
        
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
                var filterValues = TypeHelper.ObjectToDictionary(filter);
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
                        request.AddQueryParameter(f.Key, f.Value.ToString());
                    }
                }
            }
        }
    }
}
