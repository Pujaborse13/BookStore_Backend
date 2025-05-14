using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class updatedcart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SinglUnitPrice = table.Column<decimal>(nullable: false),
                    IsPurchased = table.Column<bool>(nullable: false),
                    UserEntityUserId = table.Column<int>(nullable: true),
                    BookEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Cart_Books_BookEntityId",
                        column: x => x.BookEntityId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cart_Users_UserEntityUserId",
                        column: x => x.UserEntityUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cart_BookEntityId",
                table: "Cart",
                column: "BookEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserEntityUserId",
                table: "Cart",
                column: "UserEntityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");
        }
    }
}
