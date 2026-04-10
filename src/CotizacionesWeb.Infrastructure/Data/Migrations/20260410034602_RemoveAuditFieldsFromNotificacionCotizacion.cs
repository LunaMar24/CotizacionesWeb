using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditFieldsFromNotificacionCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificacionCotizacion_Usuarios_CreatedBy",
                table: "NotificacionCotizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificacionCotizacion_Usuarios_ModifiedBy",
                table: "NotificacionCotizacion");

            migrationBuilder.DropIndex(
                name: "IX_NotificacionCotizacion_CreatedBy",
                table: "NotificacionCotizacion");

            migrationBuilder.DropIndex(
                name: "IX_NotificacionCotizacion_ModifiedBy",
                table: "NotificacionCotizacion");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NotificacionCotizacion");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "NotificacionCotizacion");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "NotificacionCotizacion");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "NotificacionCotizacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NotificacionCotizacion",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "NotificacionCotizacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "NotificacionCotizacion",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "NotificacionCotizacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_CreatedBy",
                table: "NotificacionCotizacion",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionCotizacion_ModifiedBy",
                table: "NotificacionCotizacion",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificacionCotizacion_Usuarios_CreatedBy",
                table: "NotificacionCotizacion",
                column: "CreatedBy",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificacionCotizacion_Usuarios_ModifiedBy",
                table: "NotificacionCotizacion",
                column: "ModifiedBy",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
