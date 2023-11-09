using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime ReleaseDate  { get; set; }

        [NotMapped]
        public decimal Price => Songs.Sum(x=>x.Price);

        public int? ProducerId  { get; set; }
        [ForeignKey(nameof(ProducerId))]
        [Required]
        public Producer Producer { get; set; } = null!;

        public ICollection<Song> Songs { get; set; }=new List<Song>();

    }
}
