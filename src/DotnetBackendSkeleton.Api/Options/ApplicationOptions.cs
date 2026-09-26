using System.ComponentModel.DataAnnotations;

namespace DotnetBackendSkeleton.Api.Options;

/// <summary>
/// General information about the application, bound to the <c>Application</c> configuration section.
/// </summary>
public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    /// <summary>
    /// The display name of the application (used as the OpenAPI document title).
    /// </summary>
    [Required]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// A short description of the application (used as the OpenAPI document description).
    /// </summary>
    public string? Description { get; init; }
}
