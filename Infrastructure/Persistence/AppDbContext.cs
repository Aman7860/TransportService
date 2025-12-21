using Microsoft.EntityFrameworkCore;
using Automobile.Domain.Entities;

namespace Automobile.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("Vehicles");

                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).ValueGeneratedOnAdd();

                // Map to backing fields on the Vehicle entity to preserve encapsulation
                entity.Property(v => v.Name)
                      .HasField("_name")
                      .IsRequired();

                entity.Property(v => v.Brand)
                      .HasField("_brand")
                      .IsRequired();

                entity.Property(v => v.Year)
                      .HasField("_year");

                entity.Property(v => v.Price)
                      .HasField("_price")
                      .HasColumnType("decimal(18,2)");
            });
        }
    }
}