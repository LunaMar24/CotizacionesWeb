using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandirParametrosParaConsecutivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Parametros");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "Parametros",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Parametros",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Parametros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Parametros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EsModificable",
                table: "Parametros",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notas",
                table: "Parametros",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoValor",
                table: "Parametros",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "S");

            migrationBuilder.AddColumn<string>(
                name: "ValorPorDefecto",
                table: "Parametros",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_Categoria",
                table: "Parametros",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_Categoria_Codigo",
                table: "Parametros",
                columns: new[] { "Categoria", "Codigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_Codigo",
                table: "Parametros",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_EsModificable",
                table: "Parametros",
                column: "EsModificable");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros",
                sql: "[TipoValor] IN ('S', 'N', 'B', 'D')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parametros_Categoria",
                table: "Parametros");

            migrationBuilder.DropIndex(
                name: "IX_Parametros_Categoria_Codigo",
                table: "Parametros");

            migrationBuilder.DropIndex(
                name: "IX_Parametros_Codigo",
                table: "Parametros");

            migrationBuilder.DropIndex(
                name: "IX_Parametros_EsModificable",
                table: "Parametros");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Parametros_TipoValor",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "EsModificable",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Notas",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "TipoValor",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "ValorPorDefecto",
                table: "Parametros");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "Parametros",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Parametros",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Parametros",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }
    }
}
