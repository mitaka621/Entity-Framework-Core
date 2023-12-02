using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Data.Models
{
    public class Medicine
    {
        [Key]
        public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; }

        [Range(0.01,1000.00)]
        public decimal Price { get; set; }

        [Range(0,4)]
        public Category Category { get; set; }

        public DateTime ProductionDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; }

        public int PharmacyId { get; set; }
        [ForeignKey(nameof(PharmacyId))]
        public Pharmacy Pharmacy { get; set; }

        public ICollection<PatientMedicine> PatientsMedicines { get; set; } = new List<PatientMedicine>();

    }
}
