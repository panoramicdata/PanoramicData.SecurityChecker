using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class ExceptionThrowingProductCollectionLoader : TestLoaderBase, IProductCollectionLoader
{
    public ExceptionThrowingProductCollectionLoader()
    {
    }

    public Task<ProductCollection> LoadProductCollectionAsync(CancellationToken _)
    {
        throw new NotImplementedException();
    }
}
