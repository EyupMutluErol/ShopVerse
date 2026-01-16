using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopVerse.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderStateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderState",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderState",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
