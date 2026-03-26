using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutContraintesUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Residences",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites",
                columns: new[] { "Numero", "ResidenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Residences_Nom",
                table: "Residences",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites");

            migrationBuilder.DropIndex(
                name: "IX_Residences_Nom",
                table: "Residences");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Residences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
