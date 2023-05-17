using System.Reflection;

namespace PanoramicData.SecurityChecker.Test.Fakes;

internal class TestLoaderBase
{
	protected static string GetResourcePath(string fileName)
	{
		var runtimePath = Assembly.GetAssembly(typeof(TestLoaderBase))!.Location;
		var runtimeDirectory = new DirectoryInfo(runtimePath);
		var resourceDirectory = runtimeDirectory!.Parent!.Parent!.Parent!.Parent;

		var resourcePath = Path.Combine(resourceDirectory!.FullName, "Resources", fileName);

		return resourcePath;
	}
}
