using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignments : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DueAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissions",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionLink = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_AssignmentSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_BatchId",
                table: "Assignments",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_LessonId",
                table: "Assignments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_AssignmentId_StudentId",
                table: "AssignmentSubmissions",
                columns: new[]{ "AssignmentId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_StudentId",
                table: "AssignmentSubmissions",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "AssignmentSubmissions");

            migrationBuilder.DropTable(
                name: "Assignments");
        }
    }
}
