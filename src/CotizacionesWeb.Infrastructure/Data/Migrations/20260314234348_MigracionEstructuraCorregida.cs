using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CotizacionesWeb.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionEstructuraCorregida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    ParametroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.ParametroId);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    PermisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.PermisoId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "PermisosRoles",
                columns: table => new
                {
                    PermisoId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosRoles", x => new { x.PermisoId, x.RolId });
                    table.ForeignKey(
                        name: "FK_PermisosRoles_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permisos",
                        principalColumn: "PermisoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermisosRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interesado",
                columns: table => new
                {
                    InteresadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HubspotObjectId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HubspotObjectType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TipoInteresado = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaUltSync = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interesado", x => x.InteresadoId);
                    table.ForeignKey(
                        name: "FK_Interesado_Usuarios_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interesado_Usuarios_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion",
                columns: table => new
                {
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InteresadoId = table.Column<int>(type: "int", nullable: true),
                    EstadoActual = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VersionActual = table.Column<int>(type: "int", nullable: false),
                    MontoCotizacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime", nullable: true),
                    FechaAceptacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    FechaRechazo = table.Column<DateTime>(type: "datetime", nullable: true),
                    EnviadoERP = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false, defaultValue: "N"),
                    FechaEnvioERP = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion", x => x.CotizacionId);
                    table.CheckConstraint("CK_Cotizacion_EnviadoERP", "[EnviadoERP] IN ('S', 'N')");
                    table.ForeignKey(
                        name: "FK_Cotizacion_Interesado_InteresadoId",
                        column: x => x.InteresadoId,
                        principalTable: "Interesado",
                        principalColumn: "InteresadoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Usuarios_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Usuarios_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
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
                    FechaArchivado = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioArchiva = table.Column<int>(type: "int", nullable: true),
                    FechaReactivacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioReactiva = table.Column<int>(type: "int", nullable: true),
                    TipoArchivo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MotivoArchivado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ArchivoCotizacion_Usuarios_UsuarioArchiva",
                        column: x => x.UsuarioArchiva,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArchivoCotizacion_Usuarios_UsuarioReactiva",
                        column: x => x.UsuarioReactiva,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CotizacionVersion",
                columns: table => new
                {
                    VersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroVersion = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    FechaVersion = table.Column<DateTime>(type: "datetime", nullable: false),
                    NombreInteresado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailInteresado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmpresaInteresado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TipoCambio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VersionActual = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CotizacionVersion_Usuarios_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CotizacionVersion_Usuarios_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
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
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_DetalleCotizacionVersion_Usuarios_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleCotizacionVersion_Usuarios_ModifiedBy",
                        column: x => x.ModifiedBy,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCotizacion",
                columns: table => new
                {
                    HistorialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionId = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioEvento = table.Column<int>(type: "int", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_HistorialCotizacion_Usuarios_UsuarioEvento",
                        column: x => x.UsuarioEvento,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_ArchivoCotizacion_UsuarioArchiva",
                table: "ArchivoCotizacion",
                column: "UsuarioArchiva");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivoCotizacion_UsuarioReactiva",
                table: "ArchivoCotizacion",
                column: "UsuarioReactiva");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_CreatedBy",
                table: "Cotizacion",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_InteresadoId",
                table: "Cotizacion",
                column: "InteresadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_ModifiedBy",
                table: "Cotizacion",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionVersion_CotizacionId_NumeroVersion",
                table: "CotizacionVersion",
                columns: new[] { "CotizacionId", "NumeroVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionVersion_CreatedBy",
                table: "CotizacionVersion",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CotizacionVersion_ModifiedBy",
                table: "CotizacionVersion",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCotizacionVersion_CreatedBy",
                table: "DetalleCotizacionVersion",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCotizacionVersion_ModifiedBy",
                table: "DetalleCotizacionVersion",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCotizacionVersion_VersionId",
                table: "DetalleCotizacionVersion",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCotizacion_FechaEvento",
                table: "HistorialCotizacion",
                column: "FechaEvento");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCotizacion_UsuarioEvento",
                table: "HistorialCotizacion",
                column: "UsuarioEvento");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCotizacion_VersionId",
                table: "HistorialCotizacion",
                column: "VersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Interesado_CreatedBy",
                table: "Interesado",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Interesado_HubspotObjectId",
                table: "Interesado",
                column: "HubspotObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Interesado_ModifiedBy",
                table: "Interesado",
                column: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermisosRoles_RolId",
                table: "PermisosRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolId",
                table: "UsuarioRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
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
                name: "PermisosRoles");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "CotizacionVersion");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Cotizacion");

            migrationBuilder.DropTable(
                name: "Interesado");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
