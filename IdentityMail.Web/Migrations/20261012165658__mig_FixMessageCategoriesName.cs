using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class _mig_FixMessageCategoriesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_messageCategories_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messageCategories",
                table: "messageCategories");

            migrationBuilder.RenameTable(
                name: "messageCategories",
                newName: "MessageCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageCategories",
                table: "MessageCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_MessageCategories_CategoryId",
                table: "UserMessages",
                column: "CategoryId",
                principalTable: "MessageCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_MessageCategories_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageCategories",
                table: "MessageCategories");

            migrationBuilder.RenameTable(
                name: "MessageCategories",
                newName: "messageCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messageCategories",
                table: "messageCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_messageCategories_CategoryId",
                table: "UserMessages",
                column: "CategoryId",
                principalTable: "messageCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
