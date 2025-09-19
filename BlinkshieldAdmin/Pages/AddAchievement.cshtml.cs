using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BlinkshieldAdmin.Pages
{
    public class AddAchievementModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public AddAchievementModel(BlinkDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Achievement Achievement { get; set; } = new Achievement(); // Avoid CS8618

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Add new Achievement
            _context.Achievements.Add(Achievement);
            await _context.SaveChangesAsync(); // Save first to get AchievementId

            // 2. Create AdminHistory entry (including pass)
            var history = new AdminHistory
            {
                Action = "Create Achievement",
                TableName = "Achievement",
                RecordId = Achievement.AchievementId,
                CreatedAt = DateTime.Now,
                Description = Achievement.Title,
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    Achievement.Title,
                    Achievement.Description
                })
            };

            _context.AdminHistories.Add(history);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Achievements");
        }
    }
}
