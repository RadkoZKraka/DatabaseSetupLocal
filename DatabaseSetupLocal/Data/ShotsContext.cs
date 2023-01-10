using DatabaseSetupLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Data;

public class ShotsContext : DbContext
{

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString: @"server=localhost;database=dbsetuptest;uid=rkkrowicki;password=Lapis5Redstone9;", 
                ServerVersion.AutoDetect(@"server=localhost;database=dbsetuptest;uid=rkkrowicki;password=Lapis5Redstone9;"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Shot>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
        public DbSet<User> UserModel { get; set; }
        public DbSet<Race> RaceModel { get; set; }
        public DbSet<Shot> ShotModel { get; set; }
}