using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class changementModelDemande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demandes_Genres_PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.DropIndex(
                name: "IX_Demandes_PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.DropColumn(
                name: "PreferencesGenreId",
                table: "Demandes");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Programmes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Programmes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Genres",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Campus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Abreviation",
                table: "Campus",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DemandeGenre",
                columns: table => new
                {
                    PreferencesGenreId = table.Column<int>(type: "int", nullable: false),
                    demandesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandeGenre", x => new { x.PreferencesGenreId, x.demandesId });
                    table.ForeignKey(
                        name: "FK_DemandeGenre_Demandes_demandesId",
                        column: x => x.demandesId,
                        principalTable: "Demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DemandeGenre_Genres_PreferencesGenreId",
                        column: x => x.PreferencesGenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemandeGenre_demandesId",
                table: "DemandeGenre",
                column: "demandesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemandeGenre");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Programmes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Programmes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Genres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "PreferencesGenreId",
                table: "Demandes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Campus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Abreviation",
                table: "Campus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_PreferencesGenreId",
                table: "Demandes",
                column: "PreferencesGenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demandes_Genres_PreferencesGenreId",
                table: "Demandes",
                column: "PreferencesGenreId",
                principalTable: "Genres",
                principalColumn: "Id");
        }
    }
}
