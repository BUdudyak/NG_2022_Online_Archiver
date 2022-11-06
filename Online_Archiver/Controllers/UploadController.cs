using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Archiver.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;


namespace Online_Archiver.Controllers
{
    public class UploadController : Controller
    {
        private readonly IDataService _dataService;

        public UploadController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            return View(_dataService.GetFiles());
        }

        [HttpPost]
        public IActionResult Index(IFormFile[] files)
        {
            // Iterate each files
            foreach (var file in files)
            {
                // Get the file name from the browser
                var fileName = System.IO.Path.GetFileName(file.FileName);

                // Get file path to be uploaded
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);

                // Check If file with same name exists and delete it
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Create a new local file and copy contents of uploaded file
                using (var localFile = System.IO.File.OpenWrite(filePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
            }
            ViewBag.Message = "Files are successfully uploaded";

            // Get files from the server
            return View(_dataService.GetFiles());
        }
    }
}
