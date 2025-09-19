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

        // ? ADD TRIVIA
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newTrivia = new EyeTrivia
            {
                Title = NewTrivia.Title.Trim(),
                Description = NewTrivia.Description.Trim(),
                Link = string.IsNullOrWhiteSpace(NewTrivia.Link) ? null : NewTrivia.Link.Trim(),
                CreatedAt = DateTime.Now
            };

            _db.EyeTrivia.Add(newTrivia);
            await _db.SaveChangesAsync();

            // Log to AdminHistory
            var historyEntry = new AdminHistory
            {
                Action = "Create",
                TableName = "EyeTrivia",
                RecordId = newTrivia.TriviaId,
                CreatedAt = DateTime.Now,
                Description = $"Added Eye Trivia: {newTrivia.Title}",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(newTrivia)
            };
            _db.AdminHistories.Add(historyEntry);
            await _db.SaveChangesAsync();

            TempData["Message"] = "New Eye Trivia added successfully!";
            return RedirectToPage();
        }

        // ? EDIT TRIVIA
        public async Task<IActionResult> OnPostEditTriviaAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var trivia = await _db.EyeTrivia.FindAsync(id);
            if (trivia == null)
            {
                return NotFound();
            }

            // Capture old values before update
            var oldTrivia = new EyeTrivia
            {
                TriviaId = trivia.TriviaId,
                Title = trivia.Title,
                Description = trivia.Description,
                Link = trivia.Link,
                CreatedAt = trivia.CreatedAt
            };
            var oldValues = JsonConvert.SerializeObject(oldTrivia);

            // Apply changes
            trivia.Title = NewTrivia.Title?.Trim();
            trivia.Description = NewTrivia.Description?.Trim();
            trivia.Link = string.IsNullOrWhiteSpace(NewTrivia.Link) ? null : NewTrivia.Link.Trim();

            await _db.SaveChangesAsync();

            // Capture new values
            var newValues = JsonConvert.SerializeObject(trivia);

            // Build change summary
            var changes = new List<string>();
            if (oldTrivia.Title != trivia.Title) changes.Add($"Title: '{oldTrivia.Title}' ? '{trivia.Title}'");
            if (oldTrivia.Description != trivia.Description) changes.Add($"Description: '{oldTrivia.Description}' ? '{trivia.Description}'");
            if (oldTrivia.Link != trivia.Link) changes.Add($"Link: '{oldTrivia.Link}' ? '{trivia.Link}'");
            var changeSummary = changes.Count > 0 ? string.Join(", ", changes) : "No changes";

            // Log to AdminHistory
            var historyEntry = new AdminHistory
            {
                Action = "Update",
                TableName = "EyeTrivia",
                RecordId = trivia.TriviaId,
                CreatedAt = DateTime.Now,
                Description = $"Updated Eye Trivia: {trivia.Title}<br>Changes:<br>{changeSummary.Replace(", ", "<br>")}",
                OldValues = oldValues,
                NewValues = newValues
            };
            _db.AdminHistories.Add(historyEntry);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Trivia updated successfully!";
            return RedirectToPage();
        }

        // ? DELETE TRIVIA
        public async Task<IActionResult> OnPostDeleteTriviaAsync(int id)
        {
            var trivia = await _db.EyeTrivia.FindAsync(id);
            if (trivia != null)
            {
                var oldValues = JsonConvert.SerializeObject(trivia);

                _db.EyeTrivia.Remove(trivia);
                await _db.SaveChangesAsync();

                var historyEntry = new AdminHistory
                {
                    Action = "Delete",
                    TableName = "EyeTrivia",
                    RecordId = trivia.TriviaId,
                    CreatedAt = DateTime.Now,
                    Description = $"Deleted Eye Trivia: {trivia.Title}",
                    OldValues = oldValues,
                    NewValues = null
                };
                _db.AdminHistories.Add(historyEntry);
                await _db.SaveChangesAsync();

                TempData["Message"] = "Trivia deleted successfully!";
            }

            return RedirectToPage();
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
