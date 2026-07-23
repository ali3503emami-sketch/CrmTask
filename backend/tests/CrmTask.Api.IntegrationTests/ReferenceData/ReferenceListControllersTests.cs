using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.ReferenceData;

/// <summary>
/// Positions/Customer Categories/Activity Fields are three identical-shape
/// REST resources sharing one implementation (<see cref="Api.Controllers.ReferenceListControllerBase"/>);
/// one parameterized test class covers all three rather than tripling the file.
/// </summary>
public class ReferenceListControllersTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    public static IEnumerable<object[]> Routes =>
    [
        ["/api/positions"],
        ["/api/customer-categories"],
        ["/api/activity-fields"],
    ];

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task Create_WithValidTitle_Returns201AndListsIt(string route)
    {
        var json = """{"title":"مسئول دفتر"}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync(route, content);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResponse = await _client.GetAsync(route);
        var body = await listResponse.Content.ReadAsStringAsync();
        body.Should().Contain("مسئول دفتر");
    }

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task Create_WithEmptyTitle_ReturnsValidationProblem(string route)
    {
        using var content = new StringContent("""{"title":""}""", System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(route, content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
