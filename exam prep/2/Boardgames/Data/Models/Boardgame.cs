using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class Boardgame
    {
        [Key]
        public int Id { get; set; }

        [MinLength(10)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Range(1,10.00)]
        public double Rating { get; set; }

        [Range(2018,2023)]
        public int YearPublished { get; set; }

        public CategoryType CategoryType { get; set; }

        public string Mechanics { get; set; }

        public int CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        public Creator Creator { get; set; }

        public ICollection<BoardgameSeller> BoardgamesSellers { get; set; }=new List<BoardgameSeller>();   
    }
}
