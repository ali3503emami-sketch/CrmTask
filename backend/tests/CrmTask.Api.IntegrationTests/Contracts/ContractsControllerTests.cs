using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.Contracts;

public class ContractsControllerTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidRequest_Returns201WithComputedStatus()
    {
        var customerId = await CreateCustomerAsync();
        var json = """{"title":"قرارداد پشتیبانی سالانه","amount":50000000,"startDate":"2026-01-01","endDate":"2026-12-31"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{customerId}/contracts", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"title\":\"قرارداد پشتیبانی سالانه\"");
        body.Should().Contain("\"status\":");
    }

    [Fact]
    public async Task Create_ReturnsShamsiMirrorsForStartAndEndDate()
    {
        // 2024-03-20 is the well-documented Nowruz (Persian new year) 1403 —
        // a fixed calendar fact, not something derived by the code under test.
        var customerId = await CreateCustomerAsync();
        var json = """{"title":"قرارداد پشتیبانی","amount":1000,"startDate":"2024-03-20","endDate":"2024-12-31"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{customerId}/contracts", content);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"startDateShamsi\":\"1403/01/01\"");
        body.Should().Contain("\"endDateShamsi\":");
    }

    [Fact]
    public async Task Create_WithEndDateBeforeStartDate_ReturnsValidationProblem()
    {
        var customerId = await CreateCustomerAsync();
        var json = """{"title":"قرارداد","amount":0,"startDate":"2026-12-31","endDate":"2026-01-01"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{customerId}/contracts", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ForNonExistentCustomer_Returns404()
    {
        var json = """{"title":"قرارداد","amount":0,"startDate":"2026-01-01","endDate":"2026-12-31"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{Guid.NewGuid()}/contracts", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomer_ReturnsPreviouslyCreatedContract()
    {
        var customerId = await CreateCustomerAsync();
        var json = """{"title":"قرارداد نگهداری","amount":1000,"startDate":"2026-01-01","endDate":"2026-12-31"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await _client.PostAsync($"/api/customers/{customerId}/contracts", content);

        var response = await _client.GetAsync($"/api/customers/{customerId}/contracts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("قرارداد نگهداری");
    }

    [Fact]
    public async Task GetAll_ReturnsContractAcrossCustomersWithCustomerId()
    {
        var customerId = await CreateCustomerAsync();
        var json = """{"title":"قرارداد قابل جستجوی سراسری","amount":1000,"startDate":"2026-01-01","endDate":"2026-12-31"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await _client.PostAsync($"/api/customers/{customerId}/contracts", content);

        var response = await _client.GetAsync("/api/contracts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("قرارداد قابل جستجوی سراسری");
        body.Should().Contain($"\"customerId\":\"{customerId}\"");
    }

    private async Task<Guid> CreateCustomerAsync()
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");
        var response = await _client.PostAsJsonAsync("/api/customers", request, JsonOptions);
        var created = await response.Content.ReadFromJsonAsync<CustomerDto>(JsonOptions);
        return created!.Id;
    }
}
