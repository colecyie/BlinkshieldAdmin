using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlinkshieldAdmin.Pages
{
    public class EditWardrobeItemModel : PageModel
    {
        private readonly BlinkDbContext _context;

        [BindProperty]
        public WardrobeItem Item { get; set; } = new WardrobeItem();

        [BindProperty]
        public IFormFile? ImageFile { get; set; } // uploaded file

        public EditWardrobeItemModel(BlinkDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existingItem = await _context.WardrobeItems.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            Item = existingItem;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingItem = await _context.WardrobeItems.FindAsync(Item.ItemId);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Keep a lightweight copy for old values (exclude Image)
            var oldValuesObj = new
            {
                existingItem.ItemId,
                existingItem.Name,
                existingItem.Type,
                existingItem.Price,
                existingItem.Description
            };
            var oldValues = Newtonsoft.Json.JsonConvert.SerializeObject(oldValuesObj);

            var changes = new List<string>();

            if (existingItem.Name != Item.Name)
                changes.Add($"Name: '{existingItem.Name}' ? '{Item.Name}'");

            if (existingItem.Type != Item.Type)
                changes.Add($"Type: '{existingItem.Type}' ? '{Item.Type}'");

            if (existingItem.Price != Item.Price)
                changes.Add($"Price: {existingItem.Price} ? {Item.Price}");

            if (existingItem.Description != Item.Description)
                changes.Add($"Description: '{existingItem.Description}' ? '{Item.Description}'");

            // Update fields
            existingItem.Name = Item.Name;
            existingItem.Type = Item.Type;
            existingItem.Price = Item.Price;
            existingItem.Description = Item.Description;

            // Handle image upload (replace if new image is uploaded)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    existingItem.Image = ms.ToArray();
                }
                changes.Add("Image updated");
            }

            try
            {
                await _context.SaveChangesAsync();

                // Lightweight new values (exclude Image)
                var newValuesObj = new
                {
                    existingItem.ItemId,
                    existingItem.Name,
                    existingItem.Type,
                    existingItem.Price,
                    existingItem.Description
                };
                var newValues = Newtonsoft.Json.JsonConvert.SerializeObject(newValuesObj);

                // Build change summary with <br> for line breaks
                var changeSummary = changes.Count > 0
                    ? string.Join("<br>", changes)
                    : "No changes made";

                var history = new AdminHistory
                {
                    Action = "Update",
                    TableName = "Wardrobe",
                    RecordId = existingItem.ItemId,
                    Description = $"Updated wardrobe item ({existingItem.Name})<br>Changes:<br>{changeSummary}",
                    OldValues = oldValues,
                    NewValues = newValues,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AdminHistories.Add(history);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Item updated successfully!";
                return RedirectToPage("/AdminWardrobe");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                return Page();
            }
        }
    }
}

