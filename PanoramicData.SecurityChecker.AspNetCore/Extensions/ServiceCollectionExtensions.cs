using PanoramicData.SecurityChecker.AspNetCore.Controllers;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSecurityChecks(this IServiceCollection services)
	{
		var builder = services.AddMvcCore();
		builder.AddApplicationPart(typeof(SecurityChecksController).Assembly);

		services.AddCoreSecurityChecking();
		return services;
	}
}
