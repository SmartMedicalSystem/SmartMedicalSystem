using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateLabTechincian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "LabTechnicians",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Contact",
                table: "LabTechnicians",
                newName: "EmployeeId");

            migrationBuilder.AddColumn<bool>(
                name: "AccountActive",
                table: "LabTechnicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "LabTechnicians",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AllowLogin",
                table: "LabTechnicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AlternativePhone",
                table: "LabTechnicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "LabTechnicians",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "EmploymentStatus",
                table: "LabTechnicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "LabTechnicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "JoiningDate",
                table: "LabTechnicians",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Laboratory",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "LabTechnicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "LabTechnicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "LabTechnicians",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "LabTechnicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNotifications",
                table: "LabTechnicians",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "LabTechnicians",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkShift",
                table: "LabTechnicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "LabTechnicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_EmployeeId",
                table: "LabTechnicians",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LabTechnicians_EmployeeId",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "AccountActive",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "AllowLogin",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "AlternativePhone",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "City",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "EmploymentStatus",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "JoiningDate",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Laboratory",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "ReceiveNotifications",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "WorkShift",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "LabTechnicians");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "LabTechnicians",
                newName: "Contact");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "LabTechnicians",
                newName: "Name");
        }
    }
}
