using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablasCompletasCotizacionesCorregido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interesado",
                columns: table => new
                {
                    InteresadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HubspotObjectId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HubspotObjectType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TipoInteresado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaUltSync = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interesado", x => x.InteresadoId);
                });

            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    ParametroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.ParametroId);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InteresadoId = table.Column<int>(type: "int", nullable: true),
                    EstadoActual = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VersionActual = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontoCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion", x => x.Id);
                    table.UniqueConstraint("AK_Cotizacion_CotizacionId", x => x.CotizacionId);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Interesado_InteresadoId",
                        column: x => x.InteresadoId,
                        principalTable: "Interesado",
                        principalColumn: "InteresadoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArchivoCotizacion",
                columns: table => new
                {
                    ArchivoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VersionArchivada = table.Column<int>(type: "int", nullable: true),
                    FechaArchivado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioArchiva = table.Column<int>(type: "int", nullable: true),
                    FechaReactivacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioReactiva = table.Column<int>(type: "int", nullable: true),
                    TipoArchivo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MotivoArchivado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivoCotizacion", x => x.ArchivoId);
                    table.ForeignKey(
                        name: "FK_ArchivoCotizacion_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionVersion",
                columns: table => new
                {
                    VersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroVersion = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    FechaVersion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreInteresado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailInteresado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmpresaInteresado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VersionActual = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    UsuarioCreacion = table.Column<int>(type: "int", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CotizacionVersion", x => x.VersionId);
                    table.ForeignKey(
                        name: "FK_CotizacionVersion_Cotizacion_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizacion",
                        principalColumn: "CotizacionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleCotizacionVersion",
                columns: table => new
                {
                    DetalleVersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLinea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleCotizacionVersion", x => x.DetalleVersionId);
                    table.ForeignKey(
                        name: "FK_DetalleCotizacionVersion_CotizacionVersion_VersionId",
                        column: x => x.VersionId,
                        principalTable: "CotizacionVersion",
                        principalColumn: "VersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCotizacion",
                columns: table => new
                {
                    HistorialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionId = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEvento = table.Column<int>(type: "int", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCotizacion", x => x.HistorialId);
                    table.ForeignKey(
                        name: "FK_HistorialCotizacion_CotizacionVersion_VersionId",
                        column: x => x.VersionId,
                        principalTable: "CotizacionVersion",
                        principalColumn: "VersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivoCotizacion_CotizacionId",
                table: "ArchivoCotizacion",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivoCotizacion_FechaArchivado",
                table: "ArchivoCotizacion",
                column: "FechaArchivado");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_CotizacionId",
                table: "Cotizacion",
                column: "CotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_InteresadoId",
                table: "Cotizacion",
                column: "InteresadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionVersion_CotizacionId_NumeroVersion",
                table: "CotizacionVersion",
                columns: new[] { "CotizacionId", "NumeroVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCotizacionVersion_VersionId",
                table: "DetalleCotizacionVersion",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCotizacion_FechaEvento",
                table: "HistorialCotizacion",
                column: "FechaEvento");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCotizacion_VersionId",
                table: "HistorialCotizacion",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Interesado_HubspotObjectId",
                table: "Interesado",
                column: "HubspotObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivoCotizacion");

            migrationBuilder.DropTable(
                name: "DetalleCotizacionVersion");

            migrationBuilder.DropTable(
                name: "HistorialCotizacion");

            migrationBuilder.DropTable(
                name: "Parametros");

            migrationBuilder.DropTable(
                name: "CotizacionVersion");

            migrationBuilder.DropTable(
                name: "Cotizacion");

            migrationBuilder.DropTable(
                name: "Interesado");
        }
    }
}
