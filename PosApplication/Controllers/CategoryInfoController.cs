using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PosApplication.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace PosApplication.Controllers
{
    public class CategoryInfoController : Controller
    {
        //
        // GET: /CategoryInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<CategoryInfoModel> lstCatInfo = new List<CategoryInfoModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "Select vAutoCategoryID, vCategoryID, vCategoryName, vStatus from MASTER.tbCategoryInformation Order By vCategoryName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                CategoryInfoModel catModel = new CategoryInfoModel();
                catModel.vAutoCategoryID = dataReader["vAutoCategoryID"].ToString();
                catModel.vCategoryID = dataReader["vCategoryID"].ToString();
                catModel.vCategoryName = dataReader["vCategoryName"].ToString();
                catModel.vStatus = dataReader["vStatus"].ToString();

                lstCatInfo.Add(catModel);
            }
            connection.Close();
            return View(lstCatInfo);
        }

        //
        // GET: /CategoryInfo/Create
        public ActionResult Create(string catID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            CategoryInfoModel modelCat = new CategoryInfoModel();
            if (catID == null)
            {
                catID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vCategoryID,5,LEN(vCategoryID)) as int)),0)+1 vCategoryId from MASTER.tbCategoryInformation";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelCat.vCategoryID = "CAT-" + dataReader["vCategoryId"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                catID = catID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vAutoCategoryID, vCategoryID, vCategoryName, vStatus from MASTER.tbCategoryInformation where vAutoCategoryID = '" + catID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelCat.vAutoCategoryID = dataReader["vAutoCategoryID"].ToString();
                    modelCat.vCategoryID = dataReader["vCategoryID"].ToString();
                    modelCat.vCategoryName = dataReader["vCategoryName"].ToString();
                    modelCat.vStatus = dataReader["vStatus"].ToString();
                }
            }
            connection.Close();
            return View(modelCat);
        }

        //
        // POST: /CategoryInfo/Create
        [HttpPost]
        public ActionResult Create(CategoryInfoModel modelCategory)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelCategory.vAutoCategoryID != null)
                {
                    sqlQuery = "insert into MASTER.tbUDCategoryInformation (vAutoCategoryID,vCategoryID,vCategoryName,vStatus,vEditFlag,vUserName,dEntryTime) " +
                        "select vAutoCategoryID,vCategoryID,vCategoryName,vStatus,'Edit',vUserName,dEntryTime from MASTER.tbCategoryInformation where vAutoCategoryID = '" + modelCategory.vAutoCategoryID + "'";
                    sqlQuery += "update MASTER.tbCategoryInformation set vCategoryID = '" + modelCategory.vCategoryID + "', vCategoryName = '" + modelCategory.vCategoryName + "', vStatus = '" + modelCategory.vStatus + "'," +
                        "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vAutoCategoryID = '" + modelCategory.vAutoCategoryID + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbCategoryInformation (vAutoCategoryID, vCategoryID, vCategoryName, vStatus, vUserName, dEntryTime) values ('" + gu_ID.ToUpper() + "'," +
                    "'" + modelCategory.vCategoryID + "','" + modelCategory.vCategoryName + "','" + modelCategory.vStatus + "','" + Session["userName"] + "',GETDATE())";
                }
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
