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
        var request = new CreateStaffMemberRequest("سارا محمدی", "09121112233");

        var response = await _client.PostAsJsonAsync("/api/staff", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<StaffMemberDto>();
        created!.FullName.Should().Be("سارا محمدی");
    }

    [Fact]
    public async Task GetActive_ReturnsPreviouslyCreatedStaffMember()
    {
        await _client.PostAsJsonAsync("/api/staff", new CreateStaffMemberRequest("علی رضایی", "09123334455"));

        var response = await _client.GetAsync("/api/staff");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var staff = await response.Content.ReadFromJsonAsync<List<StaffMemberDto>>();
        staff.Should().Contain(s => s.FullName == "علی رضایی");
    }
}
