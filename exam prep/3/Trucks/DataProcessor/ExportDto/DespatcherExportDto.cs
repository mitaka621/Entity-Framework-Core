using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Despatcher")]
    public class DespatcherExportDto
    {
        [XmlAttribute]
        public int TrucksCount { get; set; }

        [XmlElement]
        public string DespatcherName { get; set; }

        [XmlArray("Trucks")]
        public TrucksExportDto[] Trucks { get; set; }
    }
}
