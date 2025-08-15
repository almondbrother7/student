using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Students.Repositories;
// using Students.Models; // only needed if you seed sample Students
using System.Collections.Generic;
using System.Linq;

public class StudentsWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Put the app in the Testing environment (tweaks Program.cs behavior)
        builder.UseEnvironment("Testing");

        // Force the memory repo for predictable tests
        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["STUDENTS_REPO"] = "memory"
            });
        });

        // ⬇️ This is the spot you asked about
        builder.ConfigureServices(services =>
        {
            // Replace whatever IStudentRepository Program.cs registered
            var existing = services.SingleOrDefault(d => d.ServiceType == typeof(IStudentRepository));
            if (existing != null) services.Remove(existing);

            services.AddSingleton<IStudentRepository>(sp =>
            {
                var repo = new InMemoryStudentRepository();

                return repo;
            });
        });
    }
}
