using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

public class AddModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly LoanRepository _loan;
    private readonly BookRepository _book;
    private readonly MemberRepository _member;
    private readonly AuthService _authService;
    private readonly IMapper _mapper;

    public AddModel(IMapper mapper, LibraryRepository library, LoanRepository loan, BookRepository book,
        MemberRepository member, AuthService authService)
    {
        _mapper = mapper;
        _library = library;
        _loan = loan;
        _book = book;
        _member = member;
        _authService = authService;
    }

    [FromRoute] public Guid Guid { get; private set; }
    [BindProperty] public LoanDto NewLoan { get; set; } = default!;
    public Application.Model.Library Library { get; private set; } = default!;
    public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
    public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();
    public IReadOnlyList<Loan> Loans { get; set; } = new List<Loan>();

    public IEnumerable<SelectListItem> BookSelectList =>
        _book.Set.OrderBy(b => b.Title).Select(b => new SelectListItem(b.Title, b.Guid.ToString()));

    /*public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (success, message) = _loan
            .Insert();

        if (!success)
        {
        }

        return RedirectToPage();
    }*/

    public IActionResult OnGet(Guid guid)
    {
        return Page();
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var library = _library.Set
            .Include(l => l.Loans)
            .ThenInclude(l => l.Book)
            .Include(l => l.Loans)
            .ThenInclude(l => l.Member)
            .FirstOrDefault(l => l.Guid == Guid);

        if (library is null)
        {
            context.Result = RedirectToPage("/Stores/Index");
            return;
        }

        Library = library;
        LoansToDelete = library.Loans.ToDictionary(l => l.Guid, l => false);
        var loans = _loan.Set
            .Where(l => l.Guid == Guid)
            .Select(l => new LoanDto(
                l.Guid,
                l.LoanDate,
                l.DueDate,
                l.BookGuid,
                l.LibraryGuid
            ))
            .ToList();

        EditLoans = loans.ToDictionary(l => l.Guid, l => l);
    }
}

/*using AutoMapper;
using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Library.Webapp.Pages.Libraries;

public class AddModel : PageModel
{
private readonly LibraryRepository _libraries;
private readonly LoanRepository _loans;
private readonly BookRepository _books;
private readonly IMapper _mapper;
private readonly AuthService _authService;

public AddModel(IMapper mapper, LibraryRepository libraries, LoanRepository loans, BookRepository books,
   AuthService authService)
{
   _mapper = mapper;
   _libraries = libraries;
   _loans = loans;
   _books = books;
   _authService = authService;
}

[FromRoute] public Guid Guid { get; private set; }
public LoanDto NewLoan { get; set; } = default!;
public Application.Model.Library Library { get; private set; } = default!;
public IReadOnlyList<Loan> Loans { get; private set; } = new List<Loan>();
[BindProperty] public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();

public IEnumerable<SelectListItem> BookSelectList => _books.Set.OrderBy(b => b.Title)
   .Select(b => new SelectListItem(b.Title, b.Guid.ToString()));

public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
{
   if (!ModelState.IsValid)
   {
       return Page();
   }

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

public IActionResult OnGet(Guid guid)
{
   Guid = guid;
   var library = _libraries.Set
       .Include(l => l.Manager)
       .Include(l => l.Loans)
       .ThenInclude(l => l.Book)
       .Include(l => l.Loans)
       .ThenInclude(l => l.Member)
       .FirstOrDefault(l => l.Guid == guid);
   if (library is null)
   {
       return RedirectToPage("/Libraries/Index");
   }

   Library = library;
   LoansToDelete = library.Loans.ToDictionary(l => l.Guid, l => false);

   var loans = _loans.Set.Where(l => l.LibraryGuid == guid)
       .Select(l => new LoanDto(
           l.Guid,
           l.LoanDate,
           l.DueDate,
           l.BookGuid,
           l.LibraryGuid,
           l.MemberGuid
       )).ToList();

   var username = _authService.Username;
   if (!_authService.HasRole("Admin") && username != library.Manager.Username)
   {
       return Forbid();
   }

   EditLoans = loans.ToDictionary(l => l.Guid, l => l);

   return Page();
}
}*/