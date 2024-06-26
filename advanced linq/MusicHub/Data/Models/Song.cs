﻿using MusicHub.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        public TimeSpan Duration  { get; set; }

        [Required]
        public DateTime CreatedOn  { get; set; }

        [Required]
        public Genre Genre { get; set; }

        
        public int? AlbumId { get; set; }
        [ForeignKey(nameof(AlbumId))]
        [Required]
        public Album Album { get; set; } = null!;

        [Required]
        public int WriterId  { get; set; }
        [ForeignKey(nameof(WriterId))]
        [Required]
        public Writer Writer { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        public ICollection<SongPerformer> SongPerformers { get; set; }=new List<SongPerformer>();
    }
}
