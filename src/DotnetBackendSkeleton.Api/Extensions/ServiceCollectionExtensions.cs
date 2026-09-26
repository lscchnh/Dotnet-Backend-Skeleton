using Microsoft.Extensions.Options;

namespace DotnetBackendSkeleton.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Binds <typeparamref name="T"/> to the given configuration section, validates its data annotations
    /// and fails the application startup if the configuration is invalid.
    /// </summary>
    public static OptionsBuilder<T> AddValidatedOptions<T>(this IServiceCollection services, string section)
        where T : class
    {
        return services
            .AddOptions<T>()
            .BindConfiguration(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
