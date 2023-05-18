namespace PanoramicData.SecurityChecker;

public enum PatchingStatus : byte
{
	FullyPatched = 0,
	MissingNonSecurityPatches = 1,
	Unknown = 2,
	RequiresPatching = 3,
	OutOfSupport = 4,
}
