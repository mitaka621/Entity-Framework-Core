using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(40)]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; }

        public string Type { get; set; }

        public ICollection<ClientTruck> ClientsTrucks { get; set; }=new List<ClientTruck>();   
    }
}
