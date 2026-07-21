using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddElementEntityRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestElements_TestElements_TestElementId",
                table: "LabTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientResultElements_TestElements_TestElementId",
                table: "PatientResultElements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabTestElements",
                table: "LabTestElements");

            migrationBuilder.DropIndex(
                name: "IX_LabTestElements_TestElementId",
                table: "LabTestElements");

            migrationBuilder.RenameColumn(
                name: "TestElementId",
                table: "PatientResultElements",
                newName: "ElementId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientResultElements_TestElementId",
                table: "PatientResultElements",
                newName: "IX_PatientResultElements_ElementId");

            migrationBuilder.RenameColumn(
                name: "TestElementId",
                table: "LabTestElements",
                newName: "DisplayOrder");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "PatientResultElements",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "PatientResultElements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ElementId",
                table: "LabTestElements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "LabTestElements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabTestElements",
                table: "LabTestElements",
                columns: new[] { "LabTestId", "ElementId" });

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceRange = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestElements_ElementId",
                table: "LabTestElements",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_Name",
                table: "Elements",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestElements_Elements_ElementId",
                table: "LabTestElements",
                column: "ElementId",
                principalTable: "Elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientResultElements_Elements_ElementId",
                table: "PatientResultElements",
                column: "ElementId",
                principalTable: "Elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTestElements_Elements_ElementId",
                table: "LabTestElements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientResultElements_Elements_ElementId",
                table: "PatientResultElements");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabTestElements",
                table: "LabTestElements");

            migrationBuilder.DropIndex(
                name: "IX_LabTestElements_ElementId",
                table: "LabTestElements");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "PatientResultElements");

            migrationBuilder.DropColumn(
                name: "ElementId",
                table: "LabTestElements");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "LabTestElements");

            migrationBuilder.RenameColumn(
                name: "ElementId",
                table: "PatientResultElements",
                newName: "TestElementId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientResultElements_ElementId",
                table: "PatientResultElements",
                newName: "IX_PatientResultElements_TestElementId");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "LabTestElements",
                newName: "TestElementId");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "PatientResultElements",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabTestElements",
                table: "LabTestElements",
                columns: new[] { "LabTestId", "TestElementId" });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestElements_TestElementId",
                table: "LabTestElements",
                column: "TestElementId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTestElements_TestElements_TestElementId",
                table: "LabTestElements",
                column: "TestElementId",
                principalTable: "TestElements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientResultElements_TestElements_TestElementId",
                table: "PatientResultElements",
                column: "TestElementId",
                principalTable: "TestElements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
