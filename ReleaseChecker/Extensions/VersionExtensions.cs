namespace PanoramicData.SecurityChecker.Extensions;

internal static class VersionExtensions
{
	// Conversion of .NET's Version type to our own RuntimeVersion
	internal static RuntimeVersion ToRuntimeVersion(this Version version)
		=> new(version.Major, version.Minor, version.Build, version.Revision);
}
