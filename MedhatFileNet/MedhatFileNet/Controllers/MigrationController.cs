using MedhatFileNet.Models;
using MedhatFileNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedhatFileNet.Controllers
{
    public class MigrationController : Controller
    {
        private readonly FileNetService _fileNetService;
        private readonly LaserficheService _laserficheService;

        public MigrationController(FileNetService fileNetService, LaserficheService laserficheService)
        {
            _fileNetService = fileNetService;
            _laserficheService = laserficheService;
        }

        public async Task<IActionResult> Index()
        {
            var fileNetDocs = await _fileNetService.GetDocumentsAsync();
            var results = new List<MigrationResult>();

            foreach (var doc in fileNetDocs)
            {
                var lfDoc = new LaserficheDocument
                {
                    FileName = doc.FileName,
                    FileContent = doc.FileContent,
                    TemplateName = "FileNet_Import",
                    ParentEntryId = "1",
                    Metadata = doc.Metadata
                };

                var success = await _laserficheService.UploadDocumentAsync(lfDoc);
                results.Add(new MigrationResult { FileName = doc.FileName, Success = success });
            }

            return View("Result", results);
        }
    }
}
