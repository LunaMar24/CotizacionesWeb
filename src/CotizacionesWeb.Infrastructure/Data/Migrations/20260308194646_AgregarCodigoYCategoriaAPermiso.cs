using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoYCategoriaAPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permisos_Descripcion",
                table: "Permisos");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Permisos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Permisos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Permisos");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Descripcion",
                table: "Permisos",
                column: "Descripcion",
                unique: true);
        }
    }
}
