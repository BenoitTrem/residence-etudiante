using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class FixCampusDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programmes_Campuses_CampusId",
                table: "Programmes");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences");

            migrationBuilder.AddForeignKey(
                name: "FK_Programmes_Campuses_CampusId",
                table: "Programmes",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Programmes_Campuses_CampusId",
                table: "Programmes");

            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences");

            migrationBuilder.AddForeignKey(
                name: "FK_Programmes_Campuses_CampusId",
                table: "Programmes",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id");
        }
    }
}
