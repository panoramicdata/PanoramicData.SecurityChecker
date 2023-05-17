namespace PanoramicData.SecurityChecker;

public interface ISecurityChecker
{
    Task<SecurityStatus> GetSecurityStatusAsync(CancellationToken cancellationToken);
}
