using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using P02_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext:DbContext
    {
        public FootballBettingContext(DbContextOptions options):base(options)
        {
                
        }

        const string connectionString= "Server=.;Database=FootballBookmakerSystem;Encrypt=True;Integrated Security=True;TrustServerCertificate=True";

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Town> Towns { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Color> Colors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Bet> Bets { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(connectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>()
                .HasKey(x => new { x.GameId, x.PlayerId });
        }
    }
}
