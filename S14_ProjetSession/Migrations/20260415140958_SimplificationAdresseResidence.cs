using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class SimplificationAdresseResidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences");

            migrationBuilder.RenameColumn(
                name: "Adresse_Ville",
                table: "Residences",
                newName: "Ville");

            migrationBuilder.RenameColumn(
                name: "Adresse_Province",
                table: "Residences",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "Adresse_CodePostal",
                table: "Residences",
                newName: "CodePostal");

            migrationBuilder.RenameColumn(
                name: "Adresse_AdresseString",
                table: "Residences",
                newName: "AdresseLigne");

            migrationBuilder.AlterColumn<int>(
                name: "CampusId",
                table: "Residences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences");

            migrationBuilder.RenameColumn(
                name: "Ville",
                table: "Residences",
                newName: "Adresse_Ville");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Residences",
                newName: "Adresse_Province");

            migrationBuilder.RenameColumn(
                name: "CodePostal",
                table: "Residences",
                newName: "Adresse_CodePostal");

            migrationBuilder.RenameColumn(
                name: "AdresseLigne",
                table: "Residences",
                newName: "Adresse_AdresseString");

            migrationBuilder.AlterColumn<int>(
                name: "CampusId",
                table: "Residences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Campuses_CampusId",
                table: "Residences",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
