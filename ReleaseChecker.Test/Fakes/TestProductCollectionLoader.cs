using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class TestProductCollectionLoader : TestLoaderBase, IProductCollectionLoader
{
    private readonly string _indexFileName;

    public TestProductCollectionLoader(string indexFileName)
    {
        _indexFileName = indexFileName;
    }

    public Task<ProductCollection> LoadProductCollectionAsync(CancellationToken _)
    {
        var indexPath = GetResourcePath(_indexFileName);
        return ProductCollection.GetFromFileAsync(indexPath, false);
    }
}
