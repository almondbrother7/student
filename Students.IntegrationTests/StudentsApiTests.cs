using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Students.Models;
using Xunit;

public class StudentsApiTests : IClassFixture<StudentsWebAppFactory>
{
    private readonly HttpClient _client;

    public StudentsApiTests(StudentsWebAppFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Crud_Flow_Works()
    {
        // GET all (seeded)
        var list = await _client.GetFromJsonAsync<Student[]>("/api/students");
        Assert.NotNull(list);
        Assert.True(list!.Length >= 3);

        // POST
        var dto = new {
            firstName="Linus", lastName="Youngadev", address="1 Kernel Way",
            dateOfBirth=new DateTime(2008,12,28),
            email="linus@example.com", phone="321-555-0101", grade="11"
        };
        var postResp = await _client.PostAsJsonAsync("/api/students", dto);
        Assert.Equal(HttpStatusCode.Created, postResp.StatusCode);
        var created = await postResp.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(created);
        var id = created!.Id;

        // GET by id
        var byId = await _client.GetFromJsonAsync<Student>($"/api/students/{id}");
        Assert.NotNull(byId);
        Assert.Equal("Linus", byId!.FirstName);

        // PUT
        var dto2 = dto with { address = "123 Updated Kernel Way", grade = "12" };
        var putResp = await _client.PutAsJsonAsync($"/api/students/{id}", dto2);
        Assert.Equal(HttpStatusCode.NoContent, putResp.StatusCode);

        // Verify update
        var after = await _client.GetFromJsonAsync<Student>($"/api/students/{id}");
        Assert.Equal("123 Updated Kernel Way", after!.Address);
        Assert.Equal("12", after!.Grade);

        // DELETE
        var del = await _client.DeleteAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        // Verify 404
        var miss = await _client.GetAsync($"/api/students/{id}");
        Assert.Equal(HttpStatusCode.NotFound, miss.StatusCode);
    }
}
