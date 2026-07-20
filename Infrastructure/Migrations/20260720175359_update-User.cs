using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BasePersons_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_BasePersons_AspNetUsers_ApplicationUserId",
                table: "BasePersons");

            migrationBuilder.DropIndex(
                name: "IX_BasePersons_ApplicationUserId",
                table: "BasePersons");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "BasePersons");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BasePersons_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                principalTable: "BasePersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BasePersons_PersonId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "BasePersons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasePersons_ApplicationUserId",
                table: "BasePersons",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BasePersons_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                principalTable: "BasePersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BasePersons_AspNetUsers_ApplicationUserId",
                table: "BasePersons",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
