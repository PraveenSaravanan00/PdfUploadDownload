using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PdfUploadAndDownload.Models;

namespace PdfUploadAndDownload.Data
{
    public class PdfUploadAndDownloadContext : DbContext
    {
        public PdfUploadAndDownloadContext (DbContextOptions<PdfUploadAndDownloadContext> options)
            : base(options)
        {
        }

        public DbSet<PdfUploadAndDownload.Models.PdfDocument> PdfDocument { get; set; } = default!;
    }
}
