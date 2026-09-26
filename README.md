# Dotnet Backend Skeleton

An industrialization-ready starting point for .NET backends: a REST API on **.NET 10**, orchestrated locally by **Aspire**,
with persistence, observability, tests, CI and deployment manifests already wired. Start a new service from it, delete the
sample `Todo` feature, and focus on your domain.

## What's inside

| Concern | Choice |
| --- | --- |
| Runtime | .NET 10 (LTS), C# latest, nullable enabled, `TreatWarningsAsErrors` + recommended analyzers |
| Local orchestration | Aspire 13 AppHost: API + PostgreSQL container + dashboard (logs, traces, metrics) |
| API | ASP.NET Core controllers, RFC 9457 problem details for every error, enums as strings |
| API documentation | Built-in OpenAPI document (`/openapi/v1.json`) + [Scalar](https://scalar.com) UI (`/scalar`), fed by XML comments |
| Persistence | EF Core 10 + PostgreSQL (Npgsql) through the Aspire integration (health check, retries, telemetry) |
| Observability | OpenTelemetry logs, traces and metrics (OTLP), custom business metrics, JSON console logs in production |
| Resilience | Standard resilience handler (retry, circuit breaker, timeouts) on every `HttpClient` |
| Health | `/health` (readiness, includes the database) and `/alive` (liveness) |
| Configuration | Options pattern with data annotations, validated at startup |
| Tests | MSTest 4 on Microsoft.Testing.Platform: unit, in-memory integration (no Docker), Aspire end-to-end |
| Build | Central package management, SDK pinned in `global.json`, `.slnx` solution, `.editorconfig` enforced in build |
| Delivery | GitHub Actions CI, Dependabot, container image built by the .NET SDK (chiseled, non-root), Kubernetes manifests |

## Repository layout

```text
.
├── src/
│   ├── DotnetBackendSkeleton.AppHost/          # Aspire orchestration (local dev & tests)
│   ├── DotnetBackendSkeleton.ServiceDefaults/  # Shared defaults: OpenTelemetry, health checks, resilience, service discovery
│   └── DotnetBackendSkeleton.Api/              # The REST API
│       ├── Controllers/        # HTTP endpoints
│       ├── Contracts/          # Request/response DTOs (the public API surface)
│       ├── Services/           # Business logic
│       ├── Models/             # Domain entities
│       ├── Data/               # DbContext, entity configurations, migrations, migrator
│       ├── Diagnostics/        # Custom metrics
│       ├── Infrastructure/     # Cross-cutting concerns (exception handling, OpenAPI)
│       ├── Options/            # Strongly typed configuration
│       └── Extensions/
├── tests/
│   ├── DotnetBackendSkeleton.UnitTests/         # Services and controllers in isolation
│   ├── DotnetBackendSkeleton.IntegrationTests/  # Whole HTTP pipeline in memory, SQLite instead of PostgreSQL
│   └── DotnetBackendSkeleton.AppHost.Tests/     # End-to-end through Aspire with a real PostgreSQL (needs Docker)
├── devops/k8s/            # Kubernetes manifests
├── docs/conventions.md    # Coding conventions and how-tos
└── .github/               # CI workflow and Dependabot
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (the exact feature band is pinned in `global.json`)
- A container runtime: [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/)
- Optional: the [Aspire CLI](https://aspire.dev/get-started/install-cli/) (`aspire run`)
- An IDE: Visual Studio 2026, Rider or VS Code with C# Dev Kit

### Run

```bash
aspire run
# or, without the Aspire CLI:
dotnet run --project src/DotnetBackendSkeleton.AppHost
```

The Aspire dashboard opens: it starts PostgreSQL, applies the EF Core migrations, starts the API and shows logs, traces and
metrics of every resource. The API resource exposes a link to the Scalar API reference.

Requests samples are available in [`DotnetBackendSkeleton.Api.http`](src/DotnetBackendSkeleton.Api/DotnetBackendSkeleton.Api.http).

### Test

```bash
dotnet test
# without a container runtime, skip the Aspire end-to-end tests:
dotnet test --filter "TestCategory!=RequiresDocker" --ignore-exit-code 8
```

## Start a new service from this skeleton

The repository is also a `dotnet new` template that renames every project, namespace and file, and regenerates secrets IDs:

```bash
dotnet new install .
dotnet new backend-skeleton -n Contoso.Orders -o ../Contoso.Orders
```

Then, in the new repository:

1. Replace the sample `Todo` feature (controller, contracts, service, entity, configuration, metrics, tests) with your domain.
2. Recreate the initial migration: delete `Data/Migrations` and run
   `dotnet tool restore && dotnet ef migrations add InitialCreate --project src/<Name>.Api --output-dir Data/Migrations`.
3. Update `Application:Name` in `appsettings.json`, the container repository in the API `.csproj` and the Kubernetes manifests.
4. Fill in the [service README template](#service-readme-template) below.

## Configuration

| Key | Description | Default |
| --- | --- | --- |
| `ConnectionStrings:appdb` | PostgreSQL connection string. Injected by the AppHost locally. | – (required) |
| `Application:Name` | Application name, used as the OpenAPI title. | `Dotnet Backend Skeleton` |
| `Application:Description` | Application description, used in the OpenAPI document. | |
| `Database:ApplyMigrationsOnStartup` | Apply EF Core migrations when the API starts. | `true` in Development, `false` otherwise |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry collector endpoint. No export when empty. | Injected by the AppHost locally |

Every key can be set through environment variables using `__` as separator (e.g. `ConnectionStrings__appdb`).
Secrets must never be committed: use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) locally and
your platform's secret store once deployed.

## Database migrations

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project src/DotnetBackendSkeleton.Api --output-dir Data/Migrations
```

Locally, migrations are applied at startup. In other environments, apply them from the delivery pipeline, before deploying
the new version: the CI publishes an idempotent SQL script (`migrations` artifact), which is safe to run several times.

## Container image & deployment

The image is built by the .NET SDK, without a Dockerfile, on top of a chiseled (distroless, non-root) ASP.NET image.
It listens on port `8080`.

```bash
dotnet publish src/DotnetBackendSkeleton.Api -c Release -t:PublishContainer
# push to a registry:
dotnet publish src/DotnetBackendSkeleton.Api -c Release -t:PublishContainer -p:ContainerRegistry=myregistry.azurecr.io
```

[`devops/k8s/api.yaml`](devops/k8s/api.yaml) contains a hardened Deployment (probes, resources, read-only file system,
dropped capabilities) and its Service. Aspire can also generate deployment artifacts for Docker Compose, Kubernetes or Azure
from the AppHost with `aspire publish` / `aspire deploy`.

## Continuous integration

[`.github/workflows/ci.yml`](.github/workflows/ci.yml) runs on every push and pull request to `main`:
formatting check, build, all tests (including Aspire end-to-end tests) with coverage, idempotent migration script, and
container image build. Dependabot keeps NuGet packages and GitHub Actions up to date.

## Troubleshooting

- **`Unable to load DLL 'e_sqlite3'` or other path errors on Windows**: the path is longer than 260 characters. Clone the
  repository closer to the drive root or [enable long paths](https://learn.microsoft.com/windows/win32/fileio/maximum-file-path-limitation).
- **The AppHost fails to start PostgreSQL**: check that Docker/Podman is running.
- **`Zero tests ran` (exit code 8)**: a test filter excluded every test of a project; add `--ignore-exit-code 8`.

---

## Service README template

Once the skeleton is turned into a real service, replace this file with the sections below.

```markdown
# <Service name>

## Introduction
_General description of the service._

## Dependencies
_Resources and services this service depends on._

## Getting started
_Prerequisites, configuration (appsettings), how to run and test._

## Troubleshooting
_Known issues during build, run or tests._
```
