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
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.TrackingNumber).IsUnique();
            entity.Property(e => e.TrackingNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Seed data with fixed values (avoid dynamic values like Guid.NewGuid() or DateTime.UtcNow)
        modelBuilder.Entity<Parcel>().HasData(
            new Parcel
            {
                Id = new Guid("a210a62d-6896-4884-915c-7c641921b9bf"),
                TrackingNumber = "TRK001",
                Status = "In Transit",
                CreatedAt = new DateTime(2025, 9, 29, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3331),
                UpdatedAt = new DateTime(2025, 9, 29, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3514)
            },
            new Parcel
            {
                Id = new Guid("5655145f-0137-438f-9152-44924c521402"),
                TrackingNumber = "TRK002",
                Status = "Delivered",
                CreatedAt = new DateTime(2025, 9, 28, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3701),
                UpdatedAt = new DateTime(2025, 9, 29, 11, 54, 52, 247, DateTimeKind.Utc).AddTicks(3776)
            }
        );
    }
}