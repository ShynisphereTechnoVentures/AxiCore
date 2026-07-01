using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudentModuleProgress : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "StudentModuleProgresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.CreateTable(
                name: "StudentModuleProgresses",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId1 = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Passed = table.Column<bool>(type: "boolean", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentModuleProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentModuleProgresses_Modules_ModuleId1",
                        column: x => x.ModuleId1,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentModuleProgresses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentModuleProgresses_ModuleId1",
                table: "StudentModuleProgresses",
                column: "ModuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentModuleProgresses_StudentId",
                table: "StudentModuleProgresses",
                column: "StudentId");
        }
    }
}
