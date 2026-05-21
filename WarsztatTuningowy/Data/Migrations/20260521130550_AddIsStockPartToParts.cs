using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarsztatTuningowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsStockPartToParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStockPart",
                table: "Parts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStockPart",
                table: "Parts");
        }
    }
}
