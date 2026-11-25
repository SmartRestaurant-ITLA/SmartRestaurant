using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRestaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Invoices",
                type: "rowversion",
                rowVersion: true,
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Invoices");
        }
    }
}
