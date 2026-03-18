using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandirParametrosConEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros",
                sql: "[TipoValor] IN ('S', 'N', 'B', 'D', 'E')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros",
                sql: "[TipoValor] IN ('S', 'N', 'B', 'D')");
        }
    }
}
