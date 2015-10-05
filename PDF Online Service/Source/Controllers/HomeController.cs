using EucasesLinkingService.PdfManipulation.Classes;
using EucasesLinkingService.PdfManipulation.Models.JsonResult;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EucasesLinkingService.PdfManipulation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LinkFromUrl(string url)
        {
            string contentDir = ControllerContext.HttpContext.Server.MapPath("~/Content");
            string mkFile = ControllerContext.HttpContext.Server.MapPath("~/Content/pdf/mk.bat");
            string mkDir = ControllerContext.HttpContext.Server.MapPath("~/Content/pdf");
            string workingDir = ControllerContext.HttpContext.Server.MapPath("~/Content/work");

            string fileName = string.Empty;
            var result = new result();
            result.files = new List<file>();

            try
            {
                var folderId = Guid.NewGuid();
                fileName = Path.GetFileName(url);
                if (!fileName.EndsWith(".pdf"))
                {
                    fileName = folderId + ".pdf";
                }

                string fileDir = Path.Combine(contentDir, folderId.ToString());
                this.InitFolders(fileDir, workingDir);
                WebClient client = new WebClient();
                string filePath = Path.Combine(fileDir, fileName);
                string linkedFilePath = Path.Combine(fileDir, "linked_" + fileName);
                client.DownloadFile(url, filePath);

                PdfLinker linker = new PdfLinker();
                linker.Link(mkFile, workingDir, $" {filePath} {linkedFilePath} en {mkDir}");


                result.files.Add(new file() { url = $"/Content/{folderId}/linked_{fileName}", name = fileName });
            }
            catch (Exception ex)
            {
                result.files.Add(new file() { error = $"Error!", name = fileName });
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string contentDir = ControllerContext.HttpContext.Server.MapPath("~/Content");
            string mkFile = ControllerContext.HttpContext.Server.MapPath("~/Content/pdf/mk.bat");
            string mkDir = ControllerContext.HttpContext.Server.MapPath("~/Content/pdf");
            string workingDir = ControllerContext.HttpContext.Server.MapPath("~/Content/work/");
            var result = new result();
            result.files = new List<file>();

            try
            {
                var folderId = Guid.NewGuid();

                string fileDir = Path.Combine(contentDir, folderId.ToString());
                this.InitFolders(fileDir, workingDir);

                var fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(fileDir, fileName);
                file.SaveAs(filePath);

                string linkedFilePath = Path.Combine(fileDir, "linked_" + fileName);
                PdfLinker linker = new PdfLinker();
                string error = linker.Link(mkFile, workingDir, $" {filePath} {linkedFilePath} en ");

                if (string.IsNullOrEmpty(error))
                {
                    result.files.Add(new file() { url = $"/Content/{folderId}/linked_{fileName}", name = file.FileName });
                }
                else
                {
                    result.files.Add(new file() { error = $"Error: {error} !", name = file.FileName });
                }
            }
            catch (Exception ex)
            {
                result.files.Add(new file() { error = $"Error: {ex.Message} !", name = file.FileName });
            }

            return Json(result);
        }

        private void InitFolders(string fileDir, string workingDir)
        {
            if (!Directory.Exists(workingDir))
            {
                Directory.CreateDirectory(workingDir);
            }

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
        }
    }
}