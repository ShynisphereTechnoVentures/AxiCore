using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhaseOneOperations : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedMentorUserId",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Assignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceRole",
                table: "Assignments",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AttendanceDiscrepancies",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStatus = table.Column<int>(type: "integer", nullable: true),
                    RequestedStatus = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolutionNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_AttendanceDiscrepancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceDiscrepancies_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceDiscrepancies_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceDiscrepancies_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingRequests",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MentorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MeetingLink = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StudentResponseNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MentorFollowUpNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_MeetingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRequests_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingRequests_Users_MentorUserId",
                        column: x => x.MentorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentorProfiles",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EmergencyContact = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Designation = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_MentorProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalarySlips",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_SalarySlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySlips_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalarySlips_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignedMentorUserId",
                table: "Assignments",
                column: "AssignedMentorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CreatedByUserId",
                table: "Assignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDiscrepancies_ReviewedByUserId",
                table: "AttendanceDiscrepancies",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDiscrepancies_SessionId",
                table: "AttendanceDiscrepancies",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDiscrepancies_StudentId",
                table: "AttendanceDiscrepancies",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRequests_BatchId",
                table: "MeetingRequests",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRequests_MentorUserId",
                table: "MeetingRequests",
                column: "MentorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRequests_StudentId",
                table: "MeetingRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorProfiles_UserId",
                table: "MentorProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlips_UploadedByUserId",
                table: "SalarySlips",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlips_UserId",
                table: "SalarySlips",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_AssignedMentorUserId",
                table: "Assignments",
                column: "AssignedMentorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_CreatedByUserId",
                table: "Assignments",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_AssignedMentorUserId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_CreatedByUserId",
                table: "Assignments");

            migrationBuilder.DropTable(
                name: "AttendanceDiscrepancies");

            migrationBuilder.DropTable(
                name: "MeetingRequests");

            migrationBuilder.DropTable(
                name: "MentorProfiles");

            migrationBuilder.DropTable(
                name: "SalarySlips");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_AssignedMentorUserId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CreatedByUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "AssignedMentorUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "SourceRole",
                table: "Assignments");
        }
    }
}
