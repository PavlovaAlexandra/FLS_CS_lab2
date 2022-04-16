using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    public class Threat
    {
        public double Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceOfThreat { get; set; }
        public string Impact { get; set; }
        public bool BreachOfConfidentiality { get; set; }
        public bool BreachOfIntegrity { get; set; }
        public bool BreachOfAvailability { get; set; }
        public DateTime DateThreat { get; set; }
        public DateTime DateThreatLastChange { get; set; }
        public Threat(double id,
            string name,
            string description,
            string sourceOfThreat,
            string impact,
            bool breachOfConfidentiality,
            bool breachOfIntegrity,
            bool breachOfAvailability,
            DateTime dateThreat,
            DateTime dateThreatLastChange)
        {
            Id = id;
            Name = name;
            Description = description;
            SourceOfThreat = sourceOfThreat;
            Impact = impact;
            BreachOfConfidentiality = breachOfConfidentiality;
            BreachOfIntegrity = breachOfIntegrity;
            BreachOfAvailability = breachOfAvailability;
            DateThreat = dateThreat;
            DateThreatLastChange = dateThreatLastChange;
        }
    }
}
