using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteForDemandes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes");

            migrationBuilder.DropForeignKey(
                name: "FK_Jumelage_Demandes_DemandeId",
                table: "Jumelage");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jumelage_Demandes_DemandeId",
                table: "Jumelage",
                column: "DemandeId",
                principalTable: "Demandes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes");

            migrationBuilder.DropForeignKey(
                name: "FK_Jumelage_Demandes_DemandeId",
                table: "Jumelage");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jumelage_Demandes_DemandeId",
                table: "Jumelage",
                column: "DemandeId",
                principalTable: "Demandes",
                principalColumn: "Id");
        }
    }
}
