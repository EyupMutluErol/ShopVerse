using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopVerse.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeCouponUserOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Coupons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Coupons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_AppUserId",
                table: "Coupons",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
