using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarsztatTuningowy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddElectronicStatementToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LegalConsequencesAccepted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ModificationScopeAccepted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatementAcceptedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatementAcceptedBy",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WarrantyLossAccepted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegalConsequencesAccepted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ModificationScopeAccepted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StatementAcceptedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StatementAcceptedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarrantyLossAccepted",
                table: "Orders");
        }
    }
}
