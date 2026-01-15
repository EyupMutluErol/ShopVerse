using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopVerse.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Coupons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Coupons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_AppUserId",
                table: "Coupons",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_AppUserId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Coupons");
        }
    }
}
