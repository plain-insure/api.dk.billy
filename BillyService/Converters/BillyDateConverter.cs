using System;
using System.Collections.Generic;
using System.Text;

namespace BillyService.Converters
{
    public class BillyDateConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public BillyDateConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
