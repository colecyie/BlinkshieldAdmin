using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace BlinkshieldAdmin.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            // ? Redirect to login if not logged in
            if (HttpContext.Session.GetString("IsAdminLoggedIn") != "true")
            {
                return RedirectToPage("/AdminLogin");
            }

            return Page();
        }
    }
}
