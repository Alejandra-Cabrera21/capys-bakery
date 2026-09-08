using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapysBakery.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarComentariosBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comentario_publicacion",
                columns: table => new
                {
                    id_comentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_publicacion = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    contenido = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentario_publicacion", x => x.id_comentario);
                    table.ForeignKey(
                        name: "FK_comentario_publicacion_publicacion_id_publicacion",
                        column: x => x.id_publicacion,
                        principalTable: "publicacion",
                        principalColumn: "id_publicacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comentario_publicacion_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comentario_publicacion_id_publicacion",
                table: "comentario_publicacion",
                column: "id_publicacion");

            migrationBuilder.CreateIndex(
                name: "IX_comentario_publicacion_id_usuario",
                table: "comentario_publicacion",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentario_publicacion");
        }
    }
}
