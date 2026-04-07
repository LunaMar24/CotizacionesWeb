using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIntegracionPedidoERP_Corregida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegracionPedidoErp",
                columns: table => new
                {
                    IntegracionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VersionId = table.Column<int>(type: "int", nullable: false),
                    LoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PedidoErp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Intentos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MensajeError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegracionPedidoErp", x => x.IntegracionId);
                    table.ForeignKey(
                        name: "FK_IntegracionPedidoErp_Cotizacion",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntegracionPedidoErp_CotizacionVersion",
                        column: x => x.VersionId,
                        principalTable: "CotizacionVersion",
                        principalColumn: "VersionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegracionPedidoErp_CotizacionId_VersionId",
                table: "IntegracionPedidoErp",
                columns: new[] { "CotizacionId", "VersionId" });

            migrationBuilder.CreateIndex(
                name: "IX_IntegracionPedidoErp_Estado",
                table: "IntegracionPedidoErp",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_IntegracionPedidoErp_LoteId",
                table: "IntegracionPedidoErp",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegracionPedidoErp_VersionId",
                table: "IntegracionPedidoErp",
                column: "VersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegracionPedidoErp");
        }
    }
}
