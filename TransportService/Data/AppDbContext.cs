using Microsoft.EntityFrameworkCore;
using TransportService.Data.Entities;

namespace TransportService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreated();
                else if (entry.State == EntityState.Modified)
                    entry.Entity.SetUpdated();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing Vehicle constraints
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => new { v.Name, v.Brand, v.Year })
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
        .Property(v => v.Price)
        .HasPrecision(18, 2);

            //User entity config
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Email).IsUnique();

                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.PasswordHash).IsRequired();
                entity.Property(x => x.Role).IsRequired();

                entity.Property(x => x.CreatedDate)
              .IsRequired();

                entity.Property(x => x.UpdatedDate)
                      .IsRequired(false);
            });

            // RefreshToken entity config
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Token).IsUnique();

                entity.Property(x => x.Token).IsRequired();

                entity.Property(x => x.CreatedDate)
             .IsRequired();

                entity.Property(x => x.UpdatedDate)
                      .IsRequired(false);


                entity.HasOne(x => x.User)
                      .WithMany(x => x.RefreshTokens)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
