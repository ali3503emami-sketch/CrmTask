using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.Contacts;

public class ContactsControllerTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidRequest_Returns201()
    {
        var customerId = await CreateCustomerAsync();
        var json = $$"""
            {"direction":"Outbound","summary":"پیگیری وضعیت قرارداد","contactedAt":"2026-07-20T10:00:00Z","nextFollowUpAt":"2026-07-25T10:00:00Z"}
            """;
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{customerId}/contacts", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"direction\":\"Outbound\"");
    }

    [Fact]
    public async Task Create_ReturnsShamsiMirrorsForContactedAtAndNextFollowUpAt()
    {
        // 2024-03-20 is the well-documented Nowruz (Persian new year) 1403 —
        // a fixed calendar fact, not something derived by the code under test.
        var customerId = await CreateCustomerAsync();
        var json = """{"direction":"Outbound","summary":"خلاصه","contactedAt":"2024-03-20T10:00:00Z","nextFollowUpAt":"2024-03-21T10:00:00Z"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{customerId}/contacts", content);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"contactedAtShamsi\":\"1403/01/01\"");
        body.Should().Contain("\"nextFollowUpAtShamsi\":\"1403/01/02\"");
    }

    [Fact]
    public async Task Create_ForNonExistentCustomer_Returns404()
    {
        var json = """{"direction":"Inbound","summary":"خلاصه","contactedAt":"2026-07-20T10:00:00Z"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"/api/customers/{Guid.NewGuid()}/contacts", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCustomer_ReturnsPreviouslyCreatedContact()
    {
        var customerId = await CreateCustomerAsync();
        var json = """{"direction":"Inbound","summary":"تماس ورودی مشتری","contactedAt":"2026-07-20T10:00:00Z"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await _client.PostAsync($"/api/customers/{customerId}/contacts", content);

        var response = await _client.GetAsync($"/api/customers/{customerId}/contacts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("تماس ورودی مشتری");
    }

    private async Task<Guid> CreateCustomerAsync()
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");
        var response = await _client.PostAsJsonAsync("/api/customers", request, JsonOptions);
        var created = await response.Content.ReadFromJsonAsync<CustomerDto>(JsonOptions);
        return created!.Id;
    }
}
