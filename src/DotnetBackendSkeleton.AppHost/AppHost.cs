var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL server + database, run as containers locally (Docker or Podman required).
// Add .WithDataVolume() to keep data between runs, and .WithPgAdmin() for an admin UI.
var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("appdb");

builder.AddProject<Projects.Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "API reference (Scalar)";
        url.Url = "/scalar";
    });

builder.Build().Run();
