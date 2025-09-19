using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlinkshieldAdmin.Pages.AdminEyeLibrary
{
    public class EyeIssuesModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public EyeIssuesModel(BlinkDbContext context)
        {
            _context = context;
        }

        public List<EyeIssue> IssuesList { get; set; } = new();

        [BindProperty]
        public EyeIssue NewIssue { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.EyeIssues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(i =>
                    i.Title.Contains(SearchTerm) ||
                    i.Description.Contains(SearchTerm) ||
                    i.SuggestedAction.Contains(SearchTerm));
            }

            IssuesList = await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddIssueAsync()
        {
            if (!ModelState.IsValid)
            {
                IssuesList = await _context.EyeIssues.ToListAsync(); // Keep data on error
                return Page();
            }

            NewIssue.CreatedAt = DateTime.Now;
            _context.EyeIssues.Add(NewIssue);
            await _context.SaveChangesAsync();

            // ✅ Add AdminHistory log for CREATE
            var history = new AdminHistory
            {
                Action = "Create",
                TableName = "EyeIssues",
                RecordId = NewIssue.IssueId,
                CreatedAt = DateTime.Now,
                Description = $"Added new eye issue: {NewIssue.Title}",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(new
                {
                    NewIssue.Title,
                    NewIssue.Description,
                    NewIssue.SuggestedAction,
                    NewIssue.Link
                })
            };

            _context.AdminHistories.Add(history);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Eye issue added successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditIssueAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                IssuesList = await _context.EyeIssues
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
                return Page();
            }

            var issue = await _context.EyeIssues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            // Clone old values
            var oldIssue = new EyeIssue
            {
                IssueId = issue.IssueId,
                Title = issue.Title,
                Description = issue.Description,
                SuggestedAction = issue.SuggestedAction,
                Link = issue.Link,
                CreatedAt = issue.CreatedAt
            };

            var changes = new List<string>();

            if (issue.Title != NewIssue.Title?.Trim())
                changes.Add($"Title: '{issue.Title}' → '{NewIssue.Title?.Trim()}'");

            if (issue.Description != NewIssue.Description?.Trim())
                changes.Add($"Description: '{issue.Description}' → '{NewIssue.Description?.Trim()}'");

            if (issue.SuggestedAction != NewIssue.SuggestedAction?.Trim())
                changes.Add($"SuggestedAction: '{issue.SuggestedAction}' → '{NewIssue.SuggestedAction?.Trim()}'");

            var newLink = string.IsNullOrWhiteSpace(NewIssue.Link) ? null : NewIssue.Link.Trim();
            if (issue.Link != newLink)
                changes.Add($"Link: '{issue.Link}' → '{newLink}'");

            // Apply updates
            issue.Title = NewIssue.Title?.Trim();
            issue.Description = NewIssue.Description?.Trim();
            issue.SuggestedAction = string.IsNullOrWhiteSpace(NewIssue.SuggestedAction) ? null : NewIssue.SuggestedAction.Trim();
            issue.Link = newLink;

            await _context.SaveChangesAsync();

            // Build change summary
            var changeSummary = changes.Count > 0
                ? string.Join("<br>", changes)
                : "No changes made";

            var history = new AdminHistory
            {
                Action = "Update",
                TableName = "EyeIssues",
                RecordId = issue.IssueId,
                CreatedAt = DateTime.Now,
                Description = $"Updated eye issue: {issue.Title}<br>Changes:<br>{changeSummary}",
                OldValues = JsonConvert.SerializeObject(oldIssue),
                NewValues = JsonConvert.SerializeObject(issue)
            };

            _context.AdminHistories.Add(history);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Eye issue updated successfully!";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteIssueAsync(int id)
        {
            var issue = await _context.EyeIssues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            // Save old values for history
            var oldValues = JsonConvert.SerializeObject(new
            {
                issue.Title,
                issue.Description,
                issue.SuggestedAction,
                issue.Link
            });

            _context.EyeIssues.Remove(issue);
            await _context.SaveChangesAsync();

            // ✅ Add AdminHistory log for DELETE
            var history = new AdminHistory
            {
                Action = "Delete",
                TableName = "EyeIssues",
                RecordId = issue.IssueId,
                CreatedAt = DateTime.Now,
                Description = $"Deleted eye issue: {issue.Title}",
                OldValues = oldValues,
                NewValues = null
            };

            _context.AdminHistories.Add(history);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Eye issue deleted successfully!";
            return RedirectToPage();
        }
    }
}
