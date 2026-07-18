using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Doctors_DoctorId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DoctorId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Departments",
                newName: "HeadDoctorId");

            migrationBuilder.RenameColumn(
                name: "DepartmentMangager",
                table: "Departments",
                newName: "HeadDoctor");

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Departments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Departments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HeadDoctorId_Unique",
                table: "Departments",
                column: "HeadDoctorId",
                unique: true,
                filter: "[HeadDoctorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Status",
                table: "Departments",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Doctors_HeadDoctorId",
                table: "Departments",
                column: "HeadDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Doctors_HeadDoctorId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_HeadDoctorId_Unique",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Status",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "HeadDoctorId",
                table: "Departments",
                newName: "DoctorId");

            migrationBuilder.RenameColumn(
                name: "HeadDoctor",
                table: "Departments",
                newName: "DepartmentMangager");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DoctorId",
                table: "Departments",
                column: "DoctorId",
                unique: true,
                filter: "[DoctorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Doctors_DoctorId",
                table: "Departments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
