using PosApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosApplication.Controllers
{
    public class CompanyInformationController : Controller
    {
        //
        // GET: /CompanyInformation/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login","Account");
            CompanyInformationModel modelInfo = new CompanyInformationModel();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlReadQuery = "select vCompanyName, vAddress, vPhone, vMobileNo, vEmail, vLogo from MASTER.tbCompanyInfo";
            SqlCommand command = new SqlCommand(sqlReadQuery,connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                modelInfo.vCompanyName = dataReader["vCompanyName"].ToString();
                modelInfo.vAddress = dataReader["vAddress"].ToString();
                modelInfo.vPhone = dataReader["vPhone"].ToString();
                modelInfo.vMobile = dataReader["vMobileNo"].ToString();
                modelInfo.vEmail = dataReader["vEmail"].ToString();
                if (dataReader["vLogo"].ToString() != "")
                    modelInfo.vLogo = dataReader["vLogo"].ToString();
                else
                    modelInfo.vLogo = "~/Content/image/default.png";
            }
            connection.Close();
            return View(modelInfo);
        }

        //
        // POST: /CompanyInformation/Create
        [HttpPost]
        public ActionResult Create(CompanyInformationModel modelInfo)
        {
            try
            {
                // TODO: Add insert logic here
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string logoPath = modelInfo.vCompanyName;
                string logoExtension = "";
                if(modelInfo.LogoUpload != null)
                {
                    logoExtension = Path.GetExtension(modelInfo.LogoUpload.FileName);
                    logoPath = (logoPath.Replace("/","")).Replace("'","") + logoExtension;
                    modelInfo.vLogo = "~/Content/image/" + logoPath;
                    modelInfo.LogoUpload.SaveAs(Server.MapPath("~/Content/image/" + logoPath));
                }
                else
                {
                    modelInfo.vLogo = "";
                }
                string sqlQuery = "truncate table MASTER.tbCompanyInfo ";
                string gu_ID = Guid.NewGuid().ToString();
                sqlQuery += "insert into MASTER.tbCompanyInfo (vCompanyID, vCompanyName, vAddress, vPhone, vMobileNo, vEmail, vLogo) values ('" + gu_ID.ToUpper() + "'"+
                ",'" + modelInfo.vCompanyName + "','" + modelInfo.vAddress + "','" + modelInfo.vPhone + "','" + modelInfo.vMobile + "','" + modelInfo.vEmail + "','" + modelInfo.vLogo + "')";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = "All information saved successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
