using System.Text.Json;
using CrmTask.Domain.Customers;
using CrmTask.Domain.Staff;
using CrmTask.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmTask.Infrastructure.Tasks;

public class TaskItemEntityConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.DueAtShamsi).HasMaxLength(10);

        builder.HasIndex(t => t.CustomerId);
        builder.HasIndex(t => t.AssignedToStaffId);
        builder.HasIndex(t => t.CreatedByStaffId);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<StaffMember>()
            .WithMany()
            .HasForeignKey(t => t.AssignedToStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StaffMember>()
            .WithMany()
            .HasForeignKey(t => t.CreatedByStaffId)
            .OnDelete(DeleteBehavior.Restrict);

        // ChecklistItems is a read-only property (IReadOnlyList<ChecklistItem>); EF Core
        // finds the "_checklistItems" backing field by naming convention automatically.
        builder.OwnsMany(t => t.ChecklistItems, checklist =>
        {
            checklist.ToTable("TaskChecklistItems");
            checklist.WithOwner().HasForeignKey("TaskItemId");
            checklist.HasKey(c => c.Id);

            checklist.Property(c => c.Label)
                .IsRequired()
                .HasMaxLength(300);

            checklist.Property(c => c.FieldType)
                .HasConversion<string>()
                .HasMaxLength(20);

            checklist.Property(c => c.Value)
                .HasMaxLength(2000);

            // Options is also read-only (IReadOnlyList<string>) backed by "_options";
            // stored as a single JSON column rather than a further child table. Options
            // never change after creation (see ChecklistItem.Create), so a simple
            // reference/sequence-equality comparer is all change tracking needs.
            checklist.Property<List<string>>("_options")
                .HasColumnName("OptionsJson")
                .HasConversion(
                    options => JsonSerializer.Serialize(options, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null) ?? new List<string>(),
                    new ValueComparer<List<string>>(
                        (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
                        v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                        v => v.ToList()));
        });
    }
}
