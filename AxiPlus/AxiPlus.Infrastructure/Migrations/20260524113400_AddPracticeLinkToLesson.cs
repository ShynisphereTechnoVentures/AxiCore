using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeLinkToLesson : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.AddColumn<string>(
                name: "PracticeLink",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LessonLiveClasses",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingLink = table.Column<string>(type: "text", nullable: false),
                    RecordingLink = table.Column<string>(type: "text", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_LessonLiveClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonLiveClasses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonLiveClasses_LessonId",
                table: "LessonLiveClasses",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "LessonLiveClasses");

            migrationBuilder.DropColumn(
                name: "PracticeLink",
                table: "Lessons");
        }
    }
}
