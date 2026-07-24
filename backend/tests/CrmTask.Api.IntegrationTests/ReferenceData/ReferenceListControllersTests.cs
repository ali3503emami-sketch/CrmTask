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

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task Update_WithValidTitle_Returns200AndChangesIt(string route)
    {
        // Distinct titles per test, not the shared "مسئول دفتر" other tests use — the
        // (Kind, Title) unique index is enforced against the same persistent dev
        // database every test run reuses, so a repeated literal collides once any
        // earlier run has left that title behind.
        var originalTitle = $"مسئول دفتر {Guid.NewGuid()}";
        var updatedTitle = $"مدیر دفتر {Guid.NewGuid()}";
        using var createContent = new StringContent($$"""{"title":"{{originalTitle}}"}""", System.Text.Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(route, createContent);
        var created = await createResponse.Content.ReadFromJsonAsync<ReferenceListItemResponse>();

        using var updateContent = new StringContent($$"""{"title":"{{updatedTitle}}"}""", System.Text.Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync($"{route}/{created!.Id}", updateContent);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var listResponse = await _client.GetAsync(route);
        var body = await listResponse.Content.ReadAsStringAsync();
        body.Should().Contain(updatedTitle);
        body.Should().NotContain(originalTitle);
    }

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task Update_WithUnknownId_ReturnsNotFound(string route)
    {
        using var content = new StringContent("""{"title":"مدیر دفتر"}""", System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"{route}/{Guid.NewGuid()}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(Routes))]
    public async Task Update_WithEmptyTitle_ReturnsValidationProblem(string route)
    {
        var originalTitle = $"مسئول دفتر {Guid.NewGuid()}";
        using var createContent = new StringContent($$"""{"title":"{{originalTitle}}"}""", System.Text.Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(route, createContent);
        var created = await createResponse.Content.ReadFromJsonAsync<ReferenceListItemResponse>();

        using var updateContent = new StringContent("""{"title":""}""", System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"{route}/{created!.Id}", updateContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record ReferenceListItemResponse(Guid Id, string Title);
}
