using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Api.Models
{

    public class Currency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float ExchangeRate { get; set; }
        public object DeletedTime { get; set; }
    }

}
