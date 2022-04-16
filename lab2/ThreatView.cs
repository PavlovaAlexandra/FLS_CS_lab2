using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    public class ThreatView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ThreatView(double id, string name)
        {
            Id = "УБИ." + id.ToString();
            Name = name;
        }
    }
}
