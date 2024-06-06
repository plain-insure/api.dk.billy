using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Models
{
    public class Meta
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public double Time { get; set; }
        public Paging Paging { get; set; }
    }

    public class Paging
    {
        public int Page { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
                public string FirstUrl { get; set; }
        public string PreviousUrl { get; set; }
        public string NextUrl { get; set; }
        public string LastUrl { get; set; }
    }
}
