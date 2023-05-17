using Microsoft.Deployment.DotNet.Releases;
using PanoramicData.SecurityChecker.Interfaces;

namespace PanoramicData.SecurityChecker.Services;

/// <summary>
/// Standard loader for normal runtime operations
/// </summary>
internal class ProductCollectionLoader : IProductCollectionLoader, IDisposable
{
    private bool _disposedValue;
    private ProductCollection? _cachedCollection;
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

    /// <inheritdoc/>
    public async Task<ProductCollection> LoadProductCollectionAsync(CancellationToken cancellationToken)
    {
        if (_cachedCollection is null)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);
                _cachedCollection ??= await ProductCollection.GetAsync();
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
