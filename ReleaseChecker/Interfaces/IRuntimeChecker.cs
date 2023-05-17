namespace PanoramicData.SecurityChecker;

public interface IRuntimeChecker
{
    Task<RuntimeStatus> GetRuntimeStatusAsync(CancellationToken cancellationToken);
}