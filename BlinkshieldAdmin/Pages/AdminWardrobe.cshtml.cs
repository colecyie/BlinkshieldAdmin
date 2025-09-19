using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlinkshieldAdmin.Pages
{
    public class AdminWardrobeModel : PageModel
    {
        private readonly BlinkDbContext _context;

        public AdminWardrobeModel(BlinkDbContext context)
        {
            _context = context;
        }

        // List of all wardrobe items
        public List<WardrobeItem> Items { get; set; } = new();

        // For search/filter (matches search bar input name)
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var itemsQuery = _context.WardrobeItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string lowerSearch = SearchTerm.ToLower();

                itemsQuery = itemsQuery.Where(i =>
                    (i.Name != null && i.Name.ToLower().Contains(lowerSearch)) ||
                    (i.Description != null && i.Description.ToLower().Contains(lowerSearch)) ||
                    (i.Type != null && i.Type.ToLower().Contains(lowerSearch)) ||
                    i.Price.ToString().ToLower().Contains(lowerSearch)
                );
            }

            Items = await itemsQuery
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var wardrobe = await _context.WardrobeItems.FindAsync(id);
            if (wardrobe != null)
            {
                // Store only the safe properties (exclude Image byte[])
                var oldValues = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    wardrobe.ItemId,
                    wardrobe.Name,
                    wardrobe.Description,
                    wardrobe.Type,
                    wardrobe.Price,
                    wardrobe.CreatedAt
                });

                _context.WardrobeItems.Remove(wardrobe);
                await _context.SaveChangesAsync();

                var historyEntry = new AdminHistory
                {
                    Action = "Delete",
                    TableName = "Wardrobe",
                    RecordId = wardrobe.ItemId,
                    CreatedAt = DateTime.Now,
                    Description = $"Deleted Wardrobe Item: {wardrobe.Name}",
                    OldValues = oldValues,
                    NewValues = null
                };

                _context.AdminHistories.Add(historyEntry);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Wardrobe item deleted successfully!";
            }
            else
            {
                TempData["Success"] = "Item not found!";
            }

            return RedirectToPage();
        }
    }
}
