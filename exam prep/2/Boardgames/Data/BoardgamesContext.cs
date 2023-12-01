﻿namespace Boardgames.Data
{
    using Boardgames.Data.Models;
    using Microsoft.EntityFrameworkCore;
    
    public class BoardgamesContext : DbContext
    {
        public BoardgamesContext()
        { 
        }

        public BoardgamesContext(DbContextOptions options)
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
        public DbSet<Boardgame> Boardgames { get; set; }
        public DbSet<BoardgameSeller> BoardgamesSellers { get; set; }
        public DbSet<Creator> Creators { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardgameSeller>().HasKey(x => new { x.SellerId, x.BoardgameId });
        }
    }
}
