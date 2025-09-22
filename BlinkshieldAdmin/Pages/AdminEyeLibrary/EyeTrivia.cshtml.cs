using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlinkshieldAdmin.Pages.AdminEyeLibrary
{
    public class EyeTriviaModel : PageModel
    {
        private readonly BlinkDbContext _db;

        public EyeTriviaModel(BlinkDbContext context)
        {
            _db = context;
        }

        public List<EyeTrivia> TriviaList { get; set; } = new();

        [BindProperty]
        public TriviaInput NewTrivia { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _db.EyeTrivia.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(SearchTerm) ||
                    t.Description.Contains(SearchTerm));
            }

            TriviaList = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        // Add Trivia
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newTrivia = new EyeTrivia
            {
                TriviaId = await GetNextAvailableTriviaIDAsync(),
                Title = NewTrivia.Title.Trim(),
                Description = NewTrivia.Description.Trim(),
                Link = string.IsNullOrWhiteSpace(NewTrivia.Link) ? null : NewTrivia.Link.Trim(),
                CreatedAt = DateTime.Now
            };

            _db.EyeTrivia.Add(newTrivia);
            await _db.SaveChangesAsync();

            await LogAdminHistoryAsync("Create", newTrivia.TriviaId, $"Added Eye Trivia: {newTrivia.Title}", null, newTrivia);

            TempData["Message"] = "New Eye Trivia added successfully!";
            return RedirectToPage();
        }

        // Edit Trivia
        public async Task<IActionResult> OnPostEditTriviaAsync(int id)
        {
            if (!ModelState.IsValid) return Page();

            var trivia = await _db.EyeTrivia.FindAsync(id);
            if (trivia == null) return NotFound();

            var oldValues = JsonConvert.SerializeObject(trivia);

            trivia.Title = NewTrivia.Title?.Trim();
            trivia.Description = NewTrivia.Description?.Trim();
            trivia.Link = string.IsNullOrWhiteSpace(NewTrivia.Link) ? null : NewTrivia.Link.Trim();

            await _db.SaveChangesAsync();

            await LogAdminHistoryAsync("Update", trivia.TriviaId, $"Updated Eye Trivia: {trivia.Title}", oldValues, trivia);

            TempData["Message"] = "Trivia updated successfully!";
            return RedirectToPage();
        }

        // Delete Trivia
        public async Task<IActionResult> OnPostDeleteTriviaAsync(int id)
        {
            var trivia = await _db.EyeTrivia.FindAsync(id);
            if (trivia != null)
            {
                var oldValues = JsonConvert.SerializeObject(trivia);

                _db.EyeTrivia.Remove(trivia);
                await _db.SaveChangesAsync();

                await LogAdminHistoryAsync("Delete", trivia.TriviaId, $"Deleted Eye Trivia: {trivia.Title}", oldValues, null);

                TempData["Message"] = "Trivia deleted successfully!";
            }

            return RedirectToPage();
        }

        // Helpers
        private async Task<int> GetNextAvailableTriviaIDAsync()
        {
            var ids = await _db.EyeTrivia
                .Select(t => t.TriviaId)
                .OrderBy(i => i)
                .ToListAsync();

            int nextId = 1;
            foreach (var id in ids)
            {
                if (id == nextId) nextId++;
                else break;
            }
            return nextId;
        }

        private async Task LogAdminHistoryAsync(string action, int recordId, string description, object? oldValues, object? newValues)
        {
            var history = new AdminHistory
            {
                Action = action,
                TableName = "EyeTrivia",
                RecordId = recordId,
                CreatedAt = DateTime.Now,
                Description = description,
                OldValues = oldValues != null ? JsonConvert.SerializeObject(oldValues) : null,
                NewValues = newValues != null ? JsonConvert.SerializeObject(newValues) : null
            };
            _db.AdminHistories.Add(history);
            await _db.SaveChangesAsync();
        }

        public class TriviaInput
        {
            [Required, MaxLength(255)]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [MaxLength(500), Url]
            public string? Link { get; set; }
        }
    }
}
