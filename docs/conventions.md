# Conventions

Conventions followed by the skeleton, and how to extend it without breaking them.

## Solution & build

- One solution file, `DotnetBackendSkeleton.slnx`. Sources in `src/`, tests in `tests/`.
- Common MSBuild properties live in [`Directory.Build.props`](../Directory.Build.props): target framework, nullable,
  analyzers (`AnalysisLevel=latest-recommended`), `TreatWarningsAsErrors`, code style enforced at build time.
  Do not repeat them in project files.
- Package versions live in [`Directory.Packages.props`](../Directory.Packages.props) (central package management):
  project files reference packages **without** a version.
- The SDK is pinned in [`global.json`](../global.json) (`rollForward: latestFeature`).
- Code style is defined in [`.editorconfig`](../.editorconfig). Run `dotnet format` before pushing: the CI fails otherwise.
- .NET tools (e.g. `dotnet-ef`) are declared in `dotnet-tools.json`: run `dotnet tool restore`.

## API

- Controllers inherit from `ControllerBase`, are decorated with `[ApiController]`, and use lower-case plural routes
  (`api/todos`).
- Actions are `async`, accept a `CancellationToken` and return `ActionResult<T>` / `IActionResult`.
- Controllers stay thin: they translate HTTP to service calls. Business logic lives in `Services/`.
- The API exposes **contracts** (`Contracts/`, records), never EF entities.
- Validation uses data annotations on contracts: `[ApiController]` returns a `400` validation problem automatically.
- Declare every response with `[ProducesResponseType<T>(status, contentType)]`: it drives the OpenAPI document.
  Success responses are `application/json`, errors `application/problem+json`.
- Document endpoints and contracts with XML comments (`<summary>`, `<param>`, `<remarks>`): they are included in the
  OpenAPI document.
- Enums are serialized as strings (`[JsonConverter(typeof(JsonStringEnumConverter<T>))]` on the enum).

## Errors

- Every error response follows [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) problem details
  (`AddProblemDetails()` + `UseStatusCodePages()`), with a `traceId` to correlate with logs and traces.
- Expected outcomes (not found, invalid input...) are returned explicitly by controllers (`NotFound()`, `ValidationProblem()`).
- Unexpected exceptions are handled by [`GlobalExceptionHandler`](../src/DotnetBackendSkeleton.Api/Infrastructure/GlobalExceptionHandler.cs):
  logged, turned into a `500` problem, without internal details outside of Development. Map domain exceptions to
  status codes in its `GetStatusCode` method.

## Configuration

- Settings are bound to option classes in `Options/` with `AddValidatedOptions<T>(section)`, which validates data
  annotations **at startup**: a misconfigured application does not start.
- `appsettings.json` holds production-safe defaults; `appsettings.Development.json` overrides them locally.
- Connection strings go in `ConnectionStrings:<name>`; the name matches the Aspire resource name (`appdb`).
- Never commit secrets: user secrets locally, secret store (Kubernetes secrets, Key Vault...) once deployed.

## Persistence

- One `AppDbContext`. Each entity has its own `IEntityTypeConfiguration<T>` in `Data/Configurations`.
- Registered with `builder.AddNpgsqlDbContext<AppDbContext>("appdb")`: connection pooling, retries, health check and
  telemetry come from the Aspire integration.
- Read queries use `AsNoTracking()` and project to contracts; bulk deletes/updates use `ExecuteDelete/ExecuteUpdate`.
- Identifiers are `Guid.CreateVersion7()` (time-ordered, index-friendly). Timestamps are UTC and come from the injected
  `TimeProvider` (testable).
- Migrations: `dotnet ef migrations add <Name> --project src/DotnetBackendSkeleton.Api --output-dir Data/Migrations`.
  They are applied at startup locally only (`Database:ApplyMigrationsOnStartup`), and by the delivery pipeline elsewhere.

## Observability

- `builder.AddServiceDefaults()` configures OpenTelemetry for logs, traces and metrics, exported with OTLP when
  `OTEL_EXPORTER_OTLP_ENDPOINT` is set (always the case under Aspire).
- Log with [source-generated `LoggerMessage`](https://learn.microsoft.com/dotnet/core/extensions/logger-message-generator)
  methods and structured placeholders (`{TodoItemId}`), never string interpolation.
- Production logs are written as JSON to the console (`Logging:Console:FormatterName=json`).
- Business metrics are created from `IMeterFactory` with a meter named after the application
  (see [`TodoMetrics`](../src/DotnetBackendSkeleton.Api/Diagnostics/TodoMetrics.cs)), and follow the
  [OpenTelemetry naming conventions](https://opentelemetry.io/docs/specs/semconv/general/metrics/).
- Custom traces use an `ActivitySource` named after the application.

## Health checks

- `/alive`: liveness, only checks tagged `live` (the process responds).
- `/health`: readiness, all checks (including the database check added by the Aspire integration).
- Add dependency checks with `builder.Services.AddHealthChecks().AddCheck<T>(...)`. Tag with `live` only checks whose
  failure requires restarting the process.
- These endpoints are for the orchestrator: do not expose them through the public ingress.

## Tests

| Project | Scope | Dependencies |
| --- | --- | --- |
| `UnitTests` | A service or controller in isolation. Mocks with NSubstitute, SQLite in-memory for EF, `FakeTimeProvider`, `MetricCollector`. | None |
| `IntegrationTests` | The whole HTTP pipeline in memory with `WebApplicationFactory`, PostgreSQL replaced by SQLite in-memory. | None |
- Framework: MSTest 4 on Microsoft.Testing.Platform (`dotnet test`).
- Test names describe the behavior: `Method_expected_result_when_condition`.
- Pass `TestContext.CancellationToken` to async calls.

## Adding a new feature

1. Entity in `Models/` + configuration in `Data/Configurations/` + `DbSet` in `AppDbContext`.
2. Migration: `dotnet ef migrations add Add<Feature>`.
3. Contracts in `Contracts/`, service interface + implementation in `Services/`, registered in `Program.cs`.
4. Controller in `Controllers/` with XML comments and `ProducesResponseType` attributes.
5. Unit tests for the service, integration tests for the endpoints.

## Adding a new resource (cache, queue, other service...)

Model it in the AppHost ([`AppHost.cs`](../src/DotnetBackendSkeleton.AppHost/AppHost.cs)) with the matching Aspire hosting
integration (e.g. `builder.AddRedis("cache")`), reference it from the API (`.WithReference(cache)`), and consume it with the
client integration (e.g. `builder.AddRedisClient("cache")`). See the
[Aspire integrations](https://aspire.dev/integrations/gallery/).
