using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuFeaturesSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByStaffId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Pre-existing rows have no real "who created this" data — backfill with
            // the task's own (already-valid) assignee so the FK added below doesn't
            // fail against the all-zero default, and existing data never violates it.
            migrationBuilder.Sql("UPDATE [Tasks] SET [CreatedByStaffId] = [AssignedToStaffId];");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "StaffMembers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityField",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryTitle",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReferenceListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceListItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByStaffId",
                table: "Tasks",
                column: "CreatedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceListItems_Kind_Title",
                table: "ReferenceListItems",
                columns: new[] { "Kind", "Title" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_StaffMembers_CreatedByStaffId",
                table: "Tasks",
                column: "CreatedByStaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_StaffMembers_CreatedByStaffId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "ReferenceListItems");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedByStaffId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedByStaffId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "ActivityField",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CategoryTitle",
                table: "Customers");
        }
    }
}
