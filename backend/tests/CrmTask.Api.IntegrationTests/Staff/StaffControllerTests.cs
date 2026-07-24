using System.Net;
using System.Net.Http.Json;
using CrmTask.Application.Staff;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.Staff;

public class StaffControllerTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidRequest_Returns201()
    {
        var request = new CreateStaffMemberRequest("سارا محمدی", "09121112233", null);

        var response = await _client.PostAsJsonAsync("/api/staff", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<StaffMemberDto>();
        created!.FullName.Should().Be("سارا محمدی");
    }

    [Fact]
    public async Task GetActive_ReturnsPreviouslyCreatedStaffMember()
    {
        await _client.PostAsJsonAsync("/api/staff", new CreateStaffMemberRequest("علی رضایی", "09123334455", null));

        var response = await _client.GetAsync("/api/staff");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var staff = await response.Content.ReadFromJsonAsync<List<StaffMemberDto>>();
        staff.Should().Contain(s => s.FullName == "علی رضایی");
    }

    [Fact]
    public async Task Update_WithValidRequest_Returns200AndChangesIt()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/staff", new CreateStaffMemberRequest("علی رضایی", "09123334455", null));
        var created = await createResponse.Content.ReadFromJsonAsync<StaffMemberDto>();

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/staff/{created!.Id}",
            new CreateStaffMemberRequest("علی احمدی", "09121110000", "مدیر دفتر"));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<StaffMemberDto>();
        updated!.FullName.Should().Be("علی احمدی");
        updated.Position.Should().Be("مدیر دفتر");
    }

    [Fact]
    public async Task Update_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/staff/{Guid.NewGuid()}",
            new CreateStaffMemberRequest("علی احمدی", "09121110000", null));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithInvalidPhoneNumber_ReturnsValidationProblem()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/staff", new CreateStaffMemberRequest("علی رضایی", "09123334455", null));
        var created = await createResponse.Content.ReadFromJsonAsync<StaffMemberDto>();

        var response = await _client.PutAsJsonAsync(
            $"/api/staff/{created!.Id}",
            new CreateStaffMemberRequest("علی احمدی", "x", null));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
