using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_RequestedLabtestsEntity_withStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestLabsLabTests");

            migrationBuilder.CreateTable(
                name: "RequestLabTests",
                columns: table => new
                {
                    RequestLabId = table.Column<int>(type: "int", nullable: false),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLabTests", x => new { x.RequestLabId, x.LabTestId });
                    table.ForeignKey(
                        name: "FK_RequestLabTests_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestLabTests_RequestLabs_RequestLabId",
                        column: x => x.RequestLabId,
                        principalTable: "RequestLabs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestLabTests_LabTestId",
                table: "RequestLabTests",
                column: "LabTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestLabTests");

            migrationBuilder.CreateTable(
                name: "RequestLabsLabTests",
                columns: table => new
                {
                    LabTestsId = table.Column<int>(type: "int", nullable: false),
                    RequestLabsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLabsLabTests", x => new { x.LabTestsId, x.RequestLabsId });
                    table.ForeignKey(
                        name: "FK_RequestLabsLabTests_LabTests_LabTestsId",
                        column: x => x.LabTestsId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestLabsLabTests_RequestLabs_RequestLabsId",
                        column: x => x.RequestLabsId,
                        principalTable: "RequestLabs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestLabsLabTests_RequestLabsId",
                table: "RequestLabsLabTests",
                column: "RequestLabsId");
        }
    }
}
