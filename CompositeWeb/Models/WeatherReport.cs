using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompositeWeb.Models
{
    using System.Security.AccessControl;

    public class WeatherReport
    {
        public string Temprature { get; set; }
        public string Weather { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
