using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Data.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        
        [MinLength(10)]
        [MaxLength(20)]
        public string StreetName  { get; set; }

        public int StreetNumber  { get; set; }

        public string PostCode  { get; set; }

       
        [MinLength(5)]
        [MaxLength(15)]
        public string City  { get; set; }

        [MinLength(5)]
        [MaxLength(15)]
        public string Country  { get; set; }

        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
    }
}
