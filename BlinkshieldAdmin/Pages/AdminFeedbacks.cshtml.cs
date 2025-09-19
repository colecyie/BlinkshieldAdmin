using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlinkshieldAdmin.Pages
{
    public class AdminFeedbacksModel : PageModel
    {
        private readonly BlinkDbContext _db;

        public AdminFeedbacksModel(BlinkDbContext db)
        {
            _db = db;
        }

        public List<Feedback> Feedbacks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _db.Feedback.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(f =>
                    (f.Name != null && f.Name.Contains(SearchTerm)) ||
                    (f.Email != null && f.Email.Contains(SearchTerm)) ||
                    f.Message.Contains(SearchTerm));
            }

            Feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}
