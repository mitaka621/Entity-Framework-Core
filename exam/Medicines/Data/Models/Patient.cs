using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Data.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [MinLength(5)]
        [MaxLength(100)]
        public string FullName  { get; set; }

        [Range(0,2)]
        public AgeGroup AgeGroup { get; set; }

        [Range(0,1)]
        public Gender Gender { get; set; }

        public ICollection<PatientMedicine> PatientsMedicines { get; set; } = new List<PatientMedicine>();
    }
}
