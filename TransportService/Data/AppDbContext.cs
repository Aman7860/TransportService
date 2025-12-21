using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TransportService.Models;

namespace TransportService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated();
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated();
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => new { v.Name, v.Brand, v.Year })
                .IsUnique();
        }


    }
}
