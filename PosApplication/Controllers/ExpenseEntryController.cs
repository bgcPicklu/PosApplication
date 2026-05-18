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
    public class ExpenseEntryController : Controller
    {
        //
        // GET: /ExpenseEntry/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getTransactionNo(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select distinct 'ETRA-'+CONVERT(varchar,ISNULL(MAX(CAST(SUBSTRING(vTransactionIDAuto,6,LEN(vTransactionIDAuto)) as int)),0) + 1) vTransactionIDAuto from POS.tbExpenseEntry where vUnitID = '" + Session["UnitName"] + "'  and MONTH(dDate) = MONTH(CONVERT(date,'" + dDate + "',103)) and Year(dDate) = Year(CONVERT(date,'" + dDate + "',103))";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            var vTransactionIDAuto = "";
            while (dataReader.Read())
            {
                vTransactionIDAuto = dataReader["vTransactionIDAuto"].ToString();
            }
            dataReader.Close();
            dataReader.Dispose();
            connection.Close();
            return Json(new { vTransactionIDAuto = vTransactionIDAuto }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTableData(string vChallanNo, string dDate, string vTransactionID)
        {
            List<ExpenseEntryModel> lstExModel = new List<ExpenseEntryModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            if(Session["AccountType"].ToString() == "Super Admin" || Session["AccountType"].ToString() == "Admin")
            {
                connection.Open();
                string sqlQuery = "select COUNT(*) iCount from POS.tbExpenseEntry where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                int count = 0;
                while (dataReader.Read())
                {
                    count = Convert.ToInt16(dataReader["iCount"].ToString());
                }
                dataReader.Close();
                dataReader.Dispose();
                connection.Close();
                if (count > 0)
                {
                    connection.Open();
                    sqlQuery = "select vTransactionIDAuto,vTransactionID,vChallanNo,CONVERT(varchar(10),dDate,105) dDate,ee.vAccountHeadID,ah.vAccountHead,nAmount from POS.tbExpenseEntry ee right join " +
                        "MASTER.tbAccountHead ah on ee.vAccountHeadID = ah.vAccountHeadID  where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                    command.CommandText = sqlQuery;
                    dataReader = command.ExecuteReader();
                    int i = 0;
                    while (dataReader.Read())
                    {
                        ExpenseEntryModel exModel = new ExpenseEntryModel();
                        exModel.vTransactionIDAuto = dataReader["vTransactionIDAuto"].ToString();
                        exModel.vTransactionID = dataReader["vTransactionID"].ToString();
                        exModel.vChallanNo = dataReader["vChallanNo"].ToString();
                        exModel.dDate = dataReader["dDate"].ToString();
                        exModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                        exModel.vAccountHead = dataReader["vAccountHead"].ToString();
                        exModel.nAmount = Convert.ToDecimal(dataReader["nAmount"].ToString());
                        i++;
                        lstExModel.Add(exModel);
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    connection.Close();
                }
                else
                {
                    connection.Open();
                    sqlQuery = "select vAccountHeadID,vAccountHead from MASTER.tbAccountHead where vAccountHeadID not in (select vAccountHeadID from POS.tbExpenseEntry where dDate = CONVERT(date,'" + dDate + "',103) and vUnitID = '" + Session["UnitName"] + "')  order by vAccountHead";
                    command.CommandText = sqlQuery;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ExpenseEntryModel exModel = new ExpenseEntryModel();
                        exModel.vTransactionID = vTransactionID;
                        exModel.vChallanNo = vChallanNo;
                        exModel.dDate = dDate;
                        exModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                        exModel.vAccountHead = dataReader["vAccountHead"].ToString();
                        lstExModel.Add(exModel);
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    connection.Close();
                }
            }
            else
            {
                connection.Open();
                string sqlQuery = "select COUNT(*) iCount from POS.tbExpenseEntry where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                int count = 0;
                while (dataReader.Read())
                {
                    count = Convert.ToInt16(dataReader["iCount"].ToString());
                }
                dataReader.Close();
                dataReader.Dispose();
                connection.Close();
                if (count > 0)
                {
                    connection.Open();
                    sqlQuery = "select vAccountHeadID,vAccountHead from MASTER.tbAccountHead where vAccountHeadID not in (select vAccountHeadID from POS.tbExpenseEntry where dDate = CONVERT(date,'" + dDate + "',103) and vUnitID = '" + Session["UnitName"] + "')  order by vAccountHead";
                    command.CommandText = sqlQuery;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ExpenseEntryModel exModel = new ExpenseEntryModel();
                        exModel.vTransactionID = vTransactionID;
                        exModel.vChallanNo = "";
                        exModel.dDate = dDate;
                        exModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                        exModel.vAccountHead = dataReader["vAccountHead"].ToString();
                        lstExModel.Add(exModel);
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    connection.Close();
                }
                else
                {
                    connection.Open();
                    sqlQuery = "select vAccountHeadID,vAccountHead from MASTER.tbAccountHead where vAccountHeadID not in (select vAccountHeadID from POS.tbExpenseEntry where dDate = CONVERT(date,'" + dDate + "',103) and vUnitID = '" + Session["UnitName"] + "')  order by vAccountHead";
                    command.CommandText = sqlQuery;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ExpenseEntryModel exModel = new ExpenseEntryModel();
                        exModel.vTransactionID = vTransactionID;
                        exModel.vChallanNo = vChallanNo;
                        exModel.dDate = dDate;
                        exModel.vAccountHeadID = dataReader["vAccountHeadID"].ToString();
                        exModel.vAccountHead = dataReader["vAccountHead"].ToString();
                        lstExModel.Add(exModel);
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    connection.Close();
                }
            }
            return Json(new { Tabledata = lstExModel}, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ExpenseEntry/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login","Account");
            return View();
        }

        //
        // POST: /ExpenseEntry/Create
        [HttpPost]
        public ActionResult Create(string vTransactionIDAuto,ExpenseEntryList lstExModel)
        {
            try
            {
                // TODO: Add insert logic here
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "select COUNT(*) iCount from POS.tbExpenseEntry where vTransactionIDAuto = '" + vTransactionIDAuto + "' and vUnitID = '" + Session["UnitName"] + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                int count = 0;
                while (dataReader.Read())
                {
                    count = Convert.ToInt16(dataReader["iCount"].ToString());
                }
                dataReader.Close();
                dataReader.Dispose();
                connection.Close();
                var message = "All information saved successfully.";
                if (count > 0)
                {
                    connection.Open();
                    message = "All information updated successfully.";
                    sqlQuery = "delete from POS.tbExpenseEntry where vTransactionIDAuto = '" + vTransactionIDAuto + "'";
                    command.CommandText = sqlQuery;
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                connection.Open();
                sqlQuery = "insert into POS.tbExpenseEntry (vTransactionIDAuto,vTransactionID,vChallanNo,vUnitID,dDate,vAccountHeadID,nAmount,vUserName,dEntryTime) values";
                foreach(var getRow in lstExModel.lstExpenseEntry)
                {
                    sqlQuery += "('" + getRow.vTransactionID + "','" + getRow.vTransactionID + "','" + getRow.vChallanNo + "','" + Session["UnitName"] + "',CONVERT(date,'" + getRow.dDate + "',103),"+
                    "'" + getRow.vAccountHeadID + "','" + getRow.nAmount + "','" + Session["userName"] + "',GETDATE()),";
                }
                command.CommandText = sqlQuery.Substring(0, sqlQuery.Length - 1);
                command.ExecuteNonQuery();
                
                connection.Close();
                return Json(new { success = true, message = message}, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        //Expense Entry
        public ActionResult ExpenseEntryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ExpenseEntryReport(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Expense Entry Report";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult Details()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }
    }
}
