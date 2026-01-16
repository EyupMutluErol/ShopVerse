using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopVerse.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceLimitsToCampaignAndCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxProductPrice",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinProductPrice",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxProductPrice",
                table: "Campaigns",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinProductPrice",
                table: "Campaigns",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxProductPrice",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MinProductPrice",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MaxProductPrice",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "MinProductPrice",
                table: "Campaigns");
        }
    }
}
