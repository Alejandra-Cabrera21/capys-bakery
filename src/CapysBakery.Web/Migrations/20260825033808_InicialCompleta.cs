using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapysBakery.Web.Migrations
{
    /// <inheritdoc />
    public partial class InicialCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alergeno",
                columns: table => new
                {
                    id_alergeno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alergeno", x => x.id_alergeno);
                });

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "cuenta_bancaria",
                columns: table => new
                {
                    id_cuenta_bancaria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_metodo_pago = table.Column<int>(type: "int", nullable: false),
                    banco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tipo_cuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    numero_cuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    titular = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuenta_bancaria", x => x.id_cuenta_bancaria);
                });

            migrationBuilder.CreateTable(
                name: "estado_pedido",
                columns: table => new
                {
                    id_estado_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_pedido", x => x.id_estado_pedido);
                });

            migrationBuilder.CreateTable(
                name: "metodo_pago",
                columns: table => new
                {
                    id_metodo_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    solo_recoger = table.Column<bool>(type: "bit", nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metodo_pago", x => x.id_metodo_pago);
                });

            migrationBuilder.CreateTable(
                name: "modalidad_entrega",
                columns: table => new
                {
                    id_modalidad_entrega = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    requiere_direccion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modalidad_entrega", x => x.id_modalidad_entrega);
                });

            migrationBuilder.CreateTable(
                name: "producto",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    es_promocion = table.Column<bool>(type: "bit", nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false),
                    creado_por_correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto", x => x.id_producto);
                });

            migrationBuilder.CreateTable(
                name: "publicacion",
                columns: table => new
                {
                    id_publicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    categoria = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    resumen = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imagen_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    publicada = table.Column<bool>(type: "bit", nullable: false),
                    autor_correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    fecha_publicacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publicacion", x => x.id_publicacion);
                });

            migrationBuilder.CreateTable(
                name: "tipo_personalizacion",
                columns: table => new
                {
                    id_tipo_personalizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_personalizacion", x => x.id_tipo_personalizacion);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    creada_por_correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "imagen_producto",
                columns: table => new
                {
                    id_imagen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    url_imagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    orden = table.Column<int>(type: "int", nullable: false),
                    es_principal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagen_producto", x => x.id_imagen);
                    table.ForeignKey(
                        name: "FK_imagen_producto_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producto_alergeno",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_alergeno = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_alergeno", x => new { x.id_producto, x.id_alergeno });
                    table.ForeignKey(
                        name: "FK_prodalerg_alergeno",
                        column: x => x.id_alergeno,
                        principalTable: "alergeno",
                        principalColumn: "id_alergeno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prodalerg_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producto_categoria",
                columns: table => new
                {
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_categoria", x => new { x.id_producto, x.id_categoria });
                    table.ForeignKey(
                        name: "FK_prodcat_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prodcat_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producto_presentacion",
                columns: table => new
                {
                    id_presentacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    porciones = table.Column<int>(type: "int", nullable: true),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_presentacion", x => x.id_presentacion);
                    table.ForeignKey(
                        name: "FK_producto_presentacion_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "opcion_personalizacion",
                columns: table => new
                {
                    id_opcion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_tipo_personalizacion = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opcion_personalizacion", x => x.id_opcion);
                    table.ForeignKey(
                        name: "FK_opcion_personalizacion_tipo_personalizacion_id_tipo_personalizacion",
                        column: x => x.id_tipo_personalizacion,
                        principalTable: "tipo_personalizacion",
                        principalColumn: "id_tipo_personalizacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedido",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_pedido = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nombre_cliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    telefono_cliente = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    fecha_entrega_solicitada = table.Column<DateTime>(type: "date", nullable: false),
                    id_modalidad_entrega = table.Column<int>(type: "int", nullable: false),
                    direccion_o_punto_entrega = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    id_metodo_pago = table.Column<int>(type: "int", nullable: false),
                    id_estado_pedido = table.Column<int>(type: "int", nullable: false),
                    comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido", x => x.id_pedido);
                    table.ForeignKey(
                        name: "FK_pedido_estado_pedido_id_estado_pedido",
                        column: x => x.id_estado_pedido,
                        principalTable: "estado_pedido",
                        principalColumn: "id_estado_pedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_metodo_pago_id_metodo_pago",
                        column: x => x.id_metodo_pago,
                        principalTable: "metodo_pago",
                        principalColumn: "id_metodo_pago",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_modalidad_entrega_id_modalidad_entrega",
                        column: x => x.id_modalidad_entrega,
                        principalTable: "modalidad_entrega",
                        principalColumn: "id_modalidad_entrega",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producto_opcion_personalizacion",
                columns: table => new
                {
                    id_producto_opcion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_opcion = table.Column<int>(type: "int", nullable: false),
                    precio_adicional = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_opcion_personalizacion", x => x.id_producto_opcion);
                    table.ForeignKey(
                        name: "FK_producto_opcion_personalizacion_opcion_personalizacion_id_opcion",
                        column: x => x.id_opcion,
                        principalTable: "opcion_personalizacion",
                        principalColumn: "id_opcion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_opcion_personalizacion_producto_id_producto",
                        column: x => x.id_producto,
                        principalTable: "producto",
                        principalColumn: "id_producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historial_estado_pedido",
                columns: table => new
                {
                    id_historial_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pedido = table.Column<int>(type: "int", nullable: false),
                    id_estado_pedido = table.Column<int>(type: "int", nullable: false),
                    fecha_cambio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historial_estado_pedido", x => x.id_historial_pedido);
                    table.ForeignKey(
                        name: "FK_historial_estado_pedido_estado_pedido_id_estado_pedido",
                        column: x => x.id_estado_pedido,
                        principalTable: "estado_pedido",
                        principalColumn: "id_estado_pedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historial_estado_pedido_pedido_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedido",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedido_detalle",
                columns: table => new
                {
                    id_detalle_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pedido = table.Column<int>(type: "int", nullable: false),
                    id_presentacion = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_detalle", x => x.id_detalle_pedido);
                    table.ForeignKey(
                        name: "FK_pedido_detalle_pedido_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedido",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_detalle_producto_presentacion_id_presentacion",
                        column: x => x.id_presentacion,
                        principalTable: "producto_presentacion",
                        principalColumn: "id_presentacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedido_detalle_personalizacion",
                columns: table => new
                {
                    id_detalle_personalizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_detalle_pedido = table.Column<int>(type: "int", nullable: false),
                    id_producto_opcion = table.Column<int>(type: "int", nullable: false),
                    precio_adicional_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_detalle_personalizacion", x => x.id_detalle_personalizacion);
                    table.ForeignKey(
                        name: "FK_pedido_detalle_personalizacion_pedido_detalle_id_detalle_pedido",
                        column: x => x.id_detalle_pedido,
                        principalTable: "pedido_detalle",
                        principalColumn: "id_detalle_pedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedido_detalle_personalizacion_producto_opcion_personalizacion_id_producto_opcion",
                        column: x => x.id_producto_opcion,
                        principalTable: "producto_opcion_personalizacion",
                        principalColumn: "id_producto_opcion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alergeno_nombre",
                table: "alergeno",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_nombre",
                table: "categoria",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estado_pedido_nombre",
                table: "estado_pedido",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_historial_estado_pedido_id_estado_pedido",
                table: "historial_estado_pedido",
                column: "id_estado_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_historial_estado_pedido_id_pedido",
                table: "historial_estado_pedido",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_imagen_producto_id_producto",
                table: "imagen_producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_metodo_pago_nombre",
                table: "metodo_pago",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modalidad_entrega_nombre",
                table: "modalidad_entrega",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opcion_personalizacion_id_tipo_personalizacion",
                table: "opcion_personalizacion",
                column: "id_tipo_personalizacion");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_codigo_pedido",
                table: "pedido",
                column: "codigo_pedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedido_id_estado_pedido",
                table: "pedido",
                column: "id_estado_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_id_metodo_pago",
                table: "pedido",
                column: "id_metodo_pago");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_id_modalidad_entrega",
                table: "pedido",
                column: "id_modalidad_entrega");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_id_usuario",
                table: "pedido",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalle_id_pedido",
                table: "pedido_detalle",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalle_id_presentacion",
                table: "pedido_detalle",
                column: "id_presentacion");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalle_personalizacion_id_detalle_pedido",
                table: "pedido_detalle_personalizacion",
                column: "id_detalle_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_detalle_personalizacion_id_producto_opcion",
                table: "pedido_detalle_personalizacion",
                column: "id_producto_opcion");

            migrationBuilder.CreateIndex(
                name: "IX_producto_alergeno_id_alergeno",
                table: "producto_alergeno",
                column: "id_alergeno");

            migrationBuilder.CreateIndex(
                name: "IX_producto_categoria_id_categoria",
                table: "producto_categoria",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_producto_opcion_personalizacion_id_opcion",
                table: "producto_opcion_personalizacion",
                column: "id_opcion");

            migrationBuilder.CreateIndex(
                name: "IX_producto_opcion_personalizacion_id_producto_id_opcion",
                table: "producto_opcion_personalizacion",
                columns: new[] { "id_producto", "id_opcion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_presentacion_id_producto",
                table: "producto_presentacion",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_tipo_personalizacion_nombre",
                table: "tipo_personalizacion",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_correo",
                table: "usuario",
                column: "correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cuenta_bancaria");

            migrationBuilder.DropTable(
                name: "historial_estado_pedido");

            migrationBuilder.DropTable(
                name: "imagen_producto");

            migrationBuilder.DropTable(
                name: "pedido_detalle_personalizacion");

            migrationBuilder.DropTable(
                name: "producto_alergeno");

            migrationBuilder.DropTable(
                name: "producto_categoria");

            migrationBuilder.DropTable(
                name: "publicacion");

            migrationBuilder.DropTable(
                name: "pedido_detalle");

            migrationBuilder.DropTable(
                name: "producto_opcion_personalizacion");

            migrationBuilder.DropTable(
                name: "alergeno");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "pedido");

            migrationBuilder.DropTable(
                name: "producto_presentacion");

            migrationBuilder.DropTable(
                name: "opcion_personalizacion");

            migrationBuilder.DropTable(
                name: "estado_pedido");

            migrationBuilder.DropTable(
                name: "metodo_pago");

            migrationBuilder.DropTable(
                name: "modalidad_entrega");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "producto");

            migrationBuilder.DropTable(
                name: "tipo_personalizacion");
        }
    }
}
