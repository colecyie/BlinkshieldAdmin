using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BlinkshieldAdmin.Pages
{
    public class AchievementsModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public AchievementsModel(BlinkDbContext context)
        {
            _context = context;
        }

        public List<Achievement> AchievementList { get; set; } = new();

        [BindProperty]
        public Achievement Achievement { get; set; } = new Achievement();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Achievements.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(a =>
                    a.Title.Contains(SearchTerm) ||
                    a.Description.Contains(SearchTerm));
            }

            AchievementList = await query
                .OrderByDescending(a => a.AchievementId)
                .ToListAsync();
        }

        // ? ADD
        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newAchievement = new Achievement
            {
                Title = Achievement.Title,
                Description = Achievement.Description,
                Points = Achievement.Points,
                RequirementType = Achievement.RequirementType,   // ?? include this
                RequirementValue = Achievement.RequirementValue, // ?? and this
                CreatedAt = DateTime.Now
            };

            _context.Achievements.Add(newAchievement);
            await _context.SaveChangesAsync();

            var historyEntry = new AdminHistory
            {
                Action = "Create Achievement",
                TableName = "Achievement",
                RecordId = newAchievement.AchievementId,
                CreatedAt = DateTime.Now,
                Description = $"Added Achievement: {newAchievement.Title}",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(newAchievement)
            };

            _context.AdminHistories.Add(historyEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        // ? EDIT
        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);

            if (achievement == null)
            {
                return NotFound();
            }

            // Clone old values
            var oldAchievement = new Achievement
            {
                AchievementId = achievement.AchievementId,
                Title = achievement.Title,
                Description = achievement.Description,
                Points = achievement.Points,
                RequirementType = achievement.RequirementType,
                RequirementValue = achievement.RequirementValue,
                CreatedAt = achievement.CreatedAt
            };

            var changes = new List<string>();

            if (achievement.Title != Achievement.Title)
            {
                changes.Add($"Title: '{achievement.Title}' ? '{Achievement.Title}'");
                achievement.Title = Achievement.Title;
            }

            if (achievement.Description != Achievement.Description)
            {
                changes.Add($"Description: '{achievement.Description}' ? '{Achievement.Description}'");
                achievement.Description = Achievement.Description;
            }

            if (achievement.Points != Achievement.Points)
            {
                changes.Add($"Points: {achievement.Points} ? {Achievement.Points}");
                achievement.Points = Achievement.Points;
            }

            if (achievement.RequirementType != Achievement.RequirementType)
            {
                changes.Add($"RequirementType: '{achievement.RequirementType}' ? '{Achievement.RequirementType}'");
                achievement.RequirementType = Achievement.RequirementType;
            }

            if (achievement.RequirementValue != Achievement.RequirementValue)
            {
                changes.Add($"RequirementValue: {achievement.RequirementValue} ? {Achievement.RequirementValue}");
                achievement.RequirementValue = Achievement.RequirementValue;
            }

            await _context.SaveChangesAsync();

            var changeSummary = changes.Count > 0
                ? string.Join(", ", changes).Replace(", ", "<br>")
                : "No changes made";

            var historyEntry = new AdminHistory
            {
                Action = "Update Achievement",
                TableName = "Achievement",
                RecordId = achievement.AchievementId,
                CreatedAt = DateTime.Now,
                Description = $"Updated Achievement: {achievement.Title}<br>Changes:<br>{changeSummary}",
                OldValues = JsonConvert.SerializeObject(oldAchievement),
                NewValues = JsonConvert.SerializeObject(achievement)
            };

            _context.AdminHistories.Add(historyEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        // ? DELETE
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement != null)
            {
                var oldValues = JsonConvert.SerializeObject(achievement);

                _context.Achievements.Remove(achievement);
                await _context.SaveChangesAsync();

                var historyEntry = new AdminHistory
                {
                    Action = "Delete Achievement",
                    TableName = "Achievement",
                    RecordId = achievement.AchievementId,
                    CreatedAt = DateTime.Now,
                    Description = $"Deleted Achievement: {achievement.Title}",
                    OldValues = oldValues,
                    NewValues = null
                };

                _context.AdminHistories.Add(historyEntry);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
