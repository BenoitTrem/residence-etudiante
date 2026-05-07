using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class correctionDeleteCascadeDemande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Unites_UniteId",
                table: "Demandes");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Unites_UniteId",
                table: "Demandes",
                column: "UniteId",
                principalTable: "Unites",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Unites_UniteId",
                table: "Demandes");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Unites_UniteId",
                table: "Demandes",
                column: "UniteId",
                principalTable: "Unites",
                principalColumn: "Id");
        }
    }
}
