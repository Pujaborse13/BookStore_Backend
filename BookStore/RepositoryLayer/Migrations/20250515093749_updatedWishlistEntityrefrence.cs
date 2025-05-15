using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class updatedWishlistEntityrefrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Books_BookEntityId",
                table: "WishList");

            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Users_UserEntityUserId",
                table: "WishList");

            migrationBuilder.DropIndex(
                name: "IX_WishList_BookEntityId",
                table: "WishList");

            migrationBuilder.DropIndex(
                name: "IX_WishList_UserEntityUserId",
                table: "WishList");

            migrationBuilder.DropColumn(
                name: "BookEntityId",
                table: "WishList");

            migrationBuilder.DropColumn(
                name: "UserEntityUserId",
                table: "WishList");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_AddedBy",
                table: "WishList",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_BookId",
                table: "WishList",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Users_AddedBy",
                table: "WishList",
                column: "AddedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Books_BookId",
                table: "WishList",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Users_AddedBy",
                table: "WishList");

            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Books_BookId",
                table: "WishList");

            migrationBuilder.DropIndex(
                name: "IX_WishList_AddedBy",
                table: "WishList");

            migrationBuilder.DropIndex(
                name: "IX_WishList_BookId",
                table: "WishList");

            migrationBuilder.AddColumn<int>(
                name: "BookEntityId",
                table: "WishList",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserEntityUserId",
                table: "WishList",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishList_BookEntityId",
                table: "WishList",
                column: "BookEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_UserEntityUserId",
                table: "WishList",
                column: "UserEntityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Books_BookEntityId",
                table: "WishList",
                column: "BookEntityId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Users_UserEntityUserId",
                table: "WishList",
                column: "UserEntityUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
