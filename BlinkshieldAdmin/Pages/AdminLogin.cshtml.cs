using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using System;

namespace BlinkshieldAdmin.Pages
{
    public class AdminLoginModel : PageModel
    {
        [BindProperty]
        public string Code { get; set; }

        public string ErrorMessage { get; set; }

        private const string AdminAccessCode = "!2520BLNKSHLD2620*";
        private readonly BlinkDbContext _context;

        public AdminLoginModel(BlinkDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ErrorMessage = string.Empty;

            // Check if already logged in
            if (HttpContext.Session.GetString("IsAdminLoggedIn") == "true")
            {
                return RedirectToPage("/AdminAccounts");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (Code == AdminAccessCode)
            {
                // ? Store session variables
                HttpContext.Session.SetString("IsAdminLoggedIn", "true");
                return RedirectToPage("/AdminAccounts");
            }
            else
            {
                ErrorMessage = "Invalid admin code.";
                return Page();
            }
        }
    }
}
