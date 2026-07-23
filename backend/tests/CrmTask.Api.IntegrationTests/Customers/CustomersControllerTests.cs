using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CrmTask.Application.Customers;
using CrmTask.Domain.Customers;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.Customers;

public class CustomersControllerTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    // Matches the API's own JSON config (Program.cs) so tests read/write enums as
    // strings the same way a real client (e.g. the React frontend) would.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidRequest_Returns201WithLocationAndBody()
    {
        var request = new CreateCustomerRequest("شرکت فناوران البرز", CustomerCategory.Legal, "02112345678");

        var response = await _client.PostAsJsonAsync("/api/customers", request, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CustomerDto>(JsonOptions);
        created.Should().NotBeNull();
        created!.Name.Should().Be("شرکت فناوران البرز");
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithCategoryAsWireStringLikeARealClientSends_Succeeds()
    {
        // Regression guard: a real HTTP client (e.g. the React frontend) sends
        // enums as their JSON string name ("Legal"), not the numeric value that
        // .NET-to-.NET calls default to — this must round-trip as a string too.
        var json = """{"name":"شرکت فناوران البرز","category":"Legal","phone":"02112345678"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/customers", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"category\":\"Legal\"");
    }

    [Fact]
    public async Task Create_WithMissingName_ReturnsValidationProblem()
    {
        var request = new CreateCustomerRequest(string.Empty, CustomerCategory.Legal, "02112345678");

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_ReturnsPreviouslyCreatedCustomer()
    {
        var request = new CreateCustomerRequest("مهندس رضا کیانی", CustomerCategory.Individual, "09121234567");
        await _client.PostAsJsonAsync("/api/customers", request, JsonOptions);

        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>(JsonOptions);
        customers.Should().Contain(c => c.Name == "مهندس رضا کیانی");
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ChangesCoreAndProfileFieldsAndPersonnel()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("نام قدیمی", CustomerCategory.Legal, "02112345678"),
            JsonOptions);
        var created = await createResponse.Content.ReadFromJsonAsync<CustomerDto>(JsonOptions);

        var json = """
            {
              "name": "نام جدید",
              "category": "Legal",
              "phone": "02112345678",
              "managerName": "رضا کیانی",
              "managerBirthDate": "1980-05-10",
              "address": "تهران",
              "fax": "02112345679",
              "notes": "یادداشت",
              "nationalId": "1234567890",
              "categoryTitle": "صنعتی",
              "activityField": "تولید قطعات خودرو",
              "personnel": [
                { "fullName": "سارا محمدی", "position": "حسابدار", "phone": null, "mobile": "09121112233", "email": null }
              ]
            }
            """;
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/customers/{created!.Id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"name\":\"نام جدید\"");
        body.Should().Contain("\"managerName\":\"رضا کیانی\"");
        body.Should().Contain("\"categoryTitle\":\"صنعتی\"");
        body.Should().Contain("\"activityField\":\"تولید قطعات خودرو\"");
        body.Should().Contain("سارا محمدی");
    }

    [Fact]
    public async Task Update_WhenCustomerNotFound_Returns404()
    {
        var json = """{"name":"نام","category":"Legal","phone":"02112345678","managerName":null,"managerBirthDate":null,"address":null,"fax":null,"notes":null,"nationalId":null,"personnel":[]}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/customers/{Guid.NewGuid()}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
