using System.Collections.Generic;

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

    public class CommonName
    {
        public string name { get; set; }
        public int occurence { get; set; }
    }
}
