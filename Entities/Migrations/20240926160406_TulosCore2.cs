using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TulosCore2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Favorites",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CartItems",
                newName: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Favorites",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "CartItems",
                newName: "UserId");
        }
    }
}
