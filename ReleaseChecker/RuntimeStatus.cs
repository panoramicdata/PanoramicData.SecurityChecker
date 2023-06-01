using Microsoft.Deployment.DotNet.Releases;

namespace PanoramicData.SecurityChecker;

/// <summary>
/// DTO used to transport information about the runtime
/// </summary>
public class RuntimeStatus
{
	/// <summary>
	/// The version of the runtime on which the hosting application is running
	/// </summary>
	public RuntimeVersion Version { get; internal set; } = new();

	/// <summary>
	/// The textual name of the runtime version
	/// </summary>
	public string VersionName => Version.ToString();

	/// <summary>
	/// The patching status of the runtime
	/// </summary>
	public PatchingStatus PatchingStatus { get; internal set; } = PatchingStatus.Unknown;

	/// <summary>
	/// Any CVEs that the host application is currently vulnerable due to missing security patches
	/// </summary>
	public IEnumerable<Cve> UnpatchedCves { get; internal set; } = Enumerable.Empty<Cve>();

	/// <summary>
	/// The number of days until the runtime is no longer supported
	/// </summary>
	public int DaysToEndOfSupport { get; internal set; }
}