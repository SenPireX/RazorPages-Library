using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Library.Application.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Library.Webapp.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly UserRepository _users;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserRepository users, ILogger<IndexModel> logger)
        {
            _users = users;
            _logger = logger;
        }

        public IEnumerable<Application.Model.User> Users { get; private set; } =
            [];
        
        public void OnGet()
        {
            Users = _users.Set
                .Include(u => u.Libraries)
                .OrderBy(u => u.Usertype)
                .ThenBy(u => u.Username)
                .ToList();
            
        }
    }
}
