using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        
        [MinLength(9)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Range(5,1000)]
        public decimal Price { get; set; }

        [Range(0,4)]
        public CategoryType CategoryType { get; set; }

        public ICollection<ProductClient> ProductsClients { get; set; }=new List<ProductClient>();
    }
}
