using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string LogoUrl { get; set; }=null!;

        [Required]
        public string Initials { get; set; } = null!;

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public int PrimaryKitColorId { get; set; }
        [Required]
        [ForeignKey(nameof(PrimaryKitColorId))]
        public virtual Color PrimaryKitColor { get; set; } = null!;

        [Required]
        public int SecondaryKitColorId { get; set; }
        [Required]
        [ForeignKey(nameof(SecondaryKitColorId))]
        public virtual Color SecondaryKitColor { get; set; } = null!;

        [Required]
        public int TownId { get; set; }
        [ForeignKey(nameof(TownId))]
        [Required]
        public virtual Town Town { get; set; } = null!;

        public ICollection<Player> Players { get; set; }=new List<Player>();

        [InverseProperty(nameof(Game.HomeTeam))]
        public ICollection<Game> HomeGames { get; set; } = new List<Game>();

        [InverseProperty(nameof(Game.AwayTeam))]
        public ICollection<Game> AwayGames { get; set; } = new List<Game>();
    }
}
