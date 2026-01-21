using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChinaTown.Application.Migrations
{
    /// <inheritdoc />
    public partial class TablesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Recipes");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Recipes");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Recipes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
