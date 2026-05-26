using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lotomoto.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarListingPriceFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "CarListings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "CarListings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "CarListings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "CarListings");
        }
    }
}
