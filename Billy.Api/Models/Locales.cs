using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Api.Models
{

    public class LocaleRoot
    {
        public IEnumerable<Locale> Locales { get; set; }
    }

    public class Locale : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }

}
