using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Models
{
    public class Meta
    {
        public int statusCode { get; set; }
        public bool success { get; set; }
        public double time { get; set; }
        public Paging paging { get; set; }
    }

    public class Paging
    {
        public int page { get; set; }
        public int pageCount { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
                public string firstUrl { get; set; }
        public string previousUrl { get; set; }
        public string nextUrl { get; set; }
        public string lastUrl { get; set; }
    }
}
