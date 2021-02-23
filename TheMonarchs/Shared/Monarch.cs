using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMonarchs.Shared
{
    public class Monarch
    {
        public int id { get; set; }
        public string nm { get; set; }
        public string cty { get; set; }
        public string hse { get; set; }
        public string yrs { get; set; }

        public int rule { get; set; }
    }

    public class House
    {
        public string HouseName { get; set; }
        public List<Monarch> Monarchs { get; set; }
    }
}
