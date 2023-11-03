using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int SquadNumber { get; set; }

        [Required]
        public int TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; }=null!;

        [Required]
        public int PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public virtual Position Position { get; set; }=null!;

        [Required]
        public bool IsInjured { get; set; }

        [Required]
        public int TownId { get; set; }
        [ForeignKey(nameof(TownId))]
        [Required]
        public virtual Town Town { get; set; }=null!;

        public ICollection<PlayerStatistic> PlayersStatistics { get; set; }=new List<PlayerStatistic>();

    }
}
