using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("customer")]
    public class CustomersExport
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; } = null!;

        [XmlAttribute("bought-cars")]
        public int CarsBought { get; set; }

        //[XmlIgnore]
        //public List<List<decimal>> MoneyParts { get; set; }

        //[XmlIgnore]
        //public bool IsYoung { get; set; }

        //[XmlAttribute("spent-money")]
        //public string MoneySpent { get { return IsYoung ? $"{MoneyParts.Sum(x => x.Sum(y => y * 0.95m)):F2}": $"{MoneyParts.Sum(x => x.Sum()):F2}"; } set { } }
        [XmlAttribute("spent-money")]
        public decimal MoneySpent { get; set; }

    }
}
