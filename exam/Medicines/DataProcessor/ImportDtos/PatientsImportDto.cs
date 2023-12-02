using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.DataProcessor.ImportDtos
{
    public class PatientsImportDto
    {
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Range(0, 2)]
        public int AgeGroup { get; set; }

        [Range(0, 1)]
        public int Gender { get; set; }

        public List<int> Medicines { get; set; }
    }
}
