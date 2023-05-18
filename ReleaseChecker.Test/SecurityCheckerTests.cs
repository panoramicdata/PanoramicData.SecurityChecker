using FluentAssertions;
using Microsoft.Extensions.Logging;
using PanoramicData.SecurityChecker.Test.Fakes;
using Xunit.Abstractions;

namespace PanoramicData.SecurityChecker.Test;

public class SecurityCheckerTests : TestBase
{
	private readonly ILogger<SecurityChecker> _logger;
	private readonly ILogger<RuntimeChecker> _runtimeLogger;

	public SecurityCheckerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
		_logger = new TestLogger<SecurityChecker>(testOutputHelper.BuildLogger());
		_runtimeLogger = new TestLogger<RuntimeChecker>(testOutputHelper.BuildLogger());
	}

	#region GetSecurityStatusAsync

	[Fact]
	public async Task GetStatusAsync_VersionIsNotTheLatest_ReturnsCorrectVersion()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.RuntimeVersion.Major.Should().Be(runtimeVersion.Major);
		result.RuntimeVersion.Minor.Should().Be(runtimeVersion.Minor);
		result.RuntimeVersion.Build.Should().Be(runtimeVersion.Build);
		result.RuntimeVersion.Revision.Should().Be(runtimeVersion.Revision);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_VersionIsTheLatest_ReturnsFullyPatched()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.FullyPatched);
	}

	[Fact]
	public async Task GetStatusAsync_VersionIsLatest_ReturnsCorrectVersionName()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.RuntimeVersion.ToString().Should().Be("7.0.5");
	}

	[Fact]
	public async Task GetStatusAsync_VersionIsNotTheLatest_ReturnsCorrectVersionName()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.RuntimeVersion.ToString().Should().Be("7.0.4");
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerContainASecurityRelease_ReturnsRequiresPatching()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.RequiresPatching);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_ProductCollectionLoaderThrowsException_ReturnsUnknownPatchingStatus()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new ExceptionThrowingProductCollectionLoader();
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.Unknown);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_ProductReleasesLoaderThrowsException_ReturnsUnknownPatchingStatus()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new ExceptionThrowingProductReleasesLoader();

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.PatchingStatus.Should().Be(PatchingStatus.Unknown);
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_VersionIsTheLatest_ReturnsNoCves()
	{
		var runtimeVersion = new Version(7, 0, 5, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().BeEmpty();
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerAreAllNonSecurity_ReturnsNoCves()
	{
		var runtimeVersion = new Version(7, 0, 3, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v704Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().BeEmpty();
	}

	[Fact]
	public async Task GetRuntimeStatusAsync_NewerContainASecurityRelease_ReturnsACve()
	{
		var runtimeVersion = new Version(7, 0, 4, 0);

		var productCollectionLoader = new TestProductCollectionLoader("v7Index.json");
		var productReleasesLoader = new TestProductReleasesLoader("v705Releases.json");

		var checker = new SecurityChecker(new RuntimeChecker(productCollectionLoader, productReleasesLoader, _runtimeLogger), _logger);
		var result = await checker.GetSecurityStatusAsync(runtimeVersion, CancellationToken.None);

		result.UnpatchedCves.Should().NotBeEmpty();
		var cveList = result.UnpatchedCves.ToList();

		cveList.Should().HaveCount(1);
		cveList[0].Id.Should().Be("CVE-2023-28260");
	}

	#endregion
}