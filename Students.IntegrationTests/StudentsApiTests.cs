using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Students.Enums;
using Students.Models;
using Xunit;

public class StudentsApiTests : IClassFixture<StudentsWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _json; // ensure string-enum on requests & reads

    public StudentsApiTests(StudentsWebAppFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _json.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    [Fact]
    public async Task Crud_Flow_Works()
    {
        // GET all (seeded)
        var list = await _client.GetFromJsonAsync<StudentResponseDto[]>("/api/students", _json);
        Assert.NotNull(list);

        // If your test fixture doesn't seed, loosen this to >= 0 or seed the file via the factory
        Assert.True(list!.Length >= 3);

        // POST (use string enum via options, not numeric)
        var dto = new
        {
            firstName = "Linus",
            lastName = "Youngadev",
            address = "1 Kernel Way",
            dateOfBirth = new DateTime(2008, 12, 28),
            email = "linus@example.com",
            phone = "321-555-0101",
            grade = "11",
            enrollmentStatus = EnrollmentStatus.Active // will serialize as "active" due to _json
        };

        var postResp = await _client.PostAsJsonAsync("/api/students", dto, _json);
        if (postResp.StatusCode != HttpStatusCode.Created)
        {
            var body = await postResp.Content.ReadAsStringAsync();
            Assert.True(false, $"Expected 201 Created, got {(int)postResp.StatusCode}: {body}");
        }

        var created = await postResp.Content.ReadFromJsonAsync<StudentResponseDto>(_json);
        Assert.NotNull(created);
        var id = created!.Id;

        // GET by id
        var byId = await _client.GetFromJsonAsync<StudentResponseDto>($"/api/students/{id}", _json);
        Assert.NotNull(byId);
        Assert.Equal("Linus", byId!.FirstName);
        Assert.Equal(EnrollmentStatus.Active, byId.EnrollmentStatus);

        // PUT (create a new object; don't use 'with' on anonymous types)
        var dto2 = new
        {
            firstName = "Linus",
            lastName = "Youngadev",
            address = "123 Updated Kernel Way",
            dateOfBirth = new DateTime(2008, 12, 28),
            email = "linus@example.com",
            phone = "321-555-0101",
            grade = "12",
            enrollmentStatus = EnrollmentStatus.Active
        };

        var putResp = await _client.PutAsJsonAsync($"/api/students/{id}", dto2, _json);
        if (putResp.StatusCode != HttpStatusCode.NoContent)
        {
            var body = await putResp.Content.ReadAsStringAsync();
            Assert.True(false, $"Expected 204 NoContent, got {(int)putResp.StatusCode}: {body}");
        }

        // Verify update
        var after = await _client.GetFromJsonAsync<StudentResponseDto>($"/api/students/{id}", _json);
        Assert.NotNull(after);
        Assert.Equal("123 Updated Kernel Way", after!.Address);
        Assert.Equal("12", after.Grade);

        // DELETE
        var del = await _client.DeleteAsync($"/api/students/{id}");
        if (del.StatusCode != HttpStatusCode.NoContent)
        {
            var body = await del.Content.ReadAsStringAsync();
            Assert.True(false, $"Expected 204 NoContent, got {(int)del.StatusCode}: {body}");
        }

        // Verify 404
        var miss = await _client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NotFound, miss.StatusCode);
    }
}
