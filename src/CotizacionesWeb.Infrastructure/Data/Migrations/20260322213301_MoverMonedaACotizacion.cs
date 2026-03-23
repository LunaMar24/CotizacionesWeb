using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoverMonedaACotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "CotizacionVersion");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "CotizacionVersion");

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                table: "Cotizacion",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "CRC");

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                table: "Cotizacion",
                type: "decimal(18,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "Cotizacion");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "Cotizacion");

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                table: "CotizacionVersion",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                table: "CotizacionVersion",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
