using DatabaseSetupLocal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Data;
[AllowAnonymous]
public class ShotsContext : DbContext
{
    public string DbPath { get; }

    public ShotsContext()
    {
        // var folder = Environment.SpecialFolder.LocalApplicationData;
        // var path = Environment.GetFolderPath(folder);
        DbPath =  "shots.db";
        // this.Database.Migrate();

    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserShots>(entity => { entity.HasKey(e => e.Id); });
        modelBuilder.Entity<Race>(entity => { entity.HasKey(e => e.Id); });

        modelBuilder.Entity<Shot>(entity => { entity.HasKey(e => e.Id); });
    }

    public DbSet<UserShots> UserModel { get; set; }
    public DbSet<Race> RaceModel { get; set; }
    public DbSet<Shot> ShotModel { get; set; }
}