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

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]        
        public string Upload()
        {
            UploadedFile file = RetrieveFileFromRequest();
            string savePath = string.Empty;
            string virtualPath = SaveFile(file);

            return virtualPath;         
        }        

        private UploadedFile RetrieveFileFromRequest()
        {
            string filename = null;
            string fileType = null;
            byte[] fileContents = null;

            if (Request.Files.Count > 0)
            { //we are uploading the old way
                var file = Request.Files[0];
                fileContents = new byte[file.ContentLength];
                file.InputStream.Read(fileContents, 0, file.ContentLength);
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

        /// <summary>
        /// Saves the image
        /// </summary>
        /// <param name="logoUpload"></param>
        private string SaveFile(UploadedFile file)
        {
            System.IO.FileStream stream = null;
            var virtualPath = string.Empty;
            try
            {
                var physicalPath = Server.MapPath("~/Content");
                virtualPath = "/Content/";                
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file.Filename);
                fileName = fileName + DateTime.Now.Ticks + System.IO.Path.GetExtension(file.Filename);
                var path = System.IO.Path.Combine(physicalPath, fileName);
                virtualPath = virtualPath + fileName;
                stream = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                if (stream.CanWrite)
                {
                    stream.Write(file.Contents, 0, file.Contents.Length);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return virtualPath;
        }
    }
}
