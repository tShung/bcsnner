using System;
using System.IO;
using System.Web.Mvc;
using WPL.Models;

namespace WPL.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        IProduct _db = new Repository();
        [Authorize]
        public ActionResult Index()
        {
            return View(new ScannModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ScannModel model, string scannedImage)
        {
            string fileName = "captured.png";
            string fileNameWitPath = Path.Combine(Server.MapPath("~/images"), fileName);
            FileInfo fi = new FileInfo(fileNameWitPath);
            if (!Directory.Exists(fi.DirectoryName))
                Directory.CreateDirectory(fi.DirectoryName);

            using (FileStream fs = new FileStream(fileNameWitPath, FileMode.OpenOrCreate))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(scannedImage);
                    bw.Write(data);
                    bw.Close();
                }
                fs.Close();
            }

            BarCodeDecoder decoder = new BarCodeDecoder();
            var content = decoder.Read(scannedImage);
            return View(SearchProduct(content));
        }

        private ScannModel SearchProduct(string content)
        {
            return _db.Search(content);
        }

        [Authorize]
        public ActionResult Search()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Search(ScannModel model)
        {
            var result = SearchProduct(model.Upc);
            return View(result);
        }


        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
