using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskReferralsAndAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskUpcomingWindowDays = table.Column<int>(type: "int", nullable: false),
                    ContractEndingWindowDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskReferrals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferredByStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferredToStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ReferredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReferredAtShamsi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TaskItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskReferrals_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskReferrals_TaskItemId",
                table: "TaskReferrals",
                column: "TaskItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "TaskReferrals");
        }
    }
}
