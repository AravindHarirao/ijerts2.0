﻿using IJERTS.DAL;
using IJERTS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJERTS.BLL
{
    public class Reviewer : IReviewer
    {
        IReviewerRepository _reviewRepository = new ReviewerRepository();

        public Users Register(Users user)
        {
            return _reviewRepository.Register(user);
        }

        public List<Tuple<int, string>> GetSpecialization(CommonCode commonCode)
        {
            List<Tuple<int, string>> lstSpecialization = new List<Tuple<int, string>>();
            lstSpecialization = _reviewRepository.GetSpecialization(commonCode);
            return lstSpecialization;
        }

        public List<Users> GetAllReviewers()
        {
            return _reviewRepository.GetAllReviewers();
        }

        public List<Users> GetAllReviewersForPaper(int paperId)
        {
            return _reviewRepository.GetAllReviewersForPaper(paperId);
        }


        public void ActivateDeActivateReviewer(Users user)
        {
            _reviewRepository.ActivateDeActivateReviewer(user);
        }

        public Users GetReviewerDetails(Int32 userId)
        {
            return _reviewRepository.GetReviewerDetails(userId);
        }

        public List<Paper> GetAssignedPaper(Int32 userId)
        {
            return _reviewRepository.GetAssignedPaper(userId);
        }

        public int UpdatePaperStatus(int userId, int paperId, string Comments, string Approve)
        {
            return _reviewRepository.UpdatePaperStatus(userId, paperId, Comments, Approve);
        }
        
        public Users GetMyProfileDetails(Int64 UserId)
        {
            return _reviewRepository.GetMyProfileDetails(UserId);
        }

        public Users UpdateProfile(Users user)
        {
            return _reviewRepository.UpdateProfile(user);
        }

        public int DeleteReviewer(int UserId)
        {
            return _reviewRepository.DeleteReviewer(UserId);
        }
    }
}
