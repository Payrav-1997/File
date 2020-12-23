using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File.Models;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace File.Controllers
{
    public class QRCoderController : Controller
    {
        private ApplicationContext _context;
        const string server = "https://localhost:44379/";
        public QRCoderController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return View(BitmapToBytes(qrCodeImage));
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public IActionResult GenerateFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateFile(int id)
        {
            string path = server+_context.Files.First(p => p.Id == id).Path;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(path, QRCodeGenerator.ECCLevel.Q);
            string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
            qrCodeData.SaveRawData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);

            QRCodeData qrCodeData1 = new QRCodeData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode qrCode = new QRCode(qrCodeData1);
            Bitmap qrCodeImage = qrCode.GetGraphic(40);
            return View(BitmapToBytes(qrCodeImage));
        }
  

        public IActionResult ViewFile()
        {
            List<KeyValuePair<string, Byte[]>> fileData=new List<KeyValuePair<string, byte[]>>();
            KeyValuePair<string, Byte[]> data;

            string[] files = Directory.GetFiles("wwwroot/qrr");
            foreach (string file in files)
            {
                QRCodeData qrCodeData = new QRCodeData(file, QRCodeData.Compression.Uncompressed);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(40);

                Byte[] byteData = BitmapToBytes(qrCodeImage);
                data = new KeyValuePair<string, Byte[]>(Path.GetFileName(file), byteData);
                fileData.Add(data);
            }

            return View(fileData);
        }
        //[HttpGet]
        //public IActionResult Delete()
        //{
           
        //    List<KeyValuePair<string, Byte[]>> fileData = new List<KeyValuePair<string, byte[]>>();
        //    KeyValuePair<string, Byte[]> data;

            
        //    string[] files = Directory.GetFiles("wwwroot/qrr");

        //    if (files != null)
        //    {
        //        foreach (string file in files)
        //        {
        //            QRCodeData qrCodeData = new QRCodeData(file, QRCodeData.Compression.Uncompressed);
        //            QRCode qrCode = new QRCode(qrCodeData);
        //            Bitmap qrCodeImage = qrCode.GetGraphic(40);

        //            Byte[] byteData = BitmapToBytes(qrCodeImage);
        //            data = new KeyValuePair<string, Byte[]>(Path.GetFileName(file), byteData);
        //            fileData.Remove(data);
        //        }
        //        return View(fileData);

        //    }
        //}
    }
}