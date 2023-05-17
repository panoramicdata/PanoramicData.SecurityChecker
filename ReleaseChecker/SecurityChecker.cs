using Microsoft.Extensions.Logging;

namespace PanoramicData.SecurityChecker;

/// <summary>
/// Class for gathering security information for the end consumer
/// </summary>
internal class SecurityChecker : ISecurityChecker
{
    private readonly IRuntimeChecker _runtimeChecker;
    private readonly ILogger _logger;

    public SecurityChecker(IRuntimeChecker runtimeChecker, ILogger<SecurityChecker> logger)
    {
        _runtimeChecker = runtimeChecker;
        _logger = logger;
    }

    /// <summary>
    /// Get the security status of the system
    /// </summary>
    /// <returns>As much of the security status information successfully gathered</returns>
    public async Task<SecurityStatus> GetSecurityStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var runtimeStatus = await _runtimeChecker
                .GetRuntimeStatusAsync(cancellationToken)
                .ConfigureAwait(false);

            return new SecurityStatus
            {
                RuntimeVersion = runtimeStatus.RuntimeVersion,
                PatchingStatus = runtimeStatus.PatchingStatus,
                UnpatchedCves = runtimeStatus.UnpatchedCves,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to gather security status! Message: {Message}", ex.Message);
            return new SecurityStatus
            {
                PatchingStatus = PatchingStatus.Unknown,
            };
        }
    }
}
