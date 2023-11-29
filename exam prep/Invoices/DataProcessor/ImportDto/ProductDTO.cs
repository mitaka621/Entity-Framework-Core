using Invoices.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    public class ProductDTO
    {
        [MinLength(9)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }

        [Range(0, 4)]
        public CategoryType CategoryType { get; set; }

        [JsonProperty("Clients")]
        public int[] ClientsIDs { get; set; }
    }
}
