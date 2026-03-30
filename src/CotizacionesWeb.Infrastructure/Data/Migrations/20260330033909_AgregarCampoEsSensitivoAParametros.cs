using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoEsSensitivoAParametros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsSensitivo",
                table: "Parametros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_EsSensitivo",
                table: "Parametros",
                column: "EsSensitivo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parametros_EsSensitivo",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "EsSensitivo",
                table: "Parametros");
        }
    }
}
