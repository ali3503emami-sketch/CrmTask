using CrmTask.Domain.Contracts;
using CrmTask.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmTask.Infrastructure.Contracts;

public class ContractEntityConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.StartDateShamsi).HasMaxLength(10);
        builder.Property(c => c.EndDateShamsi).HasMaxLength(10);

        builder.HasIndex(c => c.CustomerId);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
