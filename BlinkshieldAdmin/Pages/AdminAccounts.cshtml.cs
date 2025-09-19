using Microsoft.AspNetCore.Mvc.RazorPages;
using BlinkshieldAdmin.Models;
using BlinkshieldAdmin.Data;
using System.Collections.Generic;
using System.Linq;

namespace BlinkshieldAdmin.Pages
{
    public class AdminAccountsModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public AdminAccountsModel(BlinkDbContext context)
        {
            _context = context;
        }

        public List<AdminAccount> Accounts { get; set; }

        public void OnGet()
        {
            Accounts = _context.AdminAccounts.ToList();
        }
    }
}
