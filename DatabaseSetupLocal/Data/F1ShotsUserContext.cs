using DatabaseSetupLocal.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSetupLocal.Data;

public class F1ShotsUserContext : IdentityDbContext
{
    public F1ShotsUserContext(DbContextOptions<F1ShotsUserContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<F1ShotsUser> UserModel { get; set; }

}