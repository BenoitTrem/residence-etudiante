using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDeUnite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Unite_UniteId",
                table: "Etudiants");

            migrationBuilder.DropForeignKey(
                name: "FK_Unite_Residence_ResidenceId",
                table: "Unite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unite",
                table: "Unite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Residence",
                table: "Residence");

            migrationBuilder.RenameTable(
                name: "Unite",
                newName: "Unites");

            migrationBuilder.RenameTable(
                name: "Residence",
                newName: "Residences");

            migrationBuilder.RenameIndex(
                name: "IX_Unite_ResidenceId",
                table: "Unites",
                newName: "IX_Unites_ResidenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unites",
                table: "Unites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Residences",
                table: "Residences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Unites_UniteId",
                table: "Etudiants",
                column: "UniteId",
                principalTable: "Unites",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Unites_Residences_ResidenceId",
                table: "Unites",
                column: "ResidenceId",
                principalTable: "Residences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Unites_UniteId",
                table: "Etudiants");

            migrationBuilder.DropForeignKey(
                name: "FK_Unites_Residences_ResidenceId",
                table: "Unites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Unites",
                table: "Unites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Residences",
                table: "Residences");

            migrationBuilder.RenameTable(
                name: "Unites",
                newName: "Unite");

            migrationBuilder.RenameTable(
                name: "Residences",
                newName: "Residence");

            migrationBuilder.RenameIndex(
                name: "IX_Unites_ResidenceId",
                table: "Unite",
                newName: "IX_Unite_ResidenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Unite",
                table: "Unite",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Residence",
                table: "Residence",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Unite_UniteId",
                table: "Etudiants",
                column: "UniteId",
                principalTable: "Unite",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Unite_Residence_ResidenceId",
                table: "Unite",
                column: "ResidenceId",
                principalTable: "Residence",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
