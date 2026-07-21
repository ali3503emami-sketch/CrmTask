using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CrmTask.Application.Staff;
using FluentAssertions;
using Xunit;

namespace CrmTask.Api.IntegrationTests.Tasks;

public class TasksControllerTests(CustomApiFactory factory) : IClassFixture<CustomApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_WithValidRequestAndChecklist_Returns201()
    {
        var staffId = await CreateStaffMemberAsync();
        var json = $$"""
            {
              "title": "بررسی سرور",
              "description": "بررسی وضعیت سرور پشتیبان",
              "dueAt": "2026-08-01T12:00:00Z",
              "customerId": null,
              "assignedToStaffId": "{{staffId}}",
              "checklistFields": [
                { "label": "چک شد؟", "fieldType": "Checkbox", "options": null },
                { "label": "وضعیت", "fieldType": "Dropdown", "options": ["انجام‌شده", "در حال انجام"] }
              ]
            }
            """;
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/tasks", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"title\":\"بررسی سرور\"");
        body.Should().Contain("\"status\":\"Open\"");
        body.Should().Contain("چک شد؟");
    }

    [Fact]
    public async Task Create_ForNonExistentStaffMember_Returns404()
    {
        var json = """{"title":"کار","description":"","dueAt":"2026-08-01T12:00:00Z","customerId":null,"assignedToStaffId":"11111111-1111-1111-1111-111111111111","checklistFields":[]}""";
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/tasks", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkAsDone_TransitionsStatus()
    {
        var staffId = await CreateStaffMemberAsync();
        var taskId = await CreateTaskAsync(staffId);

        var response = await _client.PostAsync($"/api/tasks/{taskId}/mark-done", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"status\":\"Done\"");
    }

    [Fact]
    public async Task SetChecklistItemValue_UpdatesTheValue()
    {
        var staffId = await CreateStaffMemberAsync();
        var json = $$"""
            {
              "title": "کار با چک‌لیست",
              "description": "",
              "dueAt": "2026-08-01T12:00:00Z",
              "customerId": null,
              "assignedToStaffId": "{{staffId}}",
              "checklistFields": [ { "label": "چک شد؟", "fieldType": "Checkbox", "options": null } ]
            }
            """;
        using var createContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/tasks", createContent);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createdTask.GetProperty("id").GetString();
        var checklistItemId = createdTask.GetProperty("checklistItems")[0].GetProperty("id").GetString();

        using var valueContent = new StringContent("""{"value":"true"}""", System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/tasks/{taskId}/checklist-items/{checklistItemId}", valueContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"value\":\"true\"");
    }

    [Fact]
    public async Task GetAll_ReturnsPreviouslyCreatedTask()
    {
        var staffId = await CreateStaffMemberAsync();
        await CreateTaskAsync(staffId, "کار قابل جستجو");

        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("کار قابل جستجو");
    }

    private async Task<string> CreateStaffMemberAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/staff", new CreateStaffMemberRequest("سارا محمدی", "09121112233"));
        var created = await response.Content.ReadFromJsonAsync<StaffMemberDto>(JsonOptions);
        return created!.Id.ToString();
    }

    private async Task<string> CreateTaskAsync(string staffId, string title = "کار")
    {
        var json = $$"""
            {"title":"{{title}}","description":"","dueAt":"2026-08-01T12:00:00Z","customerId":null,"assignedToStaffId":"{{staffId}}","checklistFields":[]}
            """;
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/tasks", content);
        var created = await response.Content.ReadFromJsonAsync<JsonElement>();
        return created.GetProperty("id").GetString()!;
    }
}
