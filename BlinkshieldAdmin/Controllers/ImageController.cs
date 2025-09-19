using Microsoft.AspNetCore.Mvc;
using System.IO;

[Route("uploads")]
public class UploadsController : Controller
{
    [HttpGet("{filename}")]
    public IActionResult GetFile(string filename)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", filename);
        if (!System.IO.File.Exists(path))
            return NotFound();

        var fileBytes = System.IO.File.ReadAllBytes(path);
        return File(fileBytes, "image/jpeg"); // or use proper mime type logic
    }
}
