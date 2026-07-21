using CrmTask.Domain.Contacts;
using CrmTask.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmTask.Infrastructure.Contacts;

public class ContactEntityConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Summary)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Direction)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(c => c.CustomerId);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
