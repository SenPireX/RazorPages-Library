using Library.Application.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.Libraries;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly LibraryRepository _libraries;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(LibraryRepository libraries, ILogger<DeleteModel> logger)
    {
        _libraries = libraries;
        _logger = logger;
    }
    
    [TempData] public string? Message { get; set; }
    public Application.Model.Library Library { get; set; } = default!;
    public IActionResult OnPostCancel() => RedirectToPage("/Libraries/Index");
    public IActionResult OnPostDelete(Guid guid)
    {
        _logger.LogInformation("OnPostDelete called with GUID: {Guid}", guid);
        
        var library = _libraries.FindByGuid(guid);
        if (library is null)
        {
            _logger.LogWarning("Library not found for GUID: {Guid}", guid);
            return RedirectToPage("/Libraries/Index");
        }

        var (success, message) = _libraries.Delete(guid);
        if (!success)
        {
            _logger.LogError("Failed to delete library: {Message}", message);
            Message = message;
        }
        
        _logger.LogInformation("Library deleted successfully");
        return RedirectToPage("/Libraries/Index");
    }
    
    public IActionResult OnGet(Guid guid)
    {
        _logger.LogInformation("OnGet called with GUID: {Guid}", guid);

        var library = _libraries.FindByGuid(guid);
        if (library is null)
        {
            _logger.LogWarning("Library not found for GUID: {Guid}", guid);
            return RedirectToPage("/Libraries/Index");
        }

        Library = library;
        _logger.LogInformation("Library found: {LibraryName}", Library.Name);
        return Page();
    }

}