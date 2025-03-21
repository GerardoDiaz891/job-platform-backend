using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_backend.Migrations
{
    /// <inheritdoc />
    public partial class CVp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdVacante",
                table: "CVs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CVs_IdVacante",
                table: "CVs",
                column: "IdVacante");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Vacantes_IdVacante",
                table: "CVs",
                column: "IdVacante",
                principalTable: "Vacantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Vacantes_IdVacante",
                table: "CVs");

            migrationBuilder.DropIndex(
                name: "IX_CVs_IdVacante",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "IdVacante",
                table: "CVs");
        }
    }
}
