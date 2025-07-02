using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoditriPekanbaru.Migrations
{
    /// <inheritdoc />
    public partial class AddPreOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaBanner = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GambarBanner = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    LinkTujuan = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Urutan = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TanggalMulai = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TanggalBerakhir = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TanggalDibuat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DibuatOleh = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                });

            migrationBuilder.CreateTable(
                name: "PreOrders",
                columns: table => new
                {
                    PreOrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MobilId = table.Column<int>(type: "INTEGER", nullable: false),
                    TanggalPreOrder = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JumlahDP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Catatan = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TanggalDibuat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TanggalUpdate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreOrders", x => x.PreOrderId);
                    table.ForeignKey(
                        name: "FK_PreOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreOrders_Mobils_MobilId",
                        column: x => x.MobilId,
                        principalTable: "Mobils",
                        principalColumn: "MobilId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 2,
                columns: new[] { "NamaLengkap", "Username" },
                values: new object[] { "User Demo", "user1" });

            migrationBuilder.InsertData(
                table: "Banners",
                columns: new[] { "BannerId", "DibuatOleh", "GambarBanner", "IsActive", "LinkTujuan", "NamaBanner", "TanggalBerakhir", "TanggalDibuat", "TanggalMulai", "Urutan" },
                values: new object[,]
                {
                    { 1, "Administrator", "/images/banner1.jpg", true, "#jual-mobil", "Banner Promo Tanpa Drama", new DateTime(2025, 9, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(116), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 9, 1, 32, 42, 31, DateTimeKind.Local).AddTicks(9811), 1 },
                    { 2, "Administrator", "/images/banner2.jpg", true, "#promo", "Banner Promo Spesial", new DateTime(2025, 8, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(640), new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(639), 2 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Alamat", "Email", "JenisKelamin", "NamaCustomer", "NoTelepon", "Pekerjaan", "TanggalDaftar" },
                values: new object[,]
                {
                    { 1, "Jl. Sudirman No. 10, Pekanbaru", "john.doe@email.com", "Laki-laki", "John Doe", "081234567890", "Karyawan Swasta", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Jl. Diponegoro No. 15, Pekanbaru", "jane.smith@email.com", "Perempuan", "Jane Smith", "082345678901", "Wiraswasta", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Mobils",
                columns: new[] { "MobilId", "GambarMobil", "Harga", "IsAvailable", "Kondisi", "Merek", "Model", "NoMesin", "NoPolisi", "NoRangka", "Tahun", "TanggalInput", "Warna" },
                values: new object[,]
                {
                    { 1, null, 850000000m, true, "Bekas, kondisi sangat baik", "Toyota", "Alphard 2.5 X", "2GR-FE987654", "B 1234 ABC", "MHFM1BA4XLJ123456", 2021, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hitam" },
                    { 2, null, 285000000m, true, "Bekas, terawat", "Honda", "City E CVT", "L15Z7654321", "B 5678 DEF", "MHFG1234567890123", 2020, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Putih" },
                    { 3, null, 195000000m, true, "Bekas, service record lengkap", "Suzuki", "Ertiga GL MT", "K15B1122334", "B 9012 GHI", "JSMRB1234567890", 2019, new DateTime(2024, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Silver" },
                    { 4, null, 520000000m, true, "Bekas, kilometer rendah", "Mitsubishi", "Pajero Sport Dakar", "4N15998877", "B 3456 JKL", "MMHKD123456789", 2022, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Putih" },
                    { 5, null, 175000000m, true, "Bekas, kondisi prima", "Daihatsu", "Xenia R MT", "3SZ-VE556677", "B 7890 MNO", "MHDXJ123456789", 2021, new DateTime(2024, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Merah" },
                    { 6, null, 165000000m, true, "Bekas, interior rapi", "Nissan", "Grand Livina XV", "HR15DE445566", "B 2468 PQR", "JNKBJ123456789", 2018, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Abu-abu" },
                    { 7, null, 750000000m, true, "Bekas, sangat terawat", "BMW", "X3 xDrive20i", "B48A20A334455", "B 1357 STU", "WBAXG123456789", 2020, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hitam" },
                    { 8, null, 680000000m, true, "Bekas, full service record", "Mercedes-Benz", "C200 AMG", "M264920223344", "B 2468 VWX", "WDD2050121A123456", 2019, new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Putih" }
                });

            migrationBuilder.InsertData(
                table: "PreOrders",
                columns: new[] { "PreOrderId", "Catatan", "CustomerId", "JumlahDP", "MobilId", "Status", "TanggalDibuat", "TanggalPreOrder", "TanggalUpdate", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Customer tertarik dengan Toyota Alphard, ingin booking dulu", 1, 100000000m, 1, "Pending", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin" },
                    { 2, "DP sudah diterima, menunggu konfirmasi final dari customer", 2, 75000000m, 4, "Approved", new DateTime(2024, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreOrders_CustomerId",
                table: "PreOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PreOrders_MobilId",
                table: "PreOrders",
                column: "MobilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "PreOrders");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Mobils",
                keyColumn: "MobilId",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 2,
                columns: new[] { "NamaLengkap", "Username" },
                values: new object[] { "User Biasa", "user" });
        }
    }
}
