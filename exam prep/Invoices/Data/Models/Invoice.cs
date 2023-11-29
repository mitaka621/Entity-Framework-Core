using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.Data.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Range(1000000000, 1500000000)]
        public int Number { get; set; }

        public DateTime IssueDate  { get; set; }

        public DateTime DueDate  { get; set; }

        public decimal Amount { get; set; }

        [EnumDataType(typeof(CurrencyType))]
        public CurrencyType CurrencyType { get; set; }

        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
    }
}
