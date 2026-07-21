using CrmTask.Domain.Contacts;
using CrmTask.Domain.Contracts;
using CrmTask.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
    }
}
