using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PanoramicData.SecurityChecker.Web.Demo.Pages;
public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;

	public IndexModel(ILogger<IndexModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
		// No implementation required at this stage
	}
}
