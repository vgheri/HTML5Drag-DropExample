using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DesktopDragAndDrop.Models;

namespace DesktopDragAndDrop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        [HttpPost]        
        public ActionResult Upload()
        {
            UploadedFile file = RetrieveFileFromRequest();

            return RedirectToAction("Index");
                      
        }

        public ActionResult About()
        {
            return View();
        }

        private UploadedFile RetrieveFileFromRequest()
        {
            string filename = null;
            string fileType = null;
            byte[] fileContents = null;

            if (Request.Files.Count > 0)
            { //they're uploading the old way
                var file = Request.Files[0];
                fileContents = new byte[file.ContentLength];
                fileType = file.ContentType;
                filename = file.FileName;
            }
            else if (Request.ContentLength > 0)
            {
                // Using FileAPI the content is in Request.InputStream!!!!
                fileContents = new byte[Request.ContentLength];
                Request.InputStream.Read(fileContents, 0, Request.ContentLength);
                filename = Request.Headers["X-File-Name"];
                fileType = Request.Headers["X-File-Type"];
            }

            return new UploadedFile()
            {
                Filename = filename,
                ContentType = fileType,
                FileSize = fileContents != null ? fileContents.Length : 0,
                Contents = fileContents
            };
        }
    }
}
