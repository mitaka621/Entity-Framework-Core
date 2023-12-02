using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class MedicineImPortDto
    {

        [Range(0, 4)]
        [XmlAttribute("category")]
        public int Category { get; set; }

        [MinLength(3)]
        [MaxLength(150)]
        [XmlElement]
        public string Name { get; set; }

        [Range(0.01, 1000.00)]
        [XmlElement]
        public decimal Price { get; set; }

        [XmlElement("ProductionDate")]
        public string ProductionDate { get; set; }

        [XmlElement("ExpiryDate")]
        public string ExpiryDate { get; set; }

        [MinLength(3)]
        [MaxLength(100)]
        [Required]
        [XmlElement]
        public string Producer { get; set; }
    }
}
