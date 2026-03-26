using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjouterSemestre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrefDurees");

            migrationBuilder.DropColumn(
                name: "JumelageVolontaire",
                table: "Demandes");

            migrationBuilder.RenameColumn(
                name: "Session",
                table: "Demandes",
                newName: "SemestreId");

            migrationBuilder.RenameColumn(
                name: "PreferencesGenre",
                table: "Demandes",
                newName: "TelephoneUrgence");

            migrationBuilder.RenameColumn(
                name: "NomJumelage",
                table: "Demandes",
                newName: "TelephoneGarant");

            migrationBuilder.RenameColumn(
                name: "CourrielJumelage",
                table: "Demandes",
                newName: "PrenomGarant");

            migrationBuilder.AddColumn<string>(
                name: "CourrielGarant",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourrielParent",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNaissanceGarant",
                table: "Demandes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LienParenteUrgence",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomGarant",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomParent",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomUrgence",
                table: "Demandes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PreferencesGenreId",
                table: "Demandes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Jumelage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Courriel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DemandeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jumelage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jumelage_Demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "Demandes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Semestre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomSemestre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semestre", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_PreferencesGenreId",
                table: "Demandes",
                column: "PreferencesGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_SemestreId",
                table: "Demandes",
                column: "SemestreId");

            migrationBuilder.CreateIndex(
                name: "IX_Jumelage_DemandeId",
                table: "Jumelage",
                column: "DemandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Genres_PreferencesGenreId",
                table: "Demandes",
                column: "PreferencesGenreId",
                principalTable: "Genres",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Semestre_SemestreId",
                table: "Demandes",
                column: "SemestreId",
                principalTable: "Semestre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Genres_PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Semestre_SemestreId",
                table: "Demandes");

            migrationBuilder.DropTable(
                name: "Jumelage");

            migrationBuilder.DropTable(
                name: "Semestre");

            migrationBuilder.DropIndex(
                name: "IX_Demandes_PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.DropIndex(
                name: "IX_Demandes_SemestreId",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "CourrielGarant",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "CourrielParent",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "DateNaissanceGarant",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "LienParenteUrgence",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "NomGarant",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "NomParent",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "NomUrgence",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.RenameColumn(
                name: "TelephoneUrgence",
                table: "Demandes",
                newName: "PreferencesGenre");

            migrationBuilder.RenameColumn(
                name: "TelephoneGarant",
                table: "Demandes",
                newName: "NomJumelage");

            migrationBuilder.RenameColumn(
                name: "SemestreId",
                table: "Demandes",
                newName: "Session");

            migrationBuilder.RenameColumn(
                name: "PrenomGarant",
                table: "Demandes",
                newName: "CourrielJumelage");

            migrationBuilder.AddColumn<bool>(
                name: "JumelageVolontaire",
                table: "Demandes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PrefDurees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duree = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrefDurees", x => x.Id);
                });
        }
    }
}
