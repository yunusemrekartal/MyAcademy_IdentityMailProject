using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class _mig_AddMessageCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "UserMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "messageCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_messageCategories_CategoryId",
                table: "UserMessages",
                column: "CategoryId",
                principalTable: "messageCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_messageCategories_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropTable(
                name: "messageCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserMessages");
        }
    }
}
