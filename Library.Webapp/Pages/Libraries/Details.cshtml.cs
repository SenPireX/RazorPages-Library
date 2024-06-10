using AutoMapper;
using AutoMapper.QueryableExtensions;
using Library.Application.Dto;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IMapper _mapper;
    private readonly LibraryRepository _libraries;
    private readonly AuthService _authService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IMapper mapper, LibraryRepository libraries, ILogger<DetailsModel> logger,
        AuthService authService)
    {
        _mapper = mapper;
        _libraries = libraries;
        _logger = logger;
        _authService = authService;
    }

    [FromRoute] public Guid Guid { get; set; }
    public Application.Model.Library? Library { get; set; }

    public IActionResult OnGet(Guid guid)
    {
        return Page();
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        _logger.LogInformation("OnPageHandlerExecuting called with Guid: {Guid}", Guid);

        var library = _libraries.Set
            .Include(l => l.Loans)
            .ThenInclude(loan => loan.Member)
            .Include(l => l.Loans)
            .ThenInclude(loan => loan.Book)
            .FirstOrDefault(l => l.Guid == Guid);

        if (library is null)
        {
            _logger.LogWarning("Library not found with Guid: {Guid}", Guid);
            context.Result = RedirectToPage("/Libraries/Index");
            return;
        }

        var username = _authService.Username;
        if (!_authService.HasRole("Admin") && username != library.Manager?.Username)
        {
            _logger.LogWarning("User {Username} does not have access to library {LibraryName}", username, library.Name);
            context.Result = new ForbidResult();
            return;
        }

        _logger.LogInformation("Library found: {LibraryName} with {LoanCount} loans", library.Name,
            library.Loans.Count);
        foreach (var loan in library.Loans)
        {
            _logger.LogInformation(
                "Loan - Member: {LoanMember}, Book: {LoanBook}, LoanDate: {LoanDate}, DueDate: {DueDate}",
                loan.Member?.FirstName + " " + loan.Member?.LastName,
                loan.Book?.Title,
                loan.LoanDate,
                loan.DueDate);
        }

        Library = library;
    }
}