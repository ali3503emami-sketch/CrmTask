using System.Text.Json.Serialization;
using CrmTask.Application.Contacts;
using CrmTask.Application.Contracts;
using CrmTask.Application.Customers;
using CrmTask.Application.ReferenceData;
using CrmTask.Application.Settings;
using CrmTask.Application.Staff;
using CrmTask.Application.Tasks;
using CrmTask.Infrastructure;
using CrmTask.Infrastructure.Contacts;
using CrmTask.Infrastructure.Contracts;
using CrmTask.Infrastructure.Customers;
using CrmTask.Infrastructure.ReferenceData;
using CrmTask.Infrastructure.Settings;
using CrmTask.Infrastructure.Staff;
using CrmTask.Infrastructure.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IReferenceListRepository, ReferenceListRepository>();
builder.Services.AddScoped<IReferenceListService, ReferenceListService>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<CrmDbContext>().Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Entry point class, exposed so <c>WebApplicationFactory&lt;Program&gt;</c> can boot this API in tests.
/// </summary>
public partial class Program
{
}
