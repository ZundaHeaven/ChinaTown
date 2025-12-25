using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaTown.Application.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "Books");

            migrationBuilder.AddColumn<Guid>(
                name: "CoverFileId",
                table: "Books",
                type: "uniqueidentifier",
                maxLength: 500,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 12, 25, 10, 41, 6, 419, DateTimeKind.Utc).AddTicks(3005), new DateTime(2025, 12, 25, 10, 41, 6, 419, DateTimeKind.Utc).AddTicks(3019) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 12, 25, 10, 41, 6, 419, DateTimeKind.Utc).AddTicks(3705), new DateTime(2025, 12, 25, 10, 41, 6, 419, DateTimeKind.Utc).AddTicks(3719) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverFileId",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 12, 24, 20, 53, 14, 393, DateTimeKind.Utc).AddTicks(5793), new DateTime(2025, 12, 24, 20, 53, 14, 393, DateTimeKind.Utc).AddTicks(5807) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                columns: new[] { "CreatedOn", "ModifiedOn" },
                values: new object[] { new DateTime(2025, 12, 24, 20, 53, 14, 393, DateTimeKind.Utc).AddTicks(6510), new DateTime(2025, 12, 24, 20, 53, 14, 393, DateTimeKind.Utc).AddTicks(6525) });
        }
    }
}
