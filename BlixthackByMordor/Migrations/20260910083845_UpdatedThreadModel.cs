using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlixthackByMordor.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedThreadModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ThreadLocked",
                table: "Threads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThreadLockedAt",
                table: "Threads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThreadLockedBy",
                table: "Threads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThreadLocked",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "ThreadLockedAt",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "ThreadLockedBy",
                table: "Threads");
        }
    }
}
