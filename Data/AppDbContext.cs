using appdev_sprint.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace appdev_sprint.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<HomePriceAnchor> HomePriceAnchors => Set<HomePriceAnchor>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Procedure>()
            .HasOne(p => p.Clinic)
            .WithMany(c => c.Procedures)
            .HasForeignKey(p => p.ClinicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Procedure>().Property(p => p.PricePhp).HasPrecision(12, 2);
        builder.Entity<HomePriceAnchor>().Property(h => h.PriceAmount).HasPrecision(12, 2);
    }
}
