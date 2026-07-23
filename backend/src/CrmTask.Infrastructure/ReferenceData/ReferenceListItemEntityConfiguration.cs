using CrmTask.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmTask.Infrastructure.ReferenceData;

public class ReferenceListItemEntityConfiguration : IEntityTypeConfiguration<ReferenceListItem>
{
    public void Configure(EntityTypeBuilder<ReferenceListItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Kind)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);

        // One title per kind — e.g. can't add "مسئول دفتر" as a Position twice.
        builder.HasIndex(i => new { i.Kind, i.Title }).IsUnique();
    }
}
