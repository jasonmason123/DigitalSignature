using DigitalSignature_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalSignature_Web.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<PublicKey> PublicKeys { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<DocHashtag> DocHashtags { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.DocumentsOwned)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId);

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.SharedDocuments)
                .WithMany(d => d.Viewers)
                .UsingEntity<Dictionary<string, object>>(
                    "DocumentViewers",
                    j => j.HasOne<Document>().WithMany().HasForeignKey("DocumentId"),
                    j => j.HasOne<AppUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("DocumentId", "UserId");
                    });
        }
    }
}
