using System.Text.Json.Serialization;

namespace DotnetBackendSkeleton.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter<TodoStatus>))]
public enum TodoStatus
{
    Todo,
    InProgress,
    Done,
}
