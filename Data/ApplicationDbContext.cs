using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Mobil> Mobils { get; set; }
        public DbSet<TransaksiPenjualan> TransaksiPenjualans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for demo purposes
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin data
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Username = "admin",
                    // Simple password without BCrypt for demo
                    Password = "admin123",
                    NamaLengkap = "Administrator",
                    Level = "Admin",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1) // Ganti DateTime.Now dengan tanggal statis
                },
                new Admin
                {
                    AdminId = 2,
                    Username = "penjualan",
                    Password = "penjualan123",
                    NamaLengkap = "Bagian Penjualan",
                    Level = "Penjualan",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1) // Ganti DateTime.Now dengan tanggal statis
                },
                new Admin
                {
                    AdminId = 3,
                    Username = "customer",
                    Password = "customer123",
                    NamaLengkap = "Customer User",
                    Level = "Customer",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Add seed data for other entities if needed...
        }
    }
}
