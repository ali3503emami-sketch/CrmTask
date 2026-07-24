using CrmTask.Domain.Contacts;
using CrmTask.Domain.Contracts;
using CrmTask.Domain.Customers;
using CrmTask.Domain.ReferenceData;
using CrmTask.Domain.Settings;
using CrmTask.Domain.Staff;
using CrmTask.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrmTask.Infrastructure;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<Contract> Contracts => Set<Contract>();

    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<ReferenceListItem> ReferenceListItems => Set<ReferenceListItem>();

    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
    }
}
