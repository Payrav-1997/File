using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using File.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Google.Apis.Analytics.v3.Data;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authentication;

namespace File.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext _context;
        IWebHostEnvironment _appEnvironment;
        const string server = "https://localhost:44379/";

        public HomeController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View(_context.Files.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {

            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {

                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                _context.Files.Add(file);
                _context.SaveChanges();
                int id = _context.Files.First(p => p.Name == file.Name && p.Path == file.Path).Id;
                path = server + _context.Files.First(p => p.Id == id).Path;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(path, QRCodeGenerator.ECCLevel.Q);
                string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
                qrCodeData.SaveRawData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);

                QRCodeData qrCodeData1 = new QRCodeData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);
                QRCode qrCode = new QRCode(qrCodeData1);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {


            var file = _context.Files.Where(p => p.Id == Id).FirstOrDefault();
            _context.Remove(file);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

    }
}
