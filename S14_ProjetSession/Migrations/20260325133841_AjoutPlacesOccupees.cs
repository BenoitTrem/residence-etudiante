using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S14_ProjetSession.Migrations
{
    /// <inheritdoc />
    public partial class AjoutPlacesOccupees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlacesOccupees",
                table: "Unites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlacesOccupees",
                table: "Unites");
        }
    }
}
