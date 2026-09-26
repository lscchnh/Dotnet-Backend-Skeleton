using DotnetBackendSkeleton.Api.Data;
using DotnetBackendSkeleton.Api.Diagnostics;
using DotnetBackendSkeleton.Api.Extensions;
using DotnetBackendSkeleton.Api.Infrastructure;
using DotnetBackendSkeleton.Api.Options;
using DotnetBackendSkeleton.Api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults: OpenTelemetry (logs, traces, metrics), health checks, service discovery, resilience.
builder.AddServiceDefaults();

// Options, validated at startup.
builder.Services.AddValidatedOptions<ApplicationOptions>(ApplicationOptions.SectionName);
builder.Services.AddValidatedOptions<DatabaseOptions>(DatabaseOptions.SectionName);

// Persistence: the "appdb" connection string is injected by the Aspire AppHost locally,
// and by the environment (ConnectionStrings__appdb) once deployed. Adds health check, retries and telemetry.
builder.AddNpgsqlDbContext<AppDbContext>(AppDbContext.ConnectionStringName);
builder.Services.AddHostedService<DatabaseMigrator>();

// Application services.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<TodoMetrics>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Web: controllers, RFC 9457 problem details for every error, OpenAPI document.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<ApplicationInfoTransformer>());

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // OpenAPI document at /openapi/v1.json and interactive reference at /scalar
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync();
