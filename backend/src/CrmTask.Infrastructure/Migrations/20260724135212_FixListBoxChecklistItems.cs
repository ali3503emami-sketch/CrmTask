using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixListBoxChecklistItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ChecklistFieldType.ListBox was removed from the enum (replaced by
            // MultilineText, per product decision — see docs/solution-structure.md).
            // Existing rows still holding the old "ListBox" string fail to deserialize
            // ("Cannot convert string value 'ListBox' ... to any value in the mapped
            // enum") and break every read of a task that has one — including the
            // list itself, so a task looked like it "didn't save" even though the
            // INSERT succeeded; the very next GET just couldn't read anything back.
            // ListBox and Dropdown were both choice-fields with an identical Options
            // shape, so remapping to Dropdown loses nothing.
            migrationBuilder.Sql("UPDATE [TaskChecklistItems] SET [FieldType] = 'Dropdown' WHERE [FieldType] = 'ListBox';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deliberately not reversible — ListBox no longer exists as a valid
            // value in the current model, so there's nothing sensible to revert to.
        }
    }
}
