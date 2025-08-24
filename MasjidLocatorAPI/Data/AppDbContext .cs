using MasjidLocatorAPI.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;

namespace MasjidLocatorAPI.Data
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MasjidSubmissionEntity> MasjidSubmission { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure spatial data type for Location
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            
            modelBuilder.Entity<RefreshTokenEntity>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshToken)
            .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<MasjidSubmissionEntity>(entity =>
            {
                entity.HasIndex(p => p.Location).HasMethod("GIST");
                entity.Property(p => p.Location).HasColumnType("geography (point)");
            });
        }
    }
}
