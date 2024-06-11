using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;

namespace Library.Webapp.Pages.Books
{
    [Authorize(Roles = "Admin, Owner")]
    public class IndexModel : PageModel
    {
        private readonly BookRepository _books;

        public IndexModel(BookRepository books)
        {
            _books = books;
        }

        public IEnumerable<Book> Books => _books.Set.OrderBy(b => b.Title);
        
        public void OnGet() {}
    }
}