using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class NumeroPeutEtreNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "Unites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites",
                columns: new[] { "Numero", "ResidenceId" },
                unique: true,
                filter: "[Numero] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "Unites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unites_Numero_ResidenceId",
                table: "Unites",
                columns: new[] { "Numero", "ResidenceId" },
                unique: true);
        }
    }
}
