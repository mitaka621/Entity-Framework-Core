﻿namespace Medicines.Data
{
    using Medicines.Data.Models;
    using Microsoft.EntityFrameworkCore;
    public class MedicinesContext : DbContext
    {
        public MedicinesContext()
        {
        }

        public MedicinesContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicine> PatientsMedicines { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientMedicine>().HasKey(x => new { x.PatientId, x.MedicineId });
        }
    }
}
