using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProgressTables : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentModuleProgresses",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId1 = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    Passed = table.Column<bool>(type: "boolean", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "StudentModules",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ExamPassed = table.Column<bool>(type: "boolean", nullable: false),
                    ExamAttempts = table.Column<int>(type: "integer", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentModules_Students_StudentId",
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

            migrationBuilder.CreateIndex(
                name: "IX_StudentModules_ModuleId",
                table: "StudentModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentModules_StudentId",
                table: "StudentModules",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "StudentModuleProgresses");

            migrationBuilder.DropTable(
                name: "StudentModules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lessons");
        }
    }
}
