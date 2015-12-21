using BillyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Models
{
    public class RootObject
    {
        public Meta meta { get; set; }
        public List<Contact> contacts { get; set; }
        public Contact contact { get; set; }
    }
}
