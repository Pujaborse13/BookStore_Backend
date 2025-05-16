using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class updatedCartEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Books_BookEntityId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_UserEntityUserId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_BookEntityId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserEntityUserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "BookEntityId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UserEntityUserId",
                table: "Cart");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_BookId",
                table: "Cart",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CustomerId",
                table: "Cart",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Books_BookId",
                table: "Cart",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_CustomerId",
                table: "Cart",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Books_BookId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_CustomerId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_BookId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_CustomerId",
                table: "Cart");

            migrationBuilder.AddColumn<int>(
                name: "BookEntityId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserEntityUserId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_BookEntityId",
                table: "Cart",
                column: "BookEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserEntityUserId",
                table: "Cart",
                column: "UserEntityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Books_BookEntityId",
                table: "Cart",
                column: "BookEntityId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_UserEntityUserId",
                table: "Cart",
                column: "UserEntityUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
