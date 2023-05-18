using Microsoft.Deployment.DotNet.Releases;
using Microsoft.Extensions.Logging;
using PanoramicData.SecurityChecker.Extensions;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker;

internal class RuntimeChecker : IRuntimeChecker
{
	private readonly IProductCollectionLoader _productCollectionLoader;
	private readonly IProductReleasesLoader _productReleasesLoader;
	private readonly ILogger _logger;

	public RuntimeChecker(IProductCollectionLoader productCollectionLoader, IProductReleasesLoader productReleasesLoader, ILogger<RuntimeChecker> logger)
	{
		_productCollectionLoader = productCollectionLoader;
		_productReleasesLoader = productReleasesLoader;
		_logger = logger;
	}

	public Task<RuntimeStatus> GetRuntimeStatusAsync(CancellationToken cancellationToken)
	{
		var runtimeVersion = Environment.Version;
		return GetRuntimeStatusAsync(runtimeVersion, cancellationToken);
	}

	public async Task<RuntimeStatus> GetRuntimeStatusAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		try
		{
			var patchingStatus = await GetRuntimePatchingStatusAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
			var cves = await GetUnpatchedCvesAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
			var daysToEndOfSupport = await GetDaysToEndOfSupportAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);

			return new RuntimeStatus
			{
				RuntimeVersion = runtimeVersion.ToRuntimeVersion(),
				PatchingStatus = patchingStatus,
				UnpatchedCves = cves,
				DaysToEndOfSupport = daysToEndOfSupport,
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to retrieve runtime status! Message: {Message}", ex.Message);
			return new RuntimeStatus
			{
				PatchingStatus = PatchingStatus.Unknown
			};
		}
	}

	private async Task<int> GetDaysToEndOfSupportAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		var daysToEndOfSupport = int.MaxValue;

		var product = await GetProductAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
		var endOfLifeDate = product.EndOfLifeDate ?? DateTime.MaxValue;

		var fractionalDaysToEndOfSupport = endOfLifeDate.Subtract(DateTime.Today).TotalDays;
		if (fractionalDaysToEndOfSupport < int.MaxValue)
		{
			daysToEndOfSupport = (int)fractionalDaysToEndOfSupport;
		}

		return daysToEndOfSupport;
	}

	internal async Task<PatchingStatus> GetRuntimePatchingStatusAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		var product = await GetProductAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
		if (product.IsOutOfSupport())
		{
			return PatchingStatus.OutOfSupport;
		}

		var latestRelease = product.LatestRuntimeVersion;
		if (runtimeVersion.Build != latestRelease.Patch)
		{
			// Not up to date; are we missing any security patches?
			return await GetMissingPatchStatusAsync(runtimeVersion, product, cancellationToken).ConfigureAwait(false);
		}

		return PatchingStatus.FullyPatched;
	}

	internal async Task<Product> GetProductAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		var runtimeVersionString = $"{runtimeVersion.Major}.{runtimeVersion.Minor}";

		var products = await _productCollectionLoader.LoadProductCollectionAsync(cancellationToken).ConfigureAwait(false);
		var product = products.FirstOrDefault(r => r.ProductVersion == runtimeVersionString);
		if (product is null)
		{
			throw new InvalidOperationException("Product is not recognised!");
		}

		return product;
	}

	internal async Task<PatchingStatus> GetMissingPatchStatusAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		var product = await GetProductAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
		return await GetMissingPatchStatusAsync(runtimeVersion, product, cancellationToken).ConfigureAwait(false);
	}

	internal async Task<PatchingStatus> GetMissingPatchStatusAsync(Version runtimeVersion, Product product, CancellationToken cancellationToken)
	{
		var productReleases = await _productReleasesLoader.LoadProductReleasesAsync(product, cancellationToken).ConfigureAwait(false);
		return GetMissingPatchStatusAsync(runtimeVersion, productReleases, cancellationToken);
	}

	internal static PatchingStatus GetMissingPatchStatusAsync(Version runtimeVersion, IReadOnlyCollection<ProductRelease> productReleases, CancellationToken cancellationToken)
	{
		var newerReleases = productReleases.Where(pr => pr.Runtime.Version.Patch > runtimeVersion.Build);
		return newerReleases.Any(pr => pr.IsSecurityUpdate)
			? PatchingStatus.RequiresPatching
			: newerReleases.Any()
				? PatchingStatus.MissingNonSecurityPatches
				: PatchingStatus.FullyPatched;
	}

	internal async Task<IEnumerable<Cve>> GetUnpatchedCvesAsync(Version runtimeVersion, CancellationToken cancellationToken)
	{
		var product = await GetProductAsync(runtimeVersion, cancellationToken).ConfigureAwait(false);
		return await GetUnpatchedCvesAsync(runtimeVersion, product, cancellationToken).ConfigureAwait(false);
	}

	internal async Task<IEnumerable<Cve>> GetUnpatchedCvesAsync(Version runtimeVersion, Product product, CancellationToken cancellationToken)
	{
		var productReleases = await _productReleasesLoader.LoadProductReleasesAsync(product, cancellationToken).ConfigureAwait(false);
		return GetUnpatchedCves(runtimeVersion, productReleases);
	}

	internal static IEnumerable<Cve> GetUnpatchedCves(Version runtimeVersion, IReadOnlyCollection<ProductRelease> productReleases)
	{
		var newerReleases = productReleases.Where(pr => pr.Runtime.Version.Patch > runtimeVersion.Build);
		return newerReleases.SelectMany(pr => pr.Cves);
	}
}
