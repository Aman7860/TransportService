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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("GETDATE()")
                      .IsRequired();

                entity.Property(e => e.UpdatedDate)
                      .IsRequired(false);
            });
        }
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    }
}
