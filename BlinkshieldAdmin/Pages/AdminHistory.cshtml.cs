using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BlinkshieldAdmin.Pages
{
    public class AdminHistoryModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public List<AdminHistory> History { get; set; } = new List<AdminHistory>();

        [BindProperty(SupportsGet = true)]
        public string? SelectedAction { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCategory { get; set; }

        public List<SelectListItem> ActionOptions { get; } = new()
        {
            new SelectListItem { Value = "Update", Text = "Update" },
            new SelectListItem { Value = "Create", Text = "Create" },
            new SelectListItem { Value = "Delete", Text = "Delete" }
        };

        public List<SelectListItem> CategoryOptions { get; } = new()
        {
            new SelectListItem { Value = "Achievement", Text = "Achievement" },
            new SelectListItem { Value = "Wardrobe", Text = "Wardrobe" },
            new SelectListItem { Value = "EyeTrivia", Text = "Eye Trivia" },
            new SelectListItem { Value = "EyeTips", Text = "Eye Tips" },
            new SelectListItem { Value = "EyeIssues", Text = "Eye Issues" }
        };

        public AdminHistoryModel(BlinkDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            var query = _context.AdminHistories.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SelectedAction))
            {
                query = query.Where(h => h.Action.Contains(SelectedAction));
            }

            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                query = query.Where(h => h.TableName.Contains(SelectedCategory) || h.Description.Contains(SelectedCategory));
            }

            // Sort newest first
            History = query
                .OrderByDescending(h => h.CreatedAt)
                .ToList();
        }
    }
}
