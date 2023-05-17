using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PanoramicData.SecurityChecker.AspNetCore.Controllers;

/// <summary>
/// Controller for exposing security checks
/// </summary>
[Route("{controller}")]
public class SecurityChecksController : ControllerBase
{
    private const string _cacheKey = "pdl-securitychecks";
    private readonly ISecurityChecker _securityChecker;
    private readonly IDistributedCache _cache;
    private readonly ILogger<SecurityChecksController> _logger;

    public SecurityChecksController(ISecurityChecker securityChecker, IDistributedCache cache, ILogger<SecurityChecksController> logger)
    {
        _securityChecker = securityChecker;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get the security status of the system
    /// </summary>
    /// <returns>The security status as determined by the core checker</returns>
    [HttpGet("")]
    public async Task<ActionResult<string>> GetAsync(CancellationToken cancellationToken)
    {
        // TODO: Add throttling to protect the site
        // TODO: Add auth. This is currently public!
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting security status");
        }

        var cacheEntry = await GetCacheEntryAsync(cancellationToken).ConfigureAwait(false);
        if (cacheEntry is null)
        {
            var securityStatus = await _securityChecker
                .GetSecurityStatusAsync(cancellationToken)
                .ConfigureAwait(false);

            cacheEntry = JsonSerializer.Serialize(securityStatus);
            await AddToCacheAsync(cacheEntry, cancellationToken).ConfigureAwait(false);
        }

        return cacheEntry;
    }

    private async Task<string?> GetCacheEntryAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _cache
                .GetStringAsync(_cacheKey, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to access cache security status in cache! Message: {Message}", ex.Message);
        }

        return null;
    }

    private async Task AddToCacheAsync(string cacheEntry, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30),
        };
        try
        {
            await _cache
                .SetStringAsync(_cacheKey, cacheEntry, options, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to cache security status! Message: {Message}", ex.Message);
        }
    }
}
