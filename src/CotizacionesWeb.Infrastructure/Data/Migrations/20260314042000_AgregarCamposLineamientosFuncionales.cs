using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposLineamientosFuncionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnviadoERP",
                table: "Cotizacion",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "N");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAceptacion",
                table: "Cotizacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEnvioERP",
                table: "Cotizacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRechazo",
                table: "Cotizacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cotizacion_EnviadoERP",
                table: "Cotizacion",
                sql: "[EnviadoERP] IN ('S', 'N')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Cotizacion_EnviadoERP",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "EnviadoERP",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "FechaAceptacion",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "FechaEnvioERP",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "FechaRechazo",
                table: "Cotizacion");
        }
    }
}
