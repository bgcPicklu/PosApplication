using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using PosApplication.Models;

namespace PosApplication.Controllers
{
    public class userAuthenticationController : Controller
    {
        //
        // GET: /userAuthentication/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserName(string UnitID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<UserModel> lstUserData = new List<UserModel>();
            lstUserData.Add(new UserModel { UserID = "0", UserName = "--Select--" });
            string sqlQuery = "select vUserName,vUserName vUser from INFO.tbLogin where vAccountType not in ('Super Admin', 'Admin') and vUnitName = '" + UnitID + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lstUserData.Add(new UserModel { UserID = dataReader["vUserName"].ToString(), UserName = dataReader["vUser"].ToString() });
            }
            connection.Close();
            return new JsonResult { Data = lstUserData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetAuthenticationData(string UserID, string UnitID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<userAuthenticationModel> lstAuthentication = new List<userAuthenticationModel>();
            string sqlQuery = "select MenuID, MenuName, FormType, iShow, iSave, iEdit, iDelete from " +
                           "(select MN.MenuID, MN.MenuName, FormType, ISNULL(iShow,0) iShow, ISNULL(iSave,0) iSave, ISNULL(iEdit,0) iEdit, ISNULL(iDelete,0) iDelete, " +
                           "MN.iOrderBy from MASTER.tbMenuName MN LEFT OUTER JOIN INFO.tbUserAuthentication ua on mn.MenuID = ua.MenuID where vUserName = '" + UserID + "' "+
                           "and vUnitID = '" + UnitID + "' " +
                           "Union "+
                           "select MenuID, MenuName, FormType, 0 iShow, 0 iSave, 0 iEdit, 0 iDelete, iOrderBy from MASTER.tbMenuName where MenuID not in (select MenuID from " +
                           "INFO.tbUserAuthentication where vUserName = '" + UserID + "' and vUnitID = '" + UnitID + "')) A ORDER BY iOrderBy";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                userAuthenticationModel modelAuthentication = new userAuthenticationModel();
                modelAuthentication.vMenuID = dataReader["MenuID"].ToString();
                modelAuthentication.vMenuName = dataReader["MenuName"].ToString();
                modelAuthentication.vMenuType = dataReader["FormType"].ToString();
                modelAuthentication.iShow = Convert.ToInt16(dataReader["iShow"].ToString());
                modelAuthentication.iSave = Convert.ToInt16(dataReader["iSave"].ToString());
                modelAuthentication.iEdit = Convert.ToInt16(dataReader["iEdit"].ToString());
                modelAuthentication.iDelete = Convert.ToInt16(dataReader["iDelete"].ToString());
                lstAuthentication.Add(modelAuthentication);
            }
            dataReader.Close();
            dataReader.Dispose();
            connection.Close();
            return Json(new { DetailsData = lstAuthentication }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /userAuthentication/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login","Account");
            return View();
        }

        //
        // POST: /userAuthentication/Create
        [HttpPost]
        public JsonResult Create(UserAuthenticationList lstAuthentication)
        {
            try
            {
                // TODO: Add insert logic here
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                var user = "";
                var unit = "";
                string sqlQuery2 = "insert into INFO.tbUserAuthentication (vUnitID,vUserName,MenuID,iShow,iSave,iEdit,iDelete,vAssignedBy,dEntryTime) values ";
                foreach(var getData in lstAuthentication.lstUserAuthentication)
                {
                    if(getData.iShow != 0 || getData.iSave != 0 || getData.iEdit != 0 || getData.iDelete != 0)
                    {
                        user = getData.vUserName;
                        unit = getData.vUnitID;
                        sqlQuery2 += "('" + getData.vUnitID + "','" + getData.vUserName + "','" + getData.vMenuID + "','" + getData.iShow + "','" + getData.iSave + "'," +
                        "'" + getData.iEdit + "','" + getData.iDelete + "','" + Session["userName"] + "',GETDATE()),";
                    }
                }

                string sqlQuery = "delete from INFO.tbUserAuthentication where vUnitID = '" + unit + "' and vUserName = '" + user + "' " + sqlQuery2;
                SqlCommand command = new SqlCommand(sqlQuery.Substring(0,sqlQuery.Length - 1),connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = "Authencation set for " + user }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        //
        // GET: /SupplierInfo/
        public ActionResult AccountHeadIndex()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<AccountHeadModel> lstAccountHeadInfo = new List<AccountHeadModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vAccountHeadAutoID,vAccountHeadID,vAccountHead,iStatus from MASTER.tbAccountHead order by vAccountHead";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                AccountHeadModel acModel = new AccountHeadModel();
                acModel.vAccountHeadIDAuto = dataReader["vAccountHeadAutoID"].ToString();
                acModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                acModel.vAccountHead = dataReader["vAccountHead"].ToString();
                acModel.vStatus = dataReader["iStatus"].ToString();

                lstAccountHeadInfo.Add(acModel);
            }
            connection.Close();
            return View(lstAccountHeadInfo);
        }

        public ActionResult AccountHeadInfo(string AccountHeadIDAuto)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            AccountHeadModel acModel = new AccountHeadModel();
            if (AccountHeadIDAuto == null)
            {
                string sqlQuery = "select 'ACH-'+CONVERT(varchar,(ISNULL(MAX(CAST(SUBSTRING(vAccountHeadID,5,LEN(vAccountHeadID)) as int)),0) + 1)) acHead from MASTER.tbAccountHead";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    acModel.vAccountHeadID = dataReader["acHead"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                AccountHeadIDAuto = AccountHeadIDAuto.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vAccountHeadAutoID,vAccountHeadID,vAccountHead,iStatus from MASTER.tbAccountHead where vAccountHeadAutoID = '" + AccountHeadIDAuto + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    acModel.vAccountHeadIDAuto = dataReader["vAccountHeadAutoID"].ToString();
                    acModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                    acModel.vAccountHead = dataReader["vAccountHead"].ToString();
                    acModel.vStatus = dataReader["iStatus"].ToString();
                }
            }
            connection.Close();
            return View(acModel);
        }
        [HttpPost]
        public ActionResult AccountHeadInfo(AccountHeadModel acModel)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (acModel.vAccountHeadIDAuto != null)
                {
                    sqlQuery = "insert into MASTER.tbUDAccountHead (vAccountHeadAutoID,vAccountHeadID,vAccountHead,iStatus,vEditFlag,vUserName,dEntryTime) " +
                        "select vAccountHeadAutoID,vAccountHeadID,vAccountHead,iStatus,'Edit',vUserName,dEntryTime from MASTER.tbAccountHead where vAccountHeadAutoID = '" + acModel.vAccountHeadIDAuto + "'";
                    sqlQuery += "update MASTER.tbAccountHead set vAccountHeadID = '" + acModel.vAccountHeadID + "', vAccountHead = '" + acModel.vAccountHead + "', iStatus = '" + acModel.vStatus + "'," +
                        "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vAccountHeadAutoID = '" + acModel.vAccountHeadIDAuto + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbAccountHead (vAccountHeadAutoID,vAccountHeadID,vAccountHead,iStatus,vUserName,dEntryTime) values "+
                        "('" + gu_ID.ToUpper() + "','" + acModel.vAccountHeadID + "','" + acModel.vAccountHead + "','" + acModel.vStatus + "','" + Session["userName"] + "',GETDATE())";
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
