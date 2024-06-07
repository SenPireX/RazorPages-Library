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
    private readonly LoanRepository _loans;
    private readonly BookRepository _books;
    private readonly AuthService _authService;
    
    public DetailsModel(IMapper mapper, LoanRepository loans, LibraryRepository libraries, BookRepository books, AuthService authService)
    {
        _mapper = mapper;
        _libraries = libraries;
        _loans = loans;
        _books = books;
        _authService = authService;
    }
    
    [FromRoute] public Guid Guid { get; set; }

    public LoanDto NewLoan { get; set; } = default!;
    public Application.Model.Library Library { get; private set; } = default!;
    public IReadOnlyList<Loan> Loans { get; private set; } = new List<Loan>();
    public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
    public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();

    public IEnumerable<SelectListItem> BookSelectList => _books.Set.OrderBy(b => b.Title)
        .Select(b => new SelectListItem(b.Title, b.Guid.ToString()));
    
    public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
    {
        if (!ModelState.IsValid) { return Page(); }

        var book = _books.FindByGuid(NewLoan.BookGuid);
        if (book is null || book.IsLoaned)
        {
            ModelState.AddModelError("", "Book is either not available or already loaned.");
            return Page();
        }
        book.IsLoaned = true;
        _books.Insert(book);
        
        var (success, message) = _loans.Insert(
            bookGuid: newLoan.BookGuid, 
            libraryGuid: newLoan.LibraryGuid, 
            memberGuid: newLoan.MemberGuid, 
            loanDate: newLoan.LoanDate, 
            dueDate: newLoan.DueDate);
        if (!success)
        {
            ModelState.AddModelError("", message!);
            return Page();
        }
        return RedirectToPage();
    }

    public IActionResult OnPostEditLoan(Guid guid, Guid loanGuid, Dictionary<Guid, LoanDto> editLoans)
    {
        if (!ModelState.IsValid) { return Page(); }

        var loan = _loans.FindByGuid(loanGuid);
        if (loan is null) { return RedirectToPage(); }

        _mapper.Map(editLoans[loanGuid], loan);
        var (success, message) = _loans.Update(loan, loan.Guid);
        if (!success)
        {
            ModelState.AddModelError("", message!);
            return Page();
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(Guid guid, Dictionary<Guid, bool> loansToDelete)
    {
        var loansDb = _loans.Set.Where(l => l.Library.Guid == guid).ToDictionary(l => l.Guid, l => l);
        var loanGuids = loansToDelete.Where(l => l.Value == true).Select(l => l.Key);

        foreach (var l in loanGuids)
        {
            if (!loansDb.TryGetValue(l, out var loan))
            {
                continue;
            }

            _loans.Delete(loan.Guid);
        }
        return RedirectToPage();
    }
    
    public IActionResult OnGet(Guid guid)
    {
        return Page();
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var library = _libraries.Set
            .Include(l => l.Member)
            .Include(l => l.Loans)
            .ThenInclude(l => l.Book)
            .FirstOrDefault(l => l.Guid == Guid);
        if (library is null)
        {
            context.Result = RedirectToPage("/Libraries/Index");
            return;
        }
        var username = _authService.Username;
        if (!_authService.HasRole("Admin") && username != library.Member?.Username)
        {
            context.Result = new ForbidResult();
            return;
        }
        Library = library;
        Loans = library.Loans.ToList();
        LoansToDelete = Loans.ToDictionary(l => l.Guid, o => false);
        /*EditLoans = _loans.Set.Where(l => l.Library.Guid == Guid)
            .ProjectTo<LoanDto>(_mapper.ConfigurationProvider)
            .ToDictionary(l => l.Guid, l => l);*/
    }
}