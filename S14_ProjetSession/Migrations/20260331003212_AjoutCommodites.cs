using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCommodites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commodites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResidenceCommodites",
                columns: table => new
                {
                    ResidenceId = table.Column<int>(type: "int", nullable: false),
                    CommoditeId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidenceCommodites", x => new { x.ResidenceId, x.CommoditeId });
                    table.ForeignKey(
                        name: "FK_ResidenceCommodites_Commodites_CommoditeId",
                        column: x => x.CommoditeId,
                        principalTable: "Commodites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResidenceCommodites_Residences_ResidenceId",
                        column: x => x.ResidenceId,
                        principalTable: "Residences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceCommodites_CommoditeId",
                table: "ResidenceCommodites",
                column: "CommoditeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResidenceCommodites");

            migrationBuilder.DropTable(
                name: "Commodites");
        }
    }
}
