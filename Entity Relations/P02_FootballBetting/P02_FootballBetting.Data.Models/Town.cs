using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int CountryId { get; set; }
        [Required]
        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }=null!; 

        public ICollection<Player> Players { get; set; }=new List<Player>();

        public ICollection<Team> Teams { get; set; }=new List<Team>();


    }
}
