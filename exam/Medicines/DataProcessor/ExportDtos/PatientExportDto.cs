using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class PatientExportDto
    {
        [XmlAttribute]
        public string Gender { get; set; }

        [XmlElement("Name")]
        public string FullName { get; set; }

        [XmlElement("AgeGroup")]
        public string AgeGroup { get; set; }

        [XmlArray("Medicines")]
        public MedicineExportDto[] Medicines { get; set; }

    }

    [XmlType("Medicine")]
    public class MedicineExportDto
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Price { get; set; }

        [XmlElement]
        public string Producer { get; set; }

        [XmlElement]
        public string BestBefore { get; set; }
    }
}
