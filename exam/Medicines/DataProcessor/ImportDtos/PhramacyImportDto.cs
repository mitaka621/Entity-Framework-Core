using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class PhramacyImportDto
    {
        [XmlAttribute("non-stop")]
        [RegularExpression("^(true|false)")]
        public string IsNonStop { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        [XmlElement]
        public string Name { get; set; }

        [MinLength(14)]
        [MaxLength(14)]
        [RegularExpression("^\\([0-9]{3}\\) [0-9]{3}\\-[0-9]{4}")]
        [XmlElement]
        public string PhoneNumber { get; set; }

        [XmlArray("Medicines")]
        public List<MedicineImPortDto> Medicines { get; set; }
    }
}
