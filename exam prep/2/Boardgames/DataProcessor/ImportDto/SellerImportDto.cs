using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class SellerImportDto
    {

        [MinLength(5)]
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(30)]
        [Required]
        public string Address { get; set; }

        [Required]
        public string Country { get; set; }

        [RegularExpression("^www\\.[\\w|\\-]+\\.com\\b")]
        [Required]
        public string Website { get; set; }

        
        public List<int> Boardgames { get; set; }
    }
}
