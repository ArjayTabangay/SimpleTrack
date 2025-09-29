using Microsoft.EntityFrameworkCore;
using ParcelTracking.Core.Models;

namespace ParcelTracking.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Parcel> Parcels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Parcel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TrackingNumber).IsUnique();
            entity.Property(e => e.TrackingNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Seed data
        modelBuilder.Entity<Parcel>().HasData(
            new Parcel
            {
                Id = Guid.NewGuid(),
                TrackingNumber = "TRK001",
                Status = "In Transit",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Parcel
            {
                Id = Guid.NewGuid(),
                TrackingNumber = "TRK002",
                Status = "Delivered",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            }
        );
    }
}