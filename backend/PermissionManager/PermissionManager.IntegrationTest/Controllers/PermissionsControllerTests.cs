using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PermissionManager.API.Utils;
using PermissionManager.Application.DTOs;
using PermissionManager.Core.Pagination;

namespace PermissionManager.IntegrationTest.Controllers;

public class PermissionsControllerTests(CustomWebAppFactory customWebAppFactory) : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _httpClient = customWebAppFactory.CreateClient();

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
    
    [Fact]
    public async Task Full_Crud_Workflow_ShouldReturnExpectedResults()
    {
        // 1. Request Permission -> POST /api/permissions
        var createDto = new
        {
            employee_name = "Jon",
            employee_surname = "Doe",
            permission_type_id = 1
        };
        var postResp = await _httpClient.PostAsJsonAsync("/api/permissions", createDto);
        postResp.StatusCode.Should().Be(HttpStatusCode.Created);
        postResp.Headers.Location.Should().NotBeNull();

        // Read Location header
        var createdAtLocationPath = postResp.Headers.Location!.PathAndQuery;
        
        // 2. Get Recently Created Permission -> GET /api/permissions
        var getCreatedResp = await _httpClient.GetAsync(createdAtLocationPath);
        getCreatedResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var permission = await getCreatedResp.Content.ReadFromJsonAsync<PermissionDto>(_jsonSerializerOptions);
        permission.Should().BeEquivalentTo(new
        {
            EmployeeName = "Jon",
            EmployeeSurname = "Doe",
            PermissionType = new
            {
                Id = 1
            }
        }, options => options.ExcludingMissingMembers());
        
        // 3. Modify -> PUT /api/permissions/{id}
        var modDto = new
        {
            employee_name = "Jane",
            employee_surname = "Doe Doe",
            permission_type_id = 2
        };
        var putResp = await _httpClient.PutAsJsonAsync($"/api/permissions/{permission.Id}", modDto);
        putResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // 4. List -> GET /api/permissions
        var getAllResp = await _httpClient.GetAsync("/api/permissions");
        getAllResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await getAllResp.Content.ReadFromJsonAsync<PagedResponse<PermissionDto>>(_jsonSerializerOptions);
        list.Should().NotBeNull();
        list.HasNextPage.Should().Be(false);
        list.Data.Should()
            .ContainSingle(p => p.EmployeeName == "Jane" && p.EmployeeSurname == "Doe Doe" && p.PermissionType.Id == 2);
    }

}