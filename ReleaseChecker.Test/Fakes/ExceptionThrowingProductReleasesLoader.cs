using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class ExceptionThrowingProductReleasesLoader : TestLoaderBase, IProductReleasesLoader
{
    public ExceptionThrowingProductReleasesLoader()
    {
    }

    public Task<IReadOnlyCollection<ProductRelease>> LoadProductReleasesAsync(Product product, CancellationToken _)
    {
        throw new NotImplementedException();
    }
}
