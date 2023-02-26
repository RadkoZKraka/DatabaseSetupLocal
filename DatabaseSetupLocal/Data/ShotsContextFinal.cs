using DatabaseSetupLocal.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Data;

public class ShotsContextFinal : DbContext
{
    public string DbPath { get; }

    public ShotsContextFinal()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "shots.db");
        this.Database.Migrate();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity => { entity.HasKey(e => e.Id); });
        modelBuilder.Entity<Race>(entity => { entity.HasKey(e => e.Id); });

        modelBuilder.Entity<Shot>(entity => { entity.HasKey(e => e.Id); });
    }

    public DbSet<User> UserModel { get; set; }
    public DbSet<Race> RaceModel { get; set; }
    public DbSet<Shot> ShotModel { get; set; }
}