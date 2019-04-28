﻿using IJERTS.Objects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJERTS.DAL
{
    public class EditorRepository : IEditorRepository
    {

        public void Register(Users user)
        {
            string query = "INSERT  INTO users (FirstName, LastName, Email, Password, Phone, Organisation, Qualification, Position, SpecializationGroupId, SpecializationId, UserType, UserActivationValue, IsActive, CreatedDateTime, CreatedBy, UpdatedDatetime, UpdatedBy) "
                                + " Values "
                                + " (?FirstName, ?LastName, ?Email, ?Password, ?Phone, ?Organisation, ?Qualification, ?Position, ?SpecializationGroupId, ?SpecializationId, ?UserType, ?UserActivationValue, 1, Now(), 'System', Now(), 'System')";
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Parameters.Add(new MySqlParameter("?FirstName", user.FirstName));
                cmd.Parameters.Add(new MySqlParameter("?LastName", user.LastName));
                cmd.Parameters.Add(new MySqlParameter("?Email", user.Email));
                cmd.Parameters.Add(new MySqlParameter("?Password", user.Password));
                cmd.Parameters.Add(new MySqlParameter("?Phone", user.Phone));
                cmd.Parameters.Add(new MySqlParameter("?Organisation", user.Organisation));
                cmd.Parameters.Add(new MySqlParameter("?Qualification", user.Qualification));
                cmd.Parameters.Add(new MySqlParameter("?Position", user.Position));
                cmd.Parameters.Add(new MySqlParameter("?SpecializationGroupId", (int)CommonDataGroup.SPECIALIZATION));
                cmd.Parameters.Add(new MySqlParameter("?SpecializationId", user.SpecializationId));
                cmd.Parameters.Add(new MySqlParameter("?UserType", "E"));
                cmd.Parameters.Add(new MySqlParameter("?UserActivationValue", user.UserActivationValue));

                using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Users> GetAllUsers(Users users)
        {
            return null;
        }

        public List<Tuple<int, string>> GetSpecialization(CommonCode commonCode)
        {
            List<Tuple<int, string>> lstSpecialization = new List<Tuple<int, string>>();
            string sqlQuery = "SELECT * FROM CommonCode WHERE IsActive = 1 AND GroupId = ?GroupId ORDER BY SortOrder";
            commonCode.GroupId = 2;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.Add(new MySqlParameter("?GroupId", commonCode.GroupId));
            using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = sqlQuery;
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    commonCode.Id = Convert.ToInt32(reader.GetString("Id"));
                    commonCode.GroupId = Convert.ToInt32(reader.GetString("GroupId"));
                    commonCode.GroupName = reader.GetString("GroupName");
                    commonCode.CodeName = reader.GetString("CodeName");
                    commonCode.SortOrder = Convert.ToInt32(reader.GetString("SortOrder"));
                    commonCode.IsActive = Convert.ToInt32(reader.GetString("IsActive"));
                    lstSpecialization.Add(new Tuple<int, string>(commonCode.Id, commonCode.CodeName));
                }
            }
            return lstSpecialization;
        }

        public List<Paper> GetAllPapers()
        {
            List<Paper> lstPaperCollection = new List<Paper>();

            //string queryPaper = "select PAP.PaperId, MainTitle, ShortDesc, CreatedBy, CreatedDateTime from Papers PAP "
            //                        + "INNER JOIN authors AUT ON "
            //                        + "PAP.PaperId = AUT.PaperID WHERFE PAP.IsActive = 1";
            //string queryPaper = "select PaperId, MainTitle, ShortDesc, CreatedBy, CreatedDateTime from Papers WHERE IsActive = 1";

            string queryPaper = "select p.PaperId, p.MainTitle, p.ShortDesc, p.CreatedBy, p.CreatedDateTime, ps.status, CONCAT(u.FirstName, ' ' ,u.LastName) AS ReviewerName "
            + " from Papers p left join paperstatus ps on ps.paperid = p.paperid "
            + " left join users u on u.UserId = ps.UserId where p.IsActive = 1 ";

            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = queryPaper;

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Paper objPaper = new Paper();
                    objPaper.PaperId = Convert.ToInt32(reader["PaperId"]);
                    objPaper.MainTitle = reader["MainTitle"].ToString();
                    objPaper.ShortDesc = reader["ShortDesc"].ToString();
                    objPaper.CreatedBy = reader["CreatedBy"].ToString();
                    objPaper.CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"].ToString());
                    objPaper.PaperStatus = reader["status"].ToString();
                    objPaper.ReviewerName = reader["ReviewerName"].ToString();
                    lstPaperCollection.Add(objPaper);
                }

                return lstPaperCollection;

            }
        }

        public Paper GetPaperDetails(int id)
        {
            Paper paper = new Paper();
            PaperComments comments = new PaperComments();

            List<PaperAuthors> lstPaperAuth = new List<PaperAuthors>();
            string queryPaper = "SELECT PAP.PaperId, MainTitle, ShortDesc, Subject, Tags,PAP.CreatedBy, PAP.CreatedDateTime, "
                                    + " CompleteFilePath, FileName, CompleteDeclarationFilePath, DeclarationFileName,  AuthorFirstName, "
                                    + "  AuthorLastName, AuthorDepartment, AuthorOrganisation, AuthorSubject, COM.Comments from Papers PAP "
                                     + " INNER JOIN authors AUT ON PAP.PaperId = AUT.PaperID "
                                     + " LEFT OUTER JOIN papercomments COM ON PAP.PaperId = COM.PaperId "
                                     + " AND COM.CommentsID = (SELECT MAX(CommentsID) from papercomments WHERE PaperId = ?PaperId) "
                                     + " WHERE PAP.IsActive = 1 AND PAP.PaperId = ?PaperId";

            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.Add(new MySqlParameter("?PaperId", id));

            using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
            {
                cmd.Connection = con;
                con.Open();
                cmd.CommandText = queryPaper;

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PaperAuthors auth = new PaperAuthors();
                    paper.PaperId = Convert.ToInt32(reader["PaperId"]);
                    paper.MainTitle = reader["MainTitle"].ToString();
                    paper.ShortDesc = reader["ShortDesc"].ToString();
                    paper.CreatedBy = reader["CreatedBy"].ToString();
                    paper.Subject = Convert.ToString(reader["Subject"]);
                    paper.Tags = Convert.ToString(reader["Tags"]);
                    paper.PaperPath = Convert.ToString(reader["CompleteFilePath"]);
                    paper.FileName = Convert.ToString(reader["FileName"]);
                    paper.PaperPath = string.Format("{0}\\{1}", paper.PaperPath, paper.FileName);
                    paper.CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"].ToString());
                    paper.DeclarationFileName = Convert.ToString(reader["DeclarationFileName"]);
                    paper.DeclarationPaperPath = Convert.ToString(reader["CompleteDeclarationFilePath"]);
                    //paper.DeclarationPaperPath = string.Format("{0}\\{1}", paper.DeclarationPaperPath, paper.DeclarationFileName);

                    comments.Comments = Convert.ToString(reader["Comments"]);

                    auth.AuthorFirstName = Convert.ToString(reader["AuthorFirstName"]);
                    auth.AuthorLastName = Convert.ToString(reader["AuthorLastName"]);
                    auth.Department = Convert.ToString(reader["AuthorDepartment"]);
                    auth.Organisation = Convert.ToString(reader["AuthorOrganisation"]);
                    auth.Subject = Convert.ToString(reader["AuthorSubject"]);
                    
                    lstPaperAuth.Add(auth);
                }
                paper.Comments = comments;
                paper.Authors = lstPaperAuth;
            }

            return paper;
        }

        public int PostComments(int paperId, string comments, int userId, int approverId)
        {

            string queryComments = " INSERT INTO papercomments(PaperId, Comments, IsEditorComments, IsActive, CreatedBy, CreatedDateTime) "
                + " VALUES(?paperId, ?comments, 1, 1, ?userId, now()); ";

            string queryCommentsStatusUpdate = "UPDATE paperStatus SET `status` = 'COMMENTS ADDED' WHERE paperID = ?paperID";


            string queryApprover = "INSERT INTO `papersapprovers`  " +
                                    "(PaperId, ApproverId, IsActive, CreatedDateTime, CreatedBy, UpdatedDatetime, UpdatedBy) " +
                                    " VALUES " +
                                    "(?PaperId, ?ApproverId, 1, now(), ?UserId, now(), ?UserId); ";

            string queryApproverStatusUpdate = "UPDATE paperStatus SET `status` = 'REVIEW IN PROGRESS' WHERE paperID = ?paperID";


            MySqlCommand cmd = new MySqlCommand();
            cmd.Parameters.Add(new MySqlParameter("?PaperId", paperId));
            cmd.Parameters.Add(new MySqlParameter("?comments", comments));
            cmd.Parameters.Add(new MySqlParameter("?userId", userId));
            cmd.CommandText = queryComments;

            MySqlCommand cmdComments = new MySqlCommand();
            cmdComments.Parameters.Add(new MySqlParameter("?PaperId", paperId));
            cmdComments.CommandText = queryCommentsStatusUpdate;

            MySqlCommand cmdApprover = new MySqlCommand();
            cmdApprover.Parameters.Add(new MySqlParameter("?PaperId", paperId));
            cmdApprover.Parameters.Add(new MySqlParameter("?ApproverId", approverId));
            cmdApprover.Parameters.Add(new MySqlParameter("?UserId", userId));
            cmdApprover.CommandText = queryApprover;

            MySqlCommand cmdApproverStatus = new MySqlCommand();
            cmdApproverStatus.Parameters.Add(new MySqlParameter("?PaperId", paperId));
            cmdApproverStatus.CommandText = queryApproverStatusUpdate;

            using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
            {
                if (!string.IsNullOrEmpty(comments))
                {
                    if (con.State != System.Data.ConnectionState.Closed) con.Close();
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();

                    if (con.State != System.Data.ConnectionState.Closed) con.Close();
                    con.Open();
                    cmdComments.Connection = con;
                    cmdComments.ExecuteNonQuery();

                }

                if (approverId != -1)
                {
                    if (con.State != System.Data.ConnectionState.Closed) con.Close();
                    cmdApprover.Connection = con;
                    con.Open();
                    cmdApprover.ExecuteNonQuery();

                    if (con.State != System.Data.ConnectionState.Closed) con.Close();
                    cmdApproverStatus.Connection = con;
                    con.Open();
                    cmdApproverStatus.ExecuteNonQuery();

                }
            }

            return 0;
        }

        public int DeletePaper(int paperId)
        {
            string query = "update papers set isactive = 0 WHERE PAperID = ?PaperID";

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Parameters.Add(new MySqlParameter("?PaperID", paperId));


                using (MySqlConnection con = new MySqlConnection(DBConnection.ConnectionString))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;

        }
    }
}
