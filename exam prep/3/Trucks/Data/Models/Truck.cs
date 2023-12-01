using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        [Key]
        public int Id { get; set; }

        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression("^[A-Z]{2}[0-9]{4}[A-Z]{2}")]
        public string RegistrationNumber { get; set; }

        [MinLength(17)]
        [MaxLength(17)]
        public string VinNumber { get; set; }

        [Range(950,1420)]
        public int TankCapacity { get; set; }

        [Range(5000,29000)]
        public int CargoCapacity { get; set; }

        [Required]
        [Range(0,3)]
        public CategoryType CategoryType { get; set; }

        [Required]
        [Range(0, 4)]
        public MakeType MakeType { get; set; }

        public int DespatcherId { get; set; }
        [ForeignKey(nameof(DespatcherId))]
        public Despatcher Despatcher { get; set; }

        public ICollection<ClientTruck> ClientsTrucks { get; set; } = new List<ClientTruck>();
    }
}
