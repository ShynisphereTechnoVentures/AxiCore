using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixStudentAcademicRelations : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Users_StudentId",
                table: "StudentLessonProgresses");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "StudentLessonProgresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLessonProgresses_UserId",
                table: "StudentLessonProgresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Students_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Users_UserId",
                table: "StudentLessonProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Students_StudentId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentLessonProgresses_Users_UserId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentLessonProgresses_UserId",
                table: "StudentLessonProgresses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentLessonProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentLessonProgresses_Users_StudentId",
                table: "StudentLessonProgresses",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
