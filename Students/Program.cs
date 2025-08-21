using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;  
using Microsoft.AspNetCore.Mvc;         // for ProblemDetails/ProblemDetailsContext
using Microsoft.AspNetCore.Mvc.Infrastructure; 

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

using Students.Exceptions;
using Students.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging for dev mode - format all with timestamp, including Kestrel, Hosting, ASP.Net.
builder.Logging.ClearProviders(); // remove defaults
builder.Logging.AddDebug();
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss ";
});

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opt.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Students API", Version = "v1" });
});

builder.Services.AddSingleton<IStudentRepository>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var choice = (cfg["STUDENTS_REPO"] ?? "memory").ToLowerInvariant();
    
    Console.WriteLine($"[Students] Repo={choice}  Path={cfg["STUDENTS_PATH"] ?? "(n/a)"}");

    return choice == "json"
        ? new JsonFileStudentRepository(env, cfg)
        : new InMemoryStudentRepository();
});

builder.Services.AddSingleton<Students.Services.IStudentService, Students.Services.StudentService>();

builder.Services.AddCors(o => o.AddPolicy("Dev", p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
));

// Consistent RFC 7807 error payloads
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;

        var ex = ctx.Exception;

        if (ex is DuplicateEmailException dup)
        {
            ctx.ProblemDetails.Status = StatusCodes.Status409Conflict;
            ctx.ProblemDetails.Title = "Email already in use";
            ctx.ProblemDetails.Detail = dup.Message;

            if (!string.IsNullOrWhiteSpace(dup.Email))
                ctx.ProblemDetails.Extensions["email"] = dup.Email;

            var existingIdProp = dup.GetType().GetProperty("ExistingId");
            var existingIdVal = existingIdProp?.GetValue(dup);
            if (existingIdVal is not null)
                ctx.ProblemDetails.Extensions["existingId"] = existingIdVal;

            return;
        }
        else if (ex is UnauthorizedAccessException)
        {
            ctx.ProblemDetails.Status = StatusCodes.Status403Forbidden;
            ctx.ProblemDetails.Title = "Access denied.";
            return;
        }
        else if (ex is FileNotFoundException)
        {
            ctx.ProblemDetails.Status = StatusCodes.Status404NotFound;
            ctx.ProblemDetails.Title = "Resource file not found.";
            return;
        }

        ctx.ProblemDetails.Status ??= StatusCodes.Status500InternalServerError;
        ctx.ProblemDetails.Title ??= "An unexpected error occurred.";
    };
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature   = context.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error;

        var statusCode =
            exception is DuplicateEmailException ? StatusCodes.Status409Conflict :
            exception is UnauthorizedAccessException ? StatusCodes.Status403Forbidden :
            exception is FileNotFoundException ? StatusCodes.Status404NotFound :
            StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;

        // Let the ProblemDetails service format the response using your CustomizeProblemDetails
        var pds = context.RequestServices.GetRequiredService<IProblemDetailsService>();
        await pds.WriteAsync(new ProblemDetailsContext
        {
            HttpContext    = context,
            Exception      = exception,
            ProblemDetails = new ProblemDetails { Status = statusCode }
        });
    });
});


// Swagger: pin server dynamically to the current request (http or https)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((doc, req) =>
            doc.Servers = [new() { Url = $"{req.Scheme}://{req.Host.Value}" }]);
    });
    app.UseSwaggerUI(c =>
    {   // Defaults root to swagger page.
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Students API v1");
        c.RoutePrefix = string.Empty; // <-- Swagger UI at "/"
    });
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseCors("Dev");

app.MapControllers();


// Make home show the links for swagger and to get students json
app.MapGet("/", () => Results.Text("Students API is running. See /swagger and /api/students"));

// For testing the duplicate email exception handler:
app.MapGet("/debug/ExceptionEmailAlreadyInUse", () =>
{
    throw new Students.Exceptions.DuplicateEmailException(
        "test@example.com",
        42
    );
});

app.MapGet("/debug/pid", () => Environment.ProcessId);
app.MapGet("/debug/repo", (IConfiguration cfg) =>
    Results.Json(new {
        repo = cfg["STUDENTS_REPO"] ?? "memory",
        path = cfg["STUDENTS_PATH"] ?? "(n/a)"
    }));

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    environment = app.Environment.EnvironmentName,
    time = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithOpenApi();

// Print PID and listening URLs once, for attaching debugger to process.
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine($"[Students] PID={Environment.ProcessId}  Env={app.Environment.EnvironmentName}");
    Console.WriteLine($"[Students] URLs={string.Join(", ", app.Urls)}");
});

app.Run();


public partial class Program { }
