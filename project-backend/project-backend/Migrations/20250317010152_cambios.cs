using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_backend.Migrations
{
    /// <inheritdoc />
    public partial class cambios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Usuarios_UsuarioId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacantes_Usuarios_UsuarioId",
                table: "Vacantes");

            migrationBuilder.DropIndex(
                name: "IX_Vacantes_UsuarioId",
                table: "Vacantes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Vacantes");

            migrationBuilder.RenameColumn(
                name: "RolId",
                table: "Usuarios",
                newName: "IdRol");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                newName: "IX_Usuarios_IdRol");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "CVs",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_CVs_UsuarioId",
                table: "CVs",
                newName: "IX_CVs_IdUsuario");

            migrationBuilder.AddColumn<int>(
                name: "IdCV",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VacanteUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdVacante = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacanteUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacanteUsuario_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacanteUsuario_Vacantes_IdVacante",
                        column: x => x.IdVacante,
                        principalTable: "Vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacanteUsuario_IdUsuario",
                table: "VacanteUsuario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_VacanteUsuario_IdVacante",
                table: "VacanteUsuario",
                column: "IdVacante");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Usuarios_IdUsuario",
                table: "CVs",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Usuarios_IdUsuario",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "VacanteUsuario");

            migrationBuilder.DropColumn(
                name: "IdCV",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "Usuarios",
                newName: "RolId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                newName: "IX_Usuarios_RolId");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "CVs",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_CVs_IdUsuario",
                table: "CVs",
                newName: "IX_CVs_UsuarioId");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Vacantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vacantes_UsuarioId",
                table: "Vacantes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Usuarios_UsuarioId",
                table: "CVs",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacantes_Usuarios_UsuarioId",
                table: "Vacantes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
