using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopVerse.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CategoryId",
                table: "Coupons",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Categories_CategoryId",
                table: "Coupons",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Categories_CategoryId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CategoryId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Coupons");
        }
    }
}
