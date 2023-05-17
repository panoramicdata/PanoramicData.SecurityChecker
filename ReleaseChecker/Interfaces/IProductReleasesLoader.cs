using Microsoft.Deployment.DotNet.Releases;

namespace PanoramicData.SecurityChecker.Interfaces;

/// <summary>
/// Contract for loaders of product releases
/// </summary>
public interface IProductReleasesLoader
{
    /// <summary>
    /// Get the releases for a specific product
    /// </summary>
    /// <param name="product">The product for which releases are required</param>
    /// <returns>The collection of releases for the product requested</returns>
    Task<IReadOnlyCollection<ProductRelease>> LoadProductReleasesAsync(Product product, CancellationToken cancellationToken);
}
