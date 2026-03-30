using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutResidenceAdresse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Residences");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_CodePostal",
                table: "Residences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Numero",
                table: "Residences",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Province",
                table: "Residences",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Rue",
                table: "Residences",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Ville",
                table: "Residences",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CampusId",
                table: "Residences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Residences_CampusId",
                table: "Residences",
                column: "CampusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Campus_CampusId",
                table: "Residences",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Campus_CampusId",
                table: "Residences");

            migrationBuilder.DropIndex(
                name: "IX_Residences_CampusId",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_CodePostal",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_Numero",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_Province",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_Rue",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_Ville",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "CampusId",
                table: "Residences");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Residences",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
