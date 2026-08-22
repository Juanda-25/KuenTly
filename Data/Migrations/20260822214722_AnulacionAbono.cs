using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuenTly.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnulacionAbono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAnulacion",
                table: "Abonos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoAnulacion",
                table: "Abonos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaAnulacion",
                table: "Abonos");

            migrationBuilder.DropColumn(
                name: "MotivoAnulacion",
                table: "Abonos");
        }
    }
}
