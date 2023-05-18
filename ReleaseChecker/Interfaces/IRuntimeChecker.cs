namespace PanoramicData.SecurityChecker;

public interface IRuntimeChecker
{
	Task<RuntimeStatus> GetRuntimeStatusAsync(CancellationToken cancellationToken);

	Task<RuntimeStatus> GetRuntimeStatusAsync(Version runtimeVersion, CancellationToken cancellationToken);
}