using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ExportDtos
{
    public class MedicineExport
    {
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; }

        [Range(0.01, 1000.00)]
        public string Price { get; set; }

        public PharmacyExport Pharmacy { get; set; }
    }
    public class PharmacyExport
    {
       
        public string Name { get; set; }

     
        public string PhoneNumber { get; set; }
    }
}
