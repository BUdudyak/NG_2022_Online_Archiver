using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Online_Archiver.Models;
using Online_Archiver.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Text;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.Administration;

namespace Online_Archiver.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDataService _dataService;

        public HomeController(ILogger<HomeController> logger, IDataService dataService)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_dataService.GetFiles());
        }

        public IActionResult Archiver()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
            List<string> files = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);

            files.AddRange(dir.GetFiles().Select(f => f.Name));

            return View(
                    new ArchivedFilesModel
                    {
                        FileName = files
                    });
        }

        [HttpPost]
        //[StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public ActionResult Archivation(ArchivedFilesModel model)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var filenames = model.Files.Where(m => m.Selected == true).Select(f => f.Name).ToList();

            string filename = model.ArchiveName + ".zip";
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(3);

            foreach (string files in filenames)
            {
                FileInfo fi = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", files));

                if (!fi.Exists)
                    continue;

                string entryName = ZipEntry.CleanName(fi.Name);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = System.IO.File.OpenRead(fi.FullName))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            zipStream.IsStreamOwner = false;
            zipStream.Close();

            outputMemStream.Position = 0;

            string file_type = "application/zip";
            return File(outputMemStream, file_type, filename);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 10_737_418_240, ValueLengthLimit = Int32.MaxValue)]
        public IActionResult Upload(IFormFile[] files)
        {
            /*using (ServerManager serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration("Default Web Site");
                ConfigurationSection requestFilteringSection = config.GetSection("system.webServer/security/requestFiltering");
                ConfigurationElement requestLimitsElement = requestFilteringSection.GetChildElement("requestLimits");
                ConfigurationElementCollection headerLimitsCollection = requestLimitsElement.GetCollection("headerLimits");

                ConfigurationElement addElement = headerLimitsCollection.CreateElement("add");
                addElement["header"] = @"Content-type";
                addElement["sizeLimit"] = 100;
                headerLimitsCollection.Add(addElement);

                serverManager.CommitChanges();
            }*/
            foreach (var file in files)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var localFile = System.IO.File.OpenWrite(filePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
            }
            ViewBag.Message = "Files are successfully uploaded";

            return View("Index", _dataService.GetFiles());
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /*
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename is not availble");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }*/
    }
}
