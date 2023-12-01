using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class TrucksExportDto
    {
        [XmlElement]
        public string RegistrationNumber { get; set; }

        [XmlElement]
        public string Make { get; set; }
    }
}
