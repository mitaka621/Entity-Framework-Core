﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class Seller
    {
        [Key]
        public int Id { get; set; }

        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(30)]
        public string Address { get; set; }

        public string Country { get; set; }

        public string Website { get; set; }

        public ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new List<BoardgameSeller>();
    }
}
