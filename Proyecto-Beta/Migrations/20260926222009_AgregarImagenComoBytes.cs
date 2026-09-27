using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_Beta.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenComoBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenNombreArchivo",
                table: "Tareas");

            migrationBuilder.AddColumn<string>(
                name: "ImagenContentType",
                table: "Tareas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenDatos",
                table: "Tareas",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenContentType",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "ImagenDatos",
                table: "Tareas");

            migrationBuilder.AddColumn<string>(
                name: "ImagenNombreArchivo",
                table: "Tareas",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
