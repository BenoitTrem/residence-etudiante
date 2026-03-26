using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRelationEtudiantUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Etudiants",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_ApplicationUserId",
                table: "Etudiants",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_AspNetUsers_ApplicationUserId",
                table: "Etudiants",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_AspNetUsers_ApplicationUserId",
                table: "Etudiants");

            migrationBuilder.DropIndex(
                name: "IX_Etudiants_ApplicationUserId",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Etudiants");
        }
    }
}
