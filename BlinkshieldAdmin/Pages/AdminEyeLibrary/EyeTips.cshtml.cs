using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlinkshieldAdmin.Pages.AdminEyeLibrary
{
    public class EyeTipsModel : PageModel
    {
        private readonly BlinkDbContext _db;

        public EyeTipsModel(BlinkDbContext context)
        {
            _db = context;
        }

        public List<EyeCareTip> TipsList { get; set; } = new();

        [BindProperty]
        public TipInput NewTip { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _db.EyeCareTip.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(SearchTerm) ||
                    t.Content.Contains(SearchTerm) ||
                    t.Category.Contains(SearchTerm));
            }

            TipsList = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        // ✅ ADD NEW TIP
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newTip = new EyeCareTip
            {
                Title = NewTip.Title.Trim(),
                Category = NewTip.Category,
                Content = NewTip.Content.Trim(),
                Link = string.IsNullOrWhiteSpace(NewTip.Link) ? null : NewTip.Link.Trim(),
                CreatedAt = DateTime.Now
            };

            _db.EyeCareTip.Add(newTip);
            await _db.SaveChangesAsync();

            // Log Admin History
            var historyEntry = new AdminHistory
            {
                Action = "Create",
                TableName = "EyeTips",
                RecordId = newTip.TipId,
                CreatedAt = DateTime.Now,
                Description = $"Added Eye Care Tip: {newTip.Title}",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(newTip)
            };

            _db.AdminHistories.Add(historyEntry);
            await _db.SaveChangesAsync();

            TempData["Message"] = "New Eye Care Tip added successfully!";
            return RedirectToPage();
        }

        // ✅ EDIT TIP
        public async Task<IActionResult> OnPostEditTipAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var tip = await _db.EyeCareTip.FindAsync(id);
            if (tip == null)
            {
                return NotFound();
            }

            // Clone old values for comparison
            var oldTip = new EyeCareTip
            {
                TipId = tip.TipId,
                Title = tip.Title,
                Category = tip.Category,
                Content = tip.Content,
                Link = tip.Link,
                CreatedAt = tip.CreatedAt
            };

            var changes = new List<string>();

            if (tip.Title != NewTip.Title?.Trim())
                changes.Add($"Title: '{tip.Title}' → '{NewTip.Title?.Trim()}'");

            if (tip.Category != NewTip.Category)
                changes.Add($"Category: '{tip.Category}' → '{NewTip.Category}'");

            if (tip.Content != NewTip.Content?.Trim())
                changes.Add($"Content: '{tip.Content}' → '{NewTip.Content?.Trim()}'");

            var newLink = string.IsNullOrWhiteSpace(NewTip.Link) ? null : NewTip.Link.Trim();
            if (tip.Link != newLink)
                changes.Add($"Link: '{tip.Link}' → '{newLink}'");

            // Apply updates
            tip.Title = NewTip.Title?.Trim();
            tip.Category = NewTip.Category;
            tip.Content = NewTip.Content?.Trim();
            tip.Link = newLink;

            await _db.SaveChangesAsync();

            var changeSummary = changes.Count > 0
                ? string.Join(", ", changes).Replace(", ", "<br>")
                : "No changes made";

            var historyEntry = new AdminHistory
            {
                Action = "Update",
                TableName = "EyeTips",
                RecordId = tip.TipId,
                CreatedAt = DateTime.Now,
                Description = $"Updated Eye Care Tip: {tip.Title}<br>Changes:<br>{changeSummary}",
                OldValues = JsonConvert.SerializeObject(oldTip),
                NewValues = JsonConvert.SerializeObject(tip)
            };

            _db.AdminHistories.Add(historyEntry);
            await _db.SaveChangesAsync();

            TempData["Message"] = "Eye Care Tip updated successfully!";
            return RedirectToPage();
        }

        // ✅ DELETE TIP
        public async Task<IActionResult> OnPostDeleteTipAsync(int id)
        {
            var tip = await _db.EyeCareTip.FindAsync(id);
            if (tip != null)
            {
                var oldValues = JsonConvert.SerializeObject(tip);

                _db.EyeCareTip.Remove(tip);
                await _db.SaveChangesAsync();

                var historyEntry = new AdminHistory
                {
                    Action = "Delete",
                    TableName = "EyeTips",
                    RecordId = tip.TipId,
                    CreatedAt = DateTime.Now,
                    Description = $"Deleted Eye Care Tip: {tip.Title}",
                    OldValues = oldValues,
                    NewValues = null
                };

                _db.AdminHistories.Add(historyEntry);
                await _db.SaveChangesAsync();

                TempData["Message"] = "Eye Care Tip deleted successfully!";
            }

            return RedirectToPage();
        }

        public class TipInput
        {
            [Required, MaxLength(255)]
            public string Title { get; set; }

            [Required]
            public string Category { get; set; }

            [Required]
            public string Content { get; set; }

            [MaxLength(500), Url]
            public string? Link { get; set; }
        }
    }
}
