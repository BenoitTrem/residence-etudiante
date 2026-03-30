using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class ModificationDeAdresse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse_Numero",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Adresse_Rue",
                table: "Residences");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_AdresseString",
                table: "Residences",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse_AdresseString",
                table: "Residences");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Numero",
                table: "Residences",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Adresse_Rue",
                table: "Residences",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
