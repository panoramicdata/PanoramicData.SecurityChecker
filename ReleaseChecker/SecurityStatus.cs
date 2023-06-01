namespace PanoramicData.SecurityChecker;

/// <summary>
/// DTO used to transport information about the application's security
/// </summary>
public class SecurityStatus
{
	/// <summary>
	/// The security status of the runtime
	/// </summary>
	public RuntimeStatus Runtime { get; set; } = new();
}