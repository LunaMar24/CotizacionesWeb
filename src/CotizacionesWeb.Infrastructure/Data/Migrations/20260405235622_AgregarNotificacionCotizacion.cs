using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNotificacionCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacionCotizacion",
                columns: table => new
                {
                    NotificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VersionId = table.Column<int>(type: "int", nullable: false),
                    TipoNotificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailDestino = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Cuerpo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaProgramada = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaEnviada = table.Column<DateTime>(type: "datetime", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Intentos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MensajeError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionCotizacion", x => x.NotificacionId);
                    table.ForeignKey(
                        name: "FK_NotificacionCotizacion_Cotizacion",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificacionCotizacion_CotizacionVersion",
                        column: x => x.VersionId,
                        principalTable: "CotizacionVersion",
                        principalColumn: "VersionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificacionCotizacion_Usuarios_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificacionCotizacion_Usuarios_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_CotizacionId_VersionId_TipoNotificacion",
                table: "NotificacionCotizacion",
                columns: new[] { "CotizacionId", "VersionId", "TipoNotificacion" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_CreatedBy",
                table: "NotificacionCotizacion",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_Estado_FechaProgramada",
                table: "NotificacionCotizacion",
                columns: new[] { "Estado", "FechaProgramada" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_ModifiedBy",
                table: "NotificacionCotizacion",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_VersionId",
                table: "NotificacionCotizacion",
                column: "VersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacionCotizacion");
        }
    }
}
