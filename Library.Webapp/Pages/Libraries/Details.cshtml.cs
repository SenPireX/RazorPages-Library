using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Application.Dto;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library.Webapp.Pages.Libraries
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly LibraryRepository _libraries;
        private readonly LoanRepository _loans;
        private readonly BookRepository _books;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public DetailsModel(IMapper mapper, LibraryRepository libraries, LoanRepository loans, BookRepository books,
            AuthService authService)
        {
            _mapper = mapper;
            _libraries = libraries;
            _loans = loans;
            _books = books;
            _authService = authService;
        }

        [FromRoute] public Guid Guid { get; set; }
        public LoanDto NewLoan { get; set; } = default!;
        public Library.Application.Model.Library Library { get; private set; } = default!;
        public IReadOnlyList<Loan> Loans { get; private set; } = new List<Loan>();
        public Dictionary<Guid, LoanDto> EditLoans { get; set; } = new();
        public Dictionary<Guid, bool> LoansToDelete { get; set; } = new();

        public IEnumerable<SelectListItem> BookSelectList =>
            _books.Set.OrderBy(p => p.Title).Select(p => new SelectListItem(p.Title, p.Guid.ToString()));

        public IActionResult OnPostEditLoan(Guid guid, Guid loanGuid, Dictionary<Guid, LoanDto> editLoans)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loan = _loans.FindByGuid(loanGuid);
            if (loan is null)
            {
                return RedirectToPage();
            }

            _mapper.Map(editLoans[loanGuid], loan);
            var (success, message) = _loans.Update(loan, guid);
            if (!success)
            {
                ModelState.AddModelError("", message!);
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostNewLoan(Guid guid, LoanDto newLoan)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (success, message) = _loans.Insert(
                bookGuid: newLoan.BookGuid,
                libraryGuid: newLoan.LibraryGuid,
                memberGuid: newLoan.MemberGuid,
                loanDate: newLoan.LoanDate,
                dueDate: newLoan.DueDate
            );
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
            var loanGuids = loansToDelete.Where(o => o.Value == true).Select(l => l.Key);

            foreach (var l in loanGuids)
            {
                if (!loansDb.TryGetValue(l, out var loan))
                {
                    continue;
                }

                _loans.Delete(guid); // TODO -  Statt guid loan?
            }
            return RedirectToPage();
        }

        public IActionResult OnGet(Guid guid) { return Page(); }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var library = _libraries.Set
                .Include(s => s.Member)
                .Include(s => s.Loans)
                .ThenInclude(o => o.Book)
                .FirstOrDefault(s => s.Guid == Guid);
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
            LoansToDelete = Loans.ToDictionary(o => o.Guid, o => false);
            EditLoans = _loans.Set.Where(o => o.Library.Guid == Guid)
                .ProjectTo<LoanDto>(_mapper.ConfigurationProvider)
                .ToDictionary(o => o.Guid, o => o);
        }
    }
}