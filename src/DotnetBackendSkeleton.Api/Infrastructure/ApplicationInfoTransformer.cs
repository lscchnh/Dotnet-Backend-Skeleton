using DotnetBackendSkeleton.Api.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace DotnetBackendSkeleton.Api.Infrastructure;

/// <summary>
/// Fills the OpenAPI document information from the <see cref="ApplicationOptions"/>.
/// </summary>
public sealed class ApplicationInfoTransformer(IOptions<ApplicationOptions> options) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = options.Value.Name;
        document.Info.Description = options.Value.Description;

        return Task.CompletedTask;
    }
}
