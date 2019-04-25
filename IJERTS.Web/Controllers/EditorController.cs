﻿using IJERTS.BLL;
using IJERTS.Common;
using IJERTS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace IJERTS.Web.Controllers
{
    public class EditorController : BaseController
    {
        IEditor _editor = new Editor();
        ILogin _login = new Login();
        IReviewer _review = new Reviewer();
        
        // GET: /<controller>/
        [HttpGet]
        public ActionResult Index()
        {
            ActionResult result = this.ValidateEditorToken();
            if (result != null)
            {
                return result;
            }

            //Getting the Papers
            List<Paper> papers = _editor.GetAllPapers();

            //Getting the Reviwer 
            List<Users> reviewers = _review.GetAllReviewers();
            ViewData["Reviewers"] = new SelectList((System.Collections.IEnumerable)reviewers, "UserId", string.Format("{0}, {1}", LastName, FirstName));

            List<Queries> queries = _editor.GetQueries();

            var objResults = new Tuple<List<Paper>, List<Users>, List<Queries>>(papers, reviewers, queries);

            ViewBag.UserType = "Editor";
            return View("Dashboard", objResults);
        }
        
        public ActionResult UpdatePaper(Paper paper, int txtPaperId, string txtComments, string Reviewers)
        {
            int reviewerId = string.IsNullOrEmpty(Reviewers) ? -1 : int.Parse(Reviewers.Trim());
            _editor.PostComments(txtPaperId, txtComments, int.Parse(HttpContext.Session["UserId"].ToString()), reviewerId);
            TempData["PaperPostingFailed"] = "Comments posted successfully";

            paper = _editor.GetPaperDetails(txtPaperId);
            List<Users> reviewers = _review.GetAllReviewersForPaper(paper.PaperId);

            ViewData["Reviewers"] = new SelectList((System.Collections.IEnumerable)reviewers, "UserId", "FirstName");

            return View("PaperDetails", paper);
        }

        [HttpPost]
        public ActionResult GetPaperDetails(int PaperId)
        {
            Paper paper = new Paper();

            ActionResult result = this.ValidateEditorToken();
            if (result != null)
            {
                return result;
            }
            paper = _editor.GetPaperDetails(PaperId);
            if(paper != null)
            {
                //List<Users> reviewers = _review.GetAllReviewersForPaper(paper.PaperId);
                //ViewData["Reviewers"] = new SelectList((System.Collections.IEnumerable)reviewers, "UserId", "FirstName");

                List<Users> reviewers = _review.GetAllReviewersForPaper(paper.PaperId);
                List<SelectListItem> lstSpec = new List<SelectListItem>();
                foreach (var obj in reviewers)
                {
                    lstSpec.Add(new SelectListItem { Value = obj.UserId.ToString(), Text = obj.FirstName });
                }
                ViewBag.Reviewers = new SelectList(lstSpec, "Value", "Text");
            }
            return View("PaperDetails", paper);
        }

        [HttpGet]
        public ActionResult Register()
        {
            CommonCode commonCode = new CommonCode();
            commonCode.GroupId = 5;
            List<Tuple<int, string>> lstSpecialization = _editor.GetSpecialization(commonCode);
            List<SelectListItem> lstSpec = new List<SelectListItem>();
            foreach (var specialization in lstSpecialization)
            {
                lstSpec.Add(new SelectListItem { Value = specialization.Item1.ToString(), Text = specialization.Item2 });
            }
            ViewBag.Specialization = new SelectList(lstSpec, "Value", "Text");
            
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users user)
        {
            user.UserActivationValue = Guid.NewGuid().ToString();
            user.Password = CommonHelper.GenerateDynamicPassword();

            _editor.Register(user);

            EmailHelper.SendWelcomeEmailtoUser(user);

            return View("CompleteRegister");
        }

        public FileResult DownloadAuthorPaper(Int32 PaperId, string PaperFileName)
        {
            string sUploadedPath = Server.MapPath("~/UploadedFiles/AuthorPapers/");
            string sFileName = sUploadedPath + PaperFileName;
            if(!string.IsNullOrEmpty(sFileName))
            {
                if (System.IO.File.Exists(sFileName))
                {
                    string fileExtension = Path.GetExtension(sFileName);
                    return this.File(sFileName, "application/" + fileExtension, PaperFileName);
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

        public FileResult DownloadDeclaration(Int32 PaperId, string DeclarationPaperfilename)
        {
            string sUploadedPath = Server.MapPath("~/UploadedFiles/Declaration/");
            string sFileName = sUploadedPath + DeclarationPaperfilename;
            if (!string.IsNullOrEmpty(sFileName))
            {
                if (System.IO.File.Exists(sFileName))
                {
                    string fileExtension = Path.GetExtension(sFileName);
                    return this.File(sFileName, "application/" + fileExtension, DeclarationPaperfilename);
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

        public ActionResult EditorialBoard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetQueryDetails(int QueryId)
        {
            Queries query = _editor.GetQueriesForId(QueryId);
            return View("QueryDetails", query);
        }

        [HttpPost]
        public ActionResult QueryDetails(int QueryId, string QueryAnswer)
        {
            int result = _editor.UpdateQuery(QueryId, QueryAnswer);
            TempData["Result"] = result == 0 ? "Query updated successfully" : "Query update failed. Please try again later";
            Queries query = _editor.GetQueriesForId(QueryId);
            EmailHelper.AnswerQuery(query);

            return View(query);
        }

        [HttpPost]
        public RedirectToRouteResult DeletePaper(int PaperId)
        {
            if (_editor.DeletePaper(PaperId) == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public RedirectToRouteResult DeleteReviewer(int UserId)
        {
            if (_review.DeleteReviewer(UserId) == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return null;
            }
        }

    }
}
