using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfUploadAndDownload.Data;
using PdfUploadAndDownload.Models;
using PagedList;

namespace PdfUploadAndDownload.Controllers
{
    public class PdfDocumentsController : Controller
    {
        private readonly PdfUploadAndDownloadContext _context;

        public PdfDocumentsController(PdfUploadAndDownloadContext context)
        {
            _context = context;
        }
        public IActionResult Index(int pageIndex=1,int pageSize=5)
        {
            var pageCount = _context.PdfDocument.Count();
            int totalPage = (int)Math.Ceiling(pageCount / (double)pageSize);
            var currentPageItems = pageCount.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalPages = totalPage;
            return View(currentPageItems);

        }
        //public async Task<IActionResult> Index()
        //{
        //    int pageSize = 5;
        //    int pageIndex = 1;

        //    return View(await _context.PdfDocument.ToListAsync());
        //}
        //public IActionResult Index(int page = 1, int pageSize = 10)
        //{
        //    // Assuming you have some data source, like a list of items
        //    var items = YourDataRepository.GetAllItems();

        //    // Calculate total pages
        //    int totalPages = (int)Math.Ceiling(items.Count / (double)pageSize);

        //    // Select the items for the current page
        //    var currentPageItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    // Pass the current page, total pages, and items to the view
        //    ViewBag.CurrentPage = page;
        //    ViewBag.TotalPages = totalPages;
        //    return View(currentPageItems);
        //}
//        @model List<YourItemModel>

//@foreach (var item in Model)
//        {
//            // Display your item
//        }

//<div>
//    @if(ViewBag.CurrentPage > 1)
//        {
//        < a href = "/YourController/Index?page=1" > First </ a >
//        < a href = "/YourController/Index?page=@(ViewBag.CurrentPage - 1)" > Previous </ a >
//    }

//        @if(ViewBag.CurrentPage<ViewBag.TotalPages)
//        {
//        < a href = "/YourController/Index?page=@(ViewBag.CurrentPage + 1)" > Next </ a >
//        < a href = "/YourController/Index?page=@(ViewBag.TotalPages)" > Last </ a >
//        }
//</div>

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
                        //FileName = file.FileName,
                        FileName = $"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}-{file.FileName}",
                        Content = memoryStream.ToArray()
                    };
                    _context.PdfDocument.Add(pdfDocument);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index)); 

            //try
            //{
            //    var supportedTypes = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
            //    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            //    if (!supportedTypes.Contains(fileExt))
            //    {
            //        ErrorMessage = "File Extension Is InValid - Only Upload WORD/PDF/EXCEL/TXT File";
            //        return ErrorMessage;
            //    }
            //    else if (file.ContentLength > (filesize * 1024))
            //    {
            //        ErrorMessage = "File size Should Be UpTo " + filesize + "KB";
            //        return ErrorMessage;
            //    }
            //    else
            //    {
            //        ErrorMessage = "File Is Successfully Uploaded";
            //        return ErrorMessage;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ErrorMessage = "Upload Container Should Not Be Empty or Contact Admin";
            //    return ErrorMessage;
            //}
        }
        public async Task<IActionResult> Download(int id)
        {
            var pdfDocument = await _context.PdfDocument.FirstOrDefaultAsync(doc => doc.Id == id);
            if (pdfDocument == null)
            {
                return NotFound();
            }

            return File(pdfDocument.Content, "application/pdf", pdfDocument.FileName);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pdfDocument = await _context.PdfDocument.FindAsync(id);
            if (pdfDocument == null)
            {
                return NotFound();
            }
            if (pdfDocument != null)
                {
                    _context.PdfDocument.Remove(pdfDocument);
                }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool PdfDocumentExists(int id)
        {
            return _context.PdfDocument.Any(e => e.Id == id);
        }
    }
}
