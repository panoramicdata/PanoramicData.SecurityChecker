using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Services;

/// <summary>
/// Standard implementation of a release loader
/// </summary>
internal class ProductReleasesLoader : IProductReleasesLoader, IDisposable
{
    private bool _disposedValue;
    private IReadOnlyCollection<ProductRelease>? _cachedCollection;
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

    /// <summary>
    /// Load the list of releases for the specified product
    /// </summary>
    /// <param name="product">The product for which releases are required</param>
    /// <returns>A read-only collection of releases for the specified product</returns>
    public async Task<IReadOnlyCollection<ProductRelease>> LoadProductReleasesAsync(Product product, CancellationToken cancellationToken)
    {
        if (_cachedCollection is null)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);
                _cachedCollection ??= await product.GetReleasesAsync();
            }
            finally
            {
                _semaphoreSlim?.Release();
            }
        }

        return _cachedCollection;
    }

    #region IDisposable

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
                _semaphoreSlim.Dispose();
            }

            // Free any unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null
            _disposedValue = true;
        }
    }

    #endregion
}
