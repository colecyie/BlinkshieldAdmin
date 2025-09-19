using BlinkshieldAdmin.Data;
using BlinkshieldAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlinkshieldAdmin.Pages
{
    public class AddWardrobeItemModel : PageModel
    {
        private readonly BlinkDbContext _context;

        [BindProperty]
        public WardrobeItem Item { get; set; } = new WardrobeItem();

        [BindProperty]
        public IFormFile? ImageFile { get; set; } // uploaded file

        public AddWardrobeItemModel(BlinkDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Handle image upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    Item.Image = ms.ToArray();
                }
            }

            try
            {
                // Save new item
                _context.WardrobeItems.Add(Item);
                await _context.SaveChangesAsync();

                // Record initial values for undo tracking
                var newValuesObj = new
                {
                    Item.ItemId,
                    Item.Name,
                    Item.Type,
                    Item.Price,
                    Item.Description
                };
                var newValues = Newtonsoft.Json.JsonConvert.SerializeObject(newValuesObj);

                var history = new AdminHistory
                {
                    Action = "Create",
                    TableName = "Wardrobe",
                    RecordId = Item.ItemId,
                    Description = $"Created wardrobe item ({Item.Name})",
                    OldValues = null, // nothing before create
                    NewValues = newValues,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AdminHistories.Add(history);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Item added successfully! You can undo this action.";
                return RedirectToPage("/AdminWardrobe");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving item: " + ex.Message);
                return Page();
            }
        }

        // ✅ Undo Action (removes the created item)
        public async Task<IActionResult> OnPostUndoAsync(int itemId)
        {
            var item = await _context.WardrobeItems.FindAsync(itemId);
            if (item != null)
            {
                _context.WardrobeItems.Remove(item);
                await _context.SaveChangesAsync();

                var history = new AdminHistory
                {
                    Action = "Undo Create",
                    TableName = "Wardrobe",
                    RecordId = item.ItemId,
                    Description = $"Undo create: deleted wardrobe item ({item.Name})",
                    OldValues = null,
                    NewValues = null,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AdminHistories.Add(history);
                await _context.SaveChangesAsync();

                TempData["UndoSuccess"] = "Item creation has been undone.";
            }

            return RedirectToPage("/AdminWardrobe");
        }
    }
}
