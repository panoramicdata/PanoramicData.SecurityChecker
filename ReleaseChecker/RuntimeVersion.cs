namespace PanoramicData.SecurityChecker;

/// <summary>
/// DTO to expose information about the runtime version
/// </summary>
public class RuntimeVersion
{
	internal RuntimeVersion()
	{
	}

	public RuntimeVersion(int major, int minor, int build, int revision)
	{
		Major = major;
		Minor = minor;
		Build = build;
		Revision = revision;
	}

	/// <summary>
	/// The Major version number
	/// </summary>
	public int Major { get; internal set; }

	/// <summary>
	/// The Minor version number
	/// </summary>
	public int Minor { get; internal set; }

	/// <summary>
	/// The Build number
	/// </summary>
	public int Build { get; internal set; }

	/// <summary>
	/// The Revision number (rarely used)
	/// </summary>
	public int Revision { get; internal set; }

	/// <summary>
	/// Override of the ToString method to provide a valuable textual version
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		var versionString = $"{Major}.{Minor}.{Build}";
		if (Revision > 0)
		{
			versionString += "." + Revision;
		}

		return versionString;
	}
}
