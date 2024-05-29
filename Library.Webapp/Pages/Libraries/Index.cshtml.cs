﻿using Library.Application.Infrastructure.Repositories;
using Library.Webapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Library.Webapp.Pages.Libraries;

public class IndexModel : PageModel
{
    private readonly LibraryRepository _library;
    private readonly AuthService _authService;

    public IndexModel(LibraryRepository library, AuthService authService)
    {
        _library = library;
        _authService = authService;
    }

    [TempData] public string? Message { get; set; }

    public IReadOnlyList<LibraryRepository.LibrariesWithBooksCount> Libraries { get; private set; } =
        new List<LibraryRepository.LibrariesWithBooksCount>();

    public void OnGet()
    {
        //Libraries = _library.GetLibrariesWithBooksCount();
    }

    public bool CanEditLibrary(Guid libraryGuid) =>
        _authService.IsAdmin
        || Libraries.FirstOrDefault(s => s.Guid == libraryGuid)?.Member?.Username == _authService.Username;
}