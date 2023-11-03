using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext:DbContext
    {
        string connectionString = "Server=.;Database=StudentSystem;Encrypt=True;Integrated Security=True;TrustServerCertificate=True";

        public DbSet<Resource> Resources { get; set; } = null!;

        public DbSet<Course> Courses { get; set; } = null!;

        public DbSet<Homework> Homeworks { get; set; } = null!;

        public DbSet<Student> Students { get; set; } = null!;

        public DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

        public StudentSystemContext(DbContextOptions options)
        : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(connectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });
        }
    }
}
