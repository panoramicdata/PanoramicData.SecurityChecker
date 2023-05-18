using Microsoft.Deployment.DotNet.Releases;

namespace PanoramicData.SecurityChecker;

/// <summary>
/// DTO used to transport information about the application's security
/// </summary>
public class SecurityStatus
{
	/// <summary>
	/// The version of the runtime on which the hosting application is running
	/// </summary>
	public RuntimeVersion RuntimeVersion { get; internal set; } = new();

	/// <summary>
	/// The textual representation of the version of the runtime on which the hosting application is running
	/// </summary>
	public string RuntimeVersionName => RuntimeVersion.ToString();

	/// <summary>
	/// The patching status of the runtime
	/// </summary>
	public PatchingStatus PatchingStatus { get; internal set; } = PatchingStatus.Unknown;

	/// <summary>
	/// Any CVEs that the host application is currently vulnerable to due to missing security patches
	/// </summary>
	public IEnumerable<Cve> UnpatchedCves { get; internal set; } = Enumerable.Empty<Cve>();

	/// <summary>
	/// The number of days until the runtime is no longer supported
	/// </summary>
	public int DaysToEndOfSupport { get; internal set; }
}