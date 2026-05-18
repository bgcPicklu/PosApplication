using PosApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosApplication.Controllers
{
    public class SupplierInfoController : Controller
    {
        //
        // GET: /SupplierInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<SupplierInformationModel> lstSupplierInfo = new List<SupplierInformationModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vSupplierIDAuto,vSupplierID,vSupplierName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile,isActive from "+
                "MASTER.tbSupplierInfo order by vSupplierName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                SupplierInformationModel supplierModel = new SupplierInformationModel();
                supplierModel.vSupplierIDAuto = dataReader["vSupplierIDAuto"].ToString();
                supplierModel.vSupplierID = dataReader["vSupplierID"].ToString();
                supplierModel.vSupplierName = dataReader["vSupplierName"].ToString();
                supplierModel.vAddress = dataReader["vAddress"].ToString();
                supplierModel.vPhone = dataReader["vPhone"].ToString();
                supplierModel.vMobileNo = dataReader["vMobileNo"].ToString();
                supplierModel.vEmail = dataReader["vEmail"].ToString();
                supplierModel.vContactPerson = dataReader["vContactPerson"].ToString();
                supplierModel.vPersonMobile = dataReader["vPersonMobile"].ToString();
                supplierModel.vStatus = dataReader["isActive"].ToString();

                lstSupplierInfo.Add(supplierModel);
            }
            connection.Close();
            return View(lstSupplierInfo);
        }

        //
        // GET: /SupplierInfo/Create
        public ActionResult Create(string supplierID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            SupplierInformationModel modelSupplier = new SupplierInformationModel();
            if (supplierID == null)
            {
                supplierID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vSupplierID,5,LEN(vSupplierID)) as int)),0)+1 supplierID from MASTER.tbSupplierInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelSupplier.vSupplierID = "SUP-" + dataReader["supplierID"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {

                supplierID = supplierID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vSupplierIDAuto,vSupplierID,vSupplierName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile,isActive from " +
                    "MASTER.tbSupplierInfo where vSupplierIDAuto = '" + supplierID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelSupplier.vSupplierIDAuto = dataReader["vSupplierIDAuto"].ToString();
                    modelSupplier.vSupplierID = dataReader["vSupplierID"].ToString();
                    modelSupplier.vSupplierName = dataReader["vSupplierName"].ToString();
                    modelSupplier.vAddress = dataReader["vAddress"].ToString();
                    modelSupplier.vPhone = dataReader["vPhone"].ToString();
                    modelSupplier.vMobileNo = dataReader["vMobileNo"].ToString();
                    modelSupplier.vEmail = dataReader["vEmail"].ToString();
                    modelSupplier.vContactPerson = dataReader["vContactPerson"].ToString();
                    modelSupplier.vPersonMobile = dataReader["vPersonMobile"].ToString();
                    modelSupplier.vStatus = dataReader["isActive"].ToString();
                }
            }
            connection.Close();
            return View(modelSupplier);
        }

        //
        // POST: /SupplierInfo/Create
        [HttpPost]
        public ActionResult Create(SupplierInformationModel modelSupplier)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelSupplier.vSupplierIDAuto != null)
                {
                    sqlQuery = "insert into MASTER.tbUDSupplierInfo (vSupplierIDAuto,vSupplierID,vSupplierName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile," +
                        "vEditFlag,isActive,vUserName,dEntryTime) select vSupplierIDAuto,vSupplierID,vSupplierName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,"+
                        "vPersonMobile,'Edit',isActive,vUserName,dEntryTime from MASTER.tbSupplierInfo where vSupplierIDAuto = '" + modelSupplier.vSupplierIDAuto + "'";
                    
                    sqlQuery += "update MASTER.tbSupplierInfo set vSupplierID = '" + modelSupplier.vSupplierID + "',vSupplierName = '" + modelSupplier.vSupplierName + "', vAddress = '" + modelSupplier.vAddress + "'," +
                        "vPhone = '" + modelSupplier.vPhone + "', vMobileNo = '" + modelSupplier.vMobileNo + "', vEmail = '" + modelSupplier.vEmail + "'," +
                        "vContactPerson = '" + modelSupplier.vContactPerson + "', vPersonMobile = '" + modelSupplier.vPersonMobile + "', isActive = '" + modelSupplier.vStatus + "'," +
                        "vUserName = '', dEntryTime = GETDATE() where vSupplierIDAuto = '" + modelSupplier.vSupplierIDAuto + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbSupplierInfo (vSupplierIDAuto,vSupplierID,vSupplierName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile," +
                        "isActive,vUserName,dEntryTime) values ('" + gu_ID.ToUpper() + "','" + modelSupplier.vSupplierID + "','" + modelSupplier.vSupplierName + "'," +
                    "'" + modelSupplier.vAddress + "','" + modelSupplier.vPhone + "','" + modelSupplier.vMobileNo + "','" + modelSupplier.vEmail + "','" + modelSupplier.vContactPerson + "'," +
                    "'" + modelSupplier.vPersonMobile + "','" + modelSupplier.vStatus + "','',GETDATE())";
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
