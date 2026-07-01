using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentPortalSupport : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.CreateTable(
                name: "StudentNotifications",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentNotifications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MentorResponse = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentNotifications_StudentId",
                table: "StudentNotifications",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_StudentId",
                table: "SupportTickets",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "StudentNotifications");

            migrationBuilder.DropTable(
                name: "SupportTickets");
        }
    }
}
