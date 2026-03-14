using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoverCamposObsoletos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioCreacion",
                table: "CotizacionVersion");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "Cotizacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacion",
                table: "CotizacionVersion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Cotizacion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Cotizacion",
                type: "datetime2",
                nullable: true);
        }
    }
}
