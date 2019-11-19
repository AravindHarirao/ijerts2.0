using IJERTS.Common;
using IJERTS.Objects;
using System.Web.Mvc;
using IJERTS.BLL;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IJERTS.Web.Controllers
{
    public class HomeController : Controller
    {
        IHome home = new Home();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public ActionResult FAQ()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public ActionResult ResearachArea()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateContactUs(ContactUs contactUs)
        {
            string sResult = EmailHelper.SendContactUsEmail(contactUs);
            if(!string.IsNullOrEmpty(sResult) && sResult.Equals("Success"))
            {
                TempData["ContactUsMessage"] = "Thank you, your query has been submitted and we will get back to you shortly...";
            }
            else
            {
                TempData["ContactUsMessage"] = "Sorry for the inconvenience, we are facing issues in sending your request. Please verify it again...";
            }
            return RedirectToAction("Contact");
        }

        public ActionResult CallforPaper()
        {
            return View();
        }

        public ActionResult Queries()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Queries(Queries query)
        {
            int result = home.PostQuery(query);
            if (result == 0)
            {
                TempData["QueryStatus"] = "Query posted successfully. We will contact you shortly";
            }
            else
            {
                TempData["QueryStatus"] = "Error in posting Query. Please contact administrator";

            }
            ModelState.Clear();
            return View(new Queries());
        }

        public ActionResult Downloads()
        {
            return View();
        }

        public ActionResult Error()
        {

            return View();
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.Trace.TraceMode });
        }

        public ActionResult LatestIssues()
        {
            return View(home.GetCurrentIssues());

        }

        public FileResult DownloadCurrentIssue(int DPaperId)
        {            
            string selectedFileName = (home.GetCurrentIssues()).FirstOrDefault(x => x.PaperId == DPaperId).PaperName;
            string selectedFilePath = (home.GetCurrentIssues()).FirstOrDefault(x => x.PaperId == DPaperId).PaperPath;
            string sUploadedPath = Server.MapPath("~/CurrentIssues/");
            string sFileName = sUploadedPath + selectedFilePath;
            if (!string.IsNullOrEmpty(sFileName))
            {
                if (System.IO.File.Exists(sFileName))
                {
                    string fileExtension = Path.GetExtension(sFileName);
                    home.IncreamentDownloadCounter(DPaperId);
                    return this.File(sFileName, "application/" + fileExtension, selectedFilePath);
                }
                else
                {
                    return this.File(Path.Combine(Server.MapPath("~/UploadedFiles/"), "FileNotExists.txt"), "application/txt", "FileNotExists.txt");
                }
            }
            else
            {
                return this.File(Path.Combine(Server.MapPath("~/UploadedFiles/"), "FileNotExists.txt"), "application/txt", "FileNotExists.txt");
            }
        }

    }
}
