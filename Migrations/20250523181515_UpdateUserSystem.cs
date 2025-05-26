using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoditriPekanbaru.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    NamaLengkap = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaCustomer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Alamat = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NoTelepon = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    JenisKelamin = table.Column<string>(type: "TEXT", nullable: false),
                    Pekerjaan = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TanggalDaftar = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Mobils",
                columns: table => new
                {
                    MobilId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Merek = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GambarMobil = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Tahun = table.Column<int>(type: "INTEGER", nullable: false),
                    Warna = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Harga = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Kondisi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoRangka = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NoMesin = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NoPolisi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TanggalInput = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mobils", x => x.MobilId);
                });

            migrationBuilder.CreateTable(
                name: "TransaksiPenjualans",
                columns: table => new
                {
                    TransaksiId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoFaktur = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TanggalTransaksi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MobilId = table.Column<int>(type: "INTEGER", nullable: false),
                    HargaJual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Diskon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBayar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UangMuka = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SisaPembayaran = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusPembayaran = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CaraPembayaran = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Keterangan = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Admin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StatusMobil = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaksiPenjualans", x => x.TransaksiId);
                    table.ForeignKey(
                        name: "FK_TransaksiPenjualans_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransaksiPenjualans_Mobils_MobilId",
                        column: x => x.MobilId,
                        principalTable: "Mobils",
                        principalColumn: "MobilId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "AdminId", "CreatedDate", "IsActive", "LastLogin", "Level", "NamaLengkap", "Password", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, "Admin", "Administrator", "admin123", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, "User", "User Biasa", "user123", "user" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransaksiPenjualans_CustomerId",
                table: "TransaksiPenjualans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransaksiPenjualans_MobilId",
                table: "TransaksiPenjualans",
                column: "MobilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "TransaksiPenjualans");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Mobils");
        }
    }
}
