using Microsoft.EntityFrameworkCore;
using SecureBankApi.Models;

namespace SecureBankApi.Data
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.MobileNumber).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.AadharNumber).IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()"); // Ensure the correct namespace is used

            base.OnModelCreating(modelBuilder);
        }
    }
}
