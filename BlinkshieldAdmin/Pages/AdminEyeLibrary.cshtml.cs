using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlinkshieldAdmin.Pages.Admin
{
    public class AdminEyeLibraryModel : PageModel
    {
        private readonly BlinkDbContext _db;

        public AdminEyeLibraryModel(BlinkDbContext dbContext)
        {
            _db = dbContext;
        }

        // Display lists
        public List<EyeTrivia> TriviaList { get; set; } = new();
        public List<EyeCareTip> TipsList { get; set; } = new();
        public List<EyeIssue> IssuesList { get; set; } = new();

        // Form input models
        [BindProperty]
        public TriviaInput NewTrivia { get; set; } = new();

        [BindProperty]
        public TipInput NewTip { get; set; } = new();

        [BindProperty]
        public IssueInput NewIssue { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            TriviaList = await _db.EyeTrivia
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            TipsList = await _db.EyeCareTip
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            IssuesList = await _db.EyeIssues
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        // Add Trivia
        public async Task<IActionResult> OnPostAddTriviaAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            try
            {
                _db.EyeTrivia.Add(new EyeTrivia
                {
                    Title = NewTrivia.Title,
                    Description = NewTrivia.Description,
                    Link = NewTrivia.Link,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Trivia added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding trivia: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Add Tip
        public async Task<IActionResult> OnPostAddTipAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            try
            {
                _db.EyeCareTip.Add(new EyeCareTip
                {
                    Title = NewTip.Title,
                    Content = NewTip.Content,
                    Link = NewTip.Link,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tip added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding tip: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Add Issue
        public async Task<IActionResult> OnPostAddIssueAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            try
            {
                _db.EyeIssues.Add(new EyeIssue
                {
                    Title = NewIssue.Title,
                    Description = NewIssue.Description,
                    SuggestedAction = NewIssue.SuggestedAction,
                    Link = NewIssue.Link,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Issue added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding issue: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Input Models
        public class TriviaInput
        {
            [Required, MaxLength(255)]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [MaxLength(500), Url]
            public string? Link { get; set; }
        }

        public class TipInput
        {
            [Required, MaxLength(150)]
            public string Title { get; set; }

            [Required]
            public string Content { get; set; }

            [Url]
            public string? Link { get; set; }
        }

        public class IssueInput
        {
            [Required, MaxLength(100)]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            public string? SuggestedAction { get; set; }

            [MaxLength(255), Url]
            public string? Link { get; set; }
        }
    }
}