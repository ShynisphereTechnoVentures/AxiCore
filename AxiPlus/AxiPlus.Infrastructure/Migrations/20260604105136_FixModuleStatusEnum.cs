using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AxiPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixModuleStatusEnum : Migration
   {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.Sql("""
ALTER TABLE "StudentModules"
ALTER COLUMN "Status"
TYPE integer
USING CASE
    WHEN "Status" = 'Locked' THEN 1
    WHEN "Status" = 'Active' THEN 2
    WHEN "Status" = 'UnderReview' THEN 3
    WHEN "Status" = 'Completed' THEN 4
    ELSE 1
END;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
       {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudentModules",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
