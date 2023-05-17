using Microsoft.Deployment.DotNet.Releases;

namespace PanoramicData.SecurityChecker.Interfaces;

/// <summary>
/// Contract for loaders of product collections
/// </summary>
public interface IProductCollectionLoader
{
    /// <summary>
    /// Load the product collection
    /// </summary>
    /// <returns>The collection of products defined in the source</returns>
    Task<ProductCollection> LoadProductCollectionAsync(CancellationToken cancellationToken);
}
