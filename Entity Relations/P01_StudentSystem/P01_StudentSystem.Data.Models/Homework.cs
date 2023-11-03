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
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }

        [Unicode(false)]
        [Required]
        public string Content { get; set; } = null!;

        public ContentTypes ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        [Required]
        public Student Student { get; set; }=null!;

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        [Required]
        public Course Course { get; set; }=null!;

        public enum ContentTypes
        {
            Application,
            Pdf,
            Zip
        }
    }
}
