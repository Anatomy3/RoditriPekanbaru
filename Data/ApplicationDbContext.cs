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
        public DbSet<Banner> Banners { get; set; }
        public DbSet<PreOrder> PreOrders { get; set; } // TAMBAHAN BARU

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for demo purposes
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin data with new levels
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    Username = "admin",
                    Password = "admin123", // In production, use proper password hashing
                    NamaLengkap = "Administrator",
                    Level = "Admin",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Admin
                {
                    AdminId = 2,
                    Username = "user1",
                    Password = "user123",
                    NamaLengkap = "User Demo",
                    Level = "User",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Seed Banner data
            modelBuilder.Entity<Banner>().HasData(
                new Banner
                {
                    BannerId = 1,
                    NamaBanner = "Banner Promo Tanpa Drama",
                    GambarBanner = "/images/banner1.jpg",
                    LinkTujuan = "#jual-mobil",
                    Urutan = 1,
                    IsActive = true,
                    TanggalMulai = new DateTime(2024, 6, 1),
                    TanggalBerakhir = new DateTime(2024, 9, 1),
                    TanggalDibuat = new DateTime(2024, 1, 1),
                    DibuatOleh = "Administrator"
                },
                new Banner
                {
                    BannerId = 2,
                    NamaBanner = "Banner Promo Spesial",
                    GambarBanner = "/images/banner2.jpg",
                    LinkTujuan = "#promo",
                    Urutan = 2,
                    IsActive = true,
                    TanggalMulai = new DateTime(2024, 6, 1),
                    TanggalBerakhir = new DateTime(2024, 8, 1),
                    TanggalDibuat = new DateTime(2024, 1, 5),
                    DibuatOleh = "Administrator"
                }
            );

            // Seed sample car data for demo
            modelBuilder.Entity<Mobil>().HasData(
                new Mobil
                {
                    MobilId = 1,
                    Merek = "Toyota",
                    Model = "Alphard 2.5 X",
                    Tahun = 2021,
                    Warna = "Hitam",
                    Harga = 850000000,
                    Kondisi = "Bekas, kondisi sangat baik",
                    IsAvailable = true,
                    NoRangka = "MHFM1BA4XLJ123456",
                    NoMesin = "2GR-FE987654",
                    NoPolisi = "B 1234 ABC",
                    TanggalInput = new DateTime(2024, 1, 15)
                },
                new Mobil
                {
                    MobilId = 2,
                    Merek = "Honda",
                    Model = "City E CVT",
                    Tahun = 2020,
                    Warna = "Putih",
                    Harga = 285000000,
                    Kondisi = "Bekas, terawat",
                    IsAvailable = true,
                    NoRangka = "MHFG1234567890123",
                    NoMesin = "L15Z7654321",
                    NoPolisi = "B 5678 DEF",
                    TanggalInput = new DateTime(2024, 1, 20)
                },
                new Mobil
                {
                    MobilId = 3,
                    Merek = "Suzuki",
                    Model = "Ertiga GL MT",
                    Tahun = 2019,
                    Warna = "Silver",
                    Harga = 195000000,
                    Kondisi = "Bekas, service record lengkap",
                    IsAvailable = true,
                    NoRangka = "JSMRB1234567890",
                    NoMesin = "K15B1122334",
                    NoPolisi = "B 9012 GHI",
                    TanggalInput = new DateTime(2024, 1, 25)
                },
                new Mobil
                {
                    MobilId = 4,
                    Merek = "Mitsubishi",
                    Model = "Pajero Sport Dakar",
                    Tahun = 2022,
                    Warna = "Putih",
                    Harga = 520000000,
                    Kondisi = "Bekas, kilometer rendah",
                    IsAvailable = true,
                    NoRangka = "MMHKD123456789",
                    NoMesin = "4N15998877",
                    NoPolisi = "B 3456 JKL",
                    TanggalInput = new DateTime(2024, 2, 1)
                },
                new Mobil
                {
                    MobilId = 5,
                    Merek = "Daihatsu",
                    Model = "Xenia R MT",
                    Tahun = 2021,
                    Warna = "Merah",
                    Harga = 175000000,
                    Kondisi = "Bekas, kondisi prima",
                    IsAvailable = true,
                    NoRangka = "MHDXJ123456789",
                    NoMesin = "3SZ-VE556677",
                    NoPolisi = "B 7890 MNO",
                    TanggalInput = new DateTime(2024, 2, 5)
                },
                new Mobil
                {
                    MobilId = 6,
                    Merek = "Nissan",
                    Model = "Grand Livina XV",
                    Tahun = 2018,
                    Warna = "Abu-abu",
                    Harga = 165000000,
                    Kondisi = "Bekas, interior rapi",
                    IsAvailable = true,
                    NoRangka = "JNKBJ123456789",
                    NoMesin = "HR15DE445566",
                    NoPolisi = "B 2468 PQR",
                    TanggalInput = new DateTime(2024, 2, 10)
                },
                new Mobil
                {
                    MobilId = 7,
                    Merek = "BMW",
                    Model = "X3 xDrive20i",
                    Tahun = 2020,
                    Warna = "Hitam",
                    Harga = 750000000,
                    Kondisi = "Bekas, sangat terawat",
                    IsAvailable = true,
                    NoRangka = "WBAXG123456789",
                    NoMesin = "B48A20A334455",
                    NoPolisi = "B 1357 STU",
                    TanggalInput = new DateTime(2024, 2, 15)
                },
                new Mobil
                {
                    MobilId = 8,
                    Merek = "Mercedes-Benz",
                    Model = "C200 AMG",
                    Tahun = 2019,
                    Warna = "Putih",
                    Harga = 680000000,
                    Kondisi = "Bekas, full service record",
                    IsAvailable = true,
                    NoRangka = "WDD2050121A123456",
                    NoMesin = "M264920223344",
                    NoPolisi = "B 2468 VWX",
                    TanggalInput = new DateTime(2024, 2, 20)
                }
            );

            // Seed Customer data
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    NamaCustomer = "John Doe",
                    Alamat = "Jl. Sudirman No. 10, Pekanbaru",
                    NoTelepon = "081234567890",
                    Email = "john.doe@email.com",
                    JenisKelamin = "Laki-laki",
                    Pekerjaan = "Karyawan Swasta",
                    TanggalDaftar = new DateTime(2024, 1, 10)
                },
                new Customer
                {
                    CustomerId = 2,
                    NamaCustomer = "Jane Smith",
                    Alamat = "Jl. Diponegoro No. 15, Pekanbaru",
                    NoTelepon = "082345678901",
                    Email = "jane.smith@email.com",
                    JenisKelamin = "Perempuan",
                    Pekerjaan = "Wiraswasta",
                    TanggalDaftar = new DateTime(2024, 1, 15)
                }
            );

            // Seed PreOrder data (TAMBAHAN BARU)
            modelBuilder.Entity<PreOrder>().HasData(
                new PreOrder
                {
                    PreOrderId = 1,
                    CustomerId = 1,
                    MobilId = 1, // Toyota Alphard
                    TanggalPreOrder = new DateTime(2024, 6, 1),
                    JumlahDP = 100000000, // 100 juta
                    Status = "Pending",
                    Catatan = "Customer tertarik dengan Toyota Alphard, ingin booking dulu",
                    TanggalDibuat = new DateTime(2024, 6, 1),
                    UpdatedBy = "admin"
                },
                new PreOrder
                {
                    PreOrderId = 2,
                    CustomerId = 2,
                    MobilId = 4, // Mitsubishi Pajero
                    TanggalPreOrder = new DateTime(2024, 6, 5),
                    JumlahDP = 75000000, // 75 juta
                    Status = "Approved",
                    Catatan = "DP sudah diterima, menunggu konfirmasi final dari customer",
                    TanggalDibuat = new DateTime(2024, 6, 5),
                    TanggalUpdate = new DateTime(2024, 6, 6),
                    UpdatedBy = "admin"
                }
            );
        }
    }
}