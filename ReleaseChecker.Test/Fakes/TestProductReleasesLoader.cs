using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class TestProductReleasesLoader : TestLoaderBase, IProductReleasesLoader
{
    private readonly string _releasesFileName;

    public TestProductReleasesLoader(string releasesFileName)
    {
        _releasesFileName = releasesFileName;
    }

    public async Task<IReadOnlyCollection<ProductRelease>> LoadProductReleasesAsync(Product product, CancellationToken _)
    {
        var releasesPath = GetResourcePath(_releasesFileName);
        return await product.GetReleasesAsync(releasesPath, false);
    }
}
