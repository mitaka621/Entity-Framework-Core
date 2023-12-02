using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Data.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [MinLength(14)]
        [MaxLength(14)]
        [RegularExpression("^\\([0-9]{3}\\) [0-9]{3}\\-[0-9]{4}")]
        public string PhoneNumber { get; set; }

        public bool IsNonStop { get; set; }

        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
