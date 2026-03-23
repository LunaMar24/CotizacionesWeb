using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoverSoloMonedaACotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "Cotizacion");

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                table: "CotizacionVersion",
                type: "decimal(18,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "CotizacionVersion");

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                table: "Cotizacion",
                type: "decimal(18,6)",
                nullable: true);
        }
    }
}
