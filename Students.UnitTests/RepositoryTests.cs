using Students.Models;
using Students.Repositories;
using Xunit;

public class RepositoryTests
{
    [Fact]
    public void Insert_Get_Update_Delete_Works()
    {
        var repo = new InMemoryStudentRepository();

        var created = repo.Insert(new Student {
            FirstName="Test", LastName="User", Address="123 St",
            DateOfBirth=new DateTime(2010,1,1), Grade="5"
        });
        Assert.True(created.Id > 0);

        var fetched = repo.GetById(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Test", fetched!.FirstName);

        created.Address = "456 Ave";
        Assert.True(repo.Update(created.Id, created));

        var updated = repo.GetById(created.Id);
        Assert.Equal("456 Ave", updated!.Address);

        Assert.True(repo.Delete(created.Id));
        Assert.Null(repo.GetById(created.Id));
    }
}
