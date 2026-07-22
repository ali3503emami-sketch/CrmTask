using CrmTask.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmTask.Infrastructure.Customers;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Category)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.CreatedAtShamsi).HasMaxLength(10);
        builder.Property(c => c.ManagerName).HasMaxLength(200);
        builder.Property(c => c.ManagerBirthDateShamsi).HasMaxLength(10);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.Fax).HasMaxLength(20);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.NationalId).HasMaxLength(20);

        builder.OwnsMany(c => c.Personnel, personnel =>
        {
            personnel.ToTable("CustomerPersonnel");
            personnel.WithOwner().HasForeignKey("CustomerId");
            personnel.HasKey(p => p.Id);

            personnel.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            personnel.Property(p => p.Position).HasMaxLength(200);
            personnel.Property(p => p.Phone).HasMaxLength(20);
            personnel.Property(p => p.Mobile).HasMaxLength(20);
            personnel.Property(p => p.Email).HasMaxLength(200);
        });
    }
}
