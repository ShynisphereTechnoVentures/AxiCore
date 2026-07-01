using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentLessonProgress : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.CreateTable(
                name: "StudentLessonProgresses",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: true),
                    MentorRemarks = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentLessonProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLessonProgresses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLessonProgresses_Users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentLessonProgresses_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLessonProgresses_LessonId",
                table: "StudentLessonProgresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLessonProgresses_ReviewedById",
                table: "StudentLessonProgresses",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLessonProgresses_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "StudentLessonProgresses");
        }
    }
}
