using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [StringLength(50)]
        [Unicode(true)]
        [Required]
        public string Name { get; set; } = null!;

        [Unicode(false)]
        [Required]
        public string Url { get; set; }= null!;

        public ResourceTypes ResourceType  { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        [Required]
        public Course Course { get; set; } = null!;

        public enum ResourceTypes 
        {
            Video,
            Presentation,
            Document,
            Other
        }
    }
}
