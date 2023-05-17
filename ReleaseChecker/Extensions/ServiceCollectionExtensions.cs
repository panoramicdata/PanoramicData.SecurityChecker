using PanoramicData.SecurityChecker;
using PanoramicData.SecurityChecker.Interfaces;
using PanoramicData.SecurityChecker.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IServiceCollection type, used for DI registrations
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Register the services necessary to use security checking
	/// </summary>
	/// <param name="services">The service collection into which to register services</param>
	/// <returns>The service collection being extended, to support method chaining</returns>
	public static IServiceCollection AddCoreSecurityChecking(this IServiceCollection services)
	{
		services.AddSingleton<IProductCollectionLoader, ProductCollectionLoader>();
		services.AddSingleton<IProductReleasesLoader, ProductReleasesLoader>();
		services.AddTransient<IRuntimeChecker, RuntimeChecker>();
		services.AddTransient<ISecurityChecker, SecurityChecker>();
		return services;
	}
}
