using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Campuses_CampusId",
                table: "Etudiants");

            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Programmes_ProgrammeId",
                table: "Etudiants");

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Campuses_CampusId",
                table: "Etudiants",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Programmes_ProgrammeId",
                table: "Etudiants",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Campuses_CampusId",
                table: "Etudiants");

            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Programmes_ProgrammeId",
                table: "Etudiants");

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Campuses_CampusId",
                table: "Etudiants",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Programmes_ProgrammeId",
                table: "Etudiants",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
