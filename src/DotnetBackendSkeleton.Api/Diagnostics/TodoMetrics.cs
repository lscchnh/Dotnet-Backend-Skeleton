using System.Diagnostics.Metrics;
using DotnetBackendSkeleton.Api.Models;

namespace DotnetBackendSkeleton.Api.Diagnostics;

/// <summary>
/// Business metrics, exported through OpenTelemetry (Aspire dashboard locally, OTLP backend in production).
/// The meter is named after the application so that it is picked up by <c>AddServiceDefaults()</c>.
/// </summary>
public sealed class TodoMetrics
{
    private readonly Counter<long> _itemsCreated;
    private readonly Counter<long> _itemsCompleted;

    public TodoMetrics(IMeterFactory meterFactory, IHostEnvironment environment)
    {
        var meter = meterFactory.Create(environment.ApplicationName);

        _itemsCreated = meter.CreateCounter<long>(
            "todo.items.created", unit: "{item}", description: "Number of todo items created.");
        _itemsCompleted = meter.CreateCounter<long>(
            "todo.items.completed", unit: "{item}", description: "Number of todo items moved to the Done status.");
    }

    public void ItemCreated(TodoStatus status) =>
        _itemsCreated.Add(1, new KeyValuePair<string, object?>("todo.status", status.ToString()));

    public void ItemCompleted() => _itemsCompleted.Add(1);
}
