using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentBilling : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.CreateTable(
                name: "StudentBillingAccounts",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonthlyFee = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GraceEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AutoPayEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UpiId = table.Column<string>(type: "text", nullable: false),
                    LastReminderAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentBillingAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentBillingAccounts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPayments",
                columns: table => new
               {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    UpiId = table.Column<string>(type: "text", nullable: false),
                    ProviderReference = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GraceEndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
               {
                    table.PrimaryKey("PK_StudentPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentPayments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentBillingAccounts_StudentId",
                table: "StudentBillingAccounts",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentPayments_StudentId",
                table: "StudentPayments",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.DropTable(
                name: "StudentBillingAccounts");

            migrationBuilder.DropTable(
                name: "StudentPayments");
        }
    }
}
