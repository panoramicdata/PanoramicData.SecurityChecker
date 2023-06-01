using FluentAssertions;
using Microsoft.Extensions.Logging;
using PanoramicData.SecurityChecker.Test.Fakes;
using Xunit.Abstractions;

namespace PanoramicData.SecurityChecker.Test;

public class RuntimeCheckerTests : TestBase
{
	private readonly ILogger<RuntimeChecker> _logger;

	public RuntimeCheckerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
		_logger = new TestLogger<RuntimeChecker>(testOutputHelper.BuildLogger());
	}

	#region GetRuntimeStatusAsync

	[Fact]
	public async Task GetRuntimeStatusAsync_VersionIsNotTheLatest_ReturnsCorrectVersion()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.Version.Major.Should().Be(runtimeVersion.Major);
		result.Version.Minor.Should().Be(runtimeVersion.Minor);
		result.Version.Build.Should().Be(runtimeVersion.Build);
		result.Version.Revision.Should().Be(runtimeVersion.Revision);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_VersionIsTheLatest_ReturnsFullyPatched()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.FullyPatched);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerContainASecurityRelease_ReturnsRequiresPatching()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.RequiresPatching);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_ProductCollectionLoaderThrowsException_ReturnsUnknownPatchingStatus()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new ExceptionThrowingProductCollectionLoader();
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.Unknown);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_ProductReleasesLoaderThrowsException_ReturnsUnknownPatchingStatus()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new ExceptionThrowingProductReleasesLoader();

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.Unknown);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_VersionIsTheLatest_ReturnsNoCves()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().BeEmpty();
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerAreAllNonSecurity_ReturnsNoCves()
	{
		var runtimeVersion = new Version(7, 0, 3, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().BeEmpty();
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerContainASecurityRelease_ReturnsACve()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimeStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().NotBeEmpty();
		var cveList = result.UnpatchedCves.ToList();

		cveList.Should().HaveCount(1);
		cveList[0].Id.Should().Be("CVE-2023-28260");
	}

	#endregion

	#region GetRuntimePatchingStatusAsync

	[Fact]
	public async Task GetRuntimePatchingStatusAsync_VersionIsTheLatest_ReturnsFullyPatched()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimePatchingStatusAsync(runtimeVersion, CancellationToken.None);

		result.Should().Be(PatchingStatus.FullyPatched);
	}

	[Fact]
	public async Task GetRuntimePatchingStatusAsync_NewerContainASecurityRelease_ReturnsRequiresPatching()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimePatchingStatusAsync(runtimeVersion, CancellationToken.None);

		result.Should().Be(PatchingStatus.RequiresPatching);
	}

	[Fact]
	public async Task GetRuntimePatchingStatusAsync_ProductIsOutOfSupport_ReturnsRequiresPatching()
	{
		var runtimeVersion = new Version(5, 0, 17, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v5Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v5017Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);
		var result = await checker.GetRuntimePatchingStatusAsync(runtimeVersion, CancellationToken.None);

		result.Should().Be(PatchingStatus.OutOfSupport);
	}

	#endregion

	#region GetMissingPatchStatusAsync

	[Fact]
	public async Task GetMissingPatchStatusAsync_NewerAreAllNonSecurity_ReturnsMissingNonSecurityPatches()
	{
		var runtimeVersion = new Version(7, 0, 3, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);

		var result = await checker.GetMissingPatchStatusAsync(runtimeVersion, CancellationToken.None);

		result.Should().Be(PatchingStatus.MissingNonSecurityPatches);
	}

	[Fact]
	public async Task GetMissingPatchStatusAsync_NewerContainASecurityRelease_ReturnsRequiresPatching()
	{
		var runtimeVersion = new Version(7, 0, 2, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);

		var result = await checker.GetMissingPatchStatusAsync(runtimeVersion, CancellationToken.None);

		result.Should().Be(PatchingStatus.RequiresPatching);
	}

	#endregion

	#region GetUnpatchedCvesAsync

	[Fact]
	public async Task GetUnpatchedCvesAsync_NewerHaveNoCves_ReturnsEmptyEnumerable()
	{
		var runtimeVersion = new Version(7, 0, 3, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);

		var result = await checker.GetUnpatchedCvesAsync(runtimeVersion, CancellationToken.None);

		result.Should().BeEmpty();
	}

	[Fact]
	public async Task GetUnpatchedCvesAsync_NewerContainsOneCve_ReturnsOneCorrectCve()
	{
		var runtimeVersion = new Version(7, 0, 2, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);

		var result = await checker.GetUnpatchedCvesAsync(runtimeVersion, CancellationToken.None);

		result.Should().NotBeEmpty();
		var resultList = result.ToList();

		resultList.Should().HaveCount(1);
		resultList[0].Id.Should().Be("CVE-2023-21808");
	}

	[Fact]
	public async Task GetUnpatchedCvesAsync_TwoNewerContainACveEach_ReturnsTwoCorrectCves()
	{
		var runtimeVersion = new Version(7, 0, 0, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new RuntimeChecker(productCollectionLoader, productReleasesLoader, _logger);

		var result = await checker.GetUnpatchedCvesAsync(runtimeVersion, CancellationToken.None);

		result.Should().NotBeEmpty();
		var resultList = result.ToList();

		resultList.Should().HaveCount(2);
		resultList[0].Id.Should().Be("CVE-2023-21808");
		resultList[1].Id.Should().Be("CVE-2022-41089");
	}

	#endregion
}