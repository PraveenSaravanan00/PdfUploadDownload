

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using PdfUploadAndDownload.Data;
using PdfUploadAndDownload.Models;

public class HomeController : Controller
{
    private readonly PdfUploadAndDownloadContext _dbContext;

    public HomeController(PdfUploadAndDownloadContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var pdfDocument = new PdfDocument
                {
                    FileName = file.FileName,
                    Content = memoryStream.ToArray()
                };
                _dbContext.PdfDocument.Add(pdfDocument);
                await _dbContext.SaveChangesAsync();
            }
        }

        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Download(int id)
    {
        var pdfDocument = await _dbContext.PdfDocument.FirstOrDefaultAsync(doc => doc.Id == id);
        if (pdfDocument == null)
        {
            return NotFound();
        }

        return File(pdfDocument.Content, "application/pdf", pdfDocument.FileName);
    }
}