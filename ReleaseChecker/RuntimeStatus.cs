using Microsoft.Deployment.DotNet.Releases;

namespace PanoramicData.SecurityChecker;

public class RuntimeStatus
{
	public Version RuntimeVersion { get; internal set; } = new();

	public PatchingStatus PatchingStatus { get; internal set; } = PatchingStatus.Unknown;

	public IEnumerable<Cve> UnpatchedCves { get; internal set; } = Enumerable.Empty<Cve>();
}