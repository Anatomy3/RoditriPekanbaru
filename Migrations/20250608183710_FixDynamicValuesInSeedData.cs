using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoditriPekanbaru.Migrations
{
    /// <inheritdoc />
    public partial class FixDynamicValuesInSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Banners",
                keyColumn: "BannerId",
                keyValue: 1,
                columns: new[] { "TanggalBerakhir", "TanggalMulai" },
                values: new object[] { new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Banners",
                keyColumn: "BannerId",
                keyValue: 2,
                columns: new[] { "TanggalBerakhir", "TanggalMulai" },
                values: new object[] { new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Banners",
                keyColumn: "BannerId",
                keyValue: 1,
                columns: new[] { "TanggalBerakhir", "TanggalMulai" },
                values: new object[] { new DateTime(2025, 9, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(116), new DateTime(2025, 6, 9, 1, 32, 42, 31, DateTimeKind.Local).AddTicks(9811) });

            migrationBuilder.UpdateData(
                table: "Banners",
                keyColumn: "BannerId",
                keyValue: 2,
                columns: new[] { "TanggalBerakhir", "TanggalMulai" },
                values: new object[] { new DateTime(2025, 8, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(640), new DateTime(2025, 6, 9, 1, 32, 42, 32, DateTimeKind.Local).AddTicks(639) });
        }
    }
}
