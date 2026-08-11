using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientResultId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestLabsId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PatientResultId",
                table: "Notifications",
                column: "PatientResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RequestLabsId",
                table: "Notifications",
                column: "RequestLabsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_PatientResults_PatientResultId",
                table: "Notifications",
                column: "PatientResultId",
                principalTable: "PatientResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_RequestLabs_RequestLabsId",
                table: "Notifications",
                column: "RequestLabsId",
                principalTable: "RequestLabs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_PatientResults_PatientResultId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_RequestLabs_RequestLabsId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PatientResultId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RequestLabsId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PatientResultId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RequestLabsId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");
        }
    }
}
