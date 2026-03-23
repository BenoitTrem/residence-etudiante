using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutSystemeResidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniteId",
                table: "Etudiants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EtudiantId",
                table: "Demandes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Residence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residence", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacite = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    ResidenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Unite_Residence_ResidenceId",
                        column: x => x.ResidenceId,
                        principalTable: "Residence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_UniteId",
                table: "Etudiants",
                column: "UniteId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_EtudiantId",
                table: "Demandes",
                column: "EtudiantId");

            migrationBuilder.CreateIndex(
                name: "IX_Unite_ResidenceId",
                table: "Unite",
                column: "ResidenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes",
                column: "EtudiantId",
                principalTable: "Etudiants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_Unite_UniteId",
                table: "Etudiants",
                column: "UniteId",
                principalTable: "Unite",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Etudiants_EtudiantId",
                table: "Demandes");

            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_Unite_UniteId",
                table: "Etudiants");

            migrationBuilder.DropTable(
                name: "Unite");

            migrationBuilder.DropTable(
                name: "Residence");

            migrationBuilder.DropIndex(
                name: "IX_Etudiants_UniteId",
                table: "Etudiants");

            migrationBuilder.DropIndex(
                name: "IX_Demandes_EtudiantId",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "UniteId",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "EtudiantId",
                table: "Demandes");
        }
    }
}
