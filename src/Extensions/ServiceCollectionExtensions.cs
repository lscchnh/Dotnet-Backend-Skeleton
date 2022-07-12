using Microsoft.Extensions.Options;

namespace BackendSkeleton.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static OptionsBuilder<T> ConfigureOptions<T>(
		this IServiceCollection services, string section) where T : class
		{
			return services
					   .AddOptions<T>()
					   .BindConfiguration(section)
					   .ValidateDataAnnotations()
					   .ValidateOnStart();
		}
	}
}
