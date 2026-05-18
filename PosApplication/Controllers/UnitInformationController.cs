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
    public class UnitInformationController : Controller
    {
        //public JsonResult UnitNameCheck(string unitName)
        //{
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
        //    connection.Open();
        //    string sqlQuery = "select COUNT(*) countRow from MASTER.tbCompanyInfo where vCompanyName = '" + unitName + "'";
        //    SqlCommand command = new SqlCommand(sqlQuery, connection);
        //    SqlDataReader dataReader = command.ExecuteReader();
        //    int countRow = 0;
        //    bool success = false;
        //    var message = "";
        //    while (dataReader.Read())
        //    {
        //        countRow = Convert.ToInt16(dataReader["countRow"].ToString());
        //    }

        //    if(countRow > 0)
        //    {
        //        success = true;
        //        message = "Unit name already exists!!!";
        //    }
        //    connection.Close();
        //    return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        //}

        //
        // GET: /UnitInformation/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["AccountType"].ToString().Equals("Super Admin") || Session["AccountType"].ToString().Equals("Admin"))
            {
                List<UnitInformationModel> lstUnitInfo = new List<UnitInformationModel>();
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "select vUnitId,vUnitName,vAddress,vPhone,vMobileNo,vEmail,vUnitType,vUnitInCharge,vInChargeMobile,CONVERT(varchar(10),dStartFrom,105) dStartFrom," +
                    "isActive from MASTER.tbUnitInfo order by vUnitType desc, vUnitName Asc";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    UnitInformationModel unitModel = new UnitInformationModel();
                    unitModel.vUnitID = dataReader["vUnitId"].ToString();
                    unitModel.vUnitName = dataReader["vUnitName"].ToString();
                    unitModel.vAddress = dataReader["vAddress"].ToString();
                    unitModel.vPhone = dataReader["vPhone"].ToString();
                    unitModel.vMobileNo = dataReader["vMobileNo"].ToString();
                    unitModel.vEmail = dataReader["vEmail"].ToString();
                    unitModel.vUnitType = dataReader["vUnitType"].ToString();
                    unitModel.vUnitInCharge = dataReader["vUnitInCharge"].ToString();
                    unitModel.vInChargeMobile = dataReader["vInChargeMobile"].ToString();
                    unitModel.dStartedDate = dataReader["dStartFrom"].ToString();
                    unitModel.vStatus = dataReader["isActive"].ToString();

                    lstUnitInfo.Add(unitModel);
                }
                connection.Close();
                return View(lstUnitInfo);
            }
            else
                return RedirectToAction("Create","UnitInformation");
        }

        //
        // GET: /UnitInformation/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /UnitInformation/Create
        public ActionResult Create(string unitID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            if (unitID == null)
            {
                unitID = "";
            }
            unitID = unitID.Replace(" ", "").Replace("\n", "");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vCompanyId,vUnitID,vUnitName,vAddress,vPhone,vMobileNo,vEmail,vUnitInCharge,vInChargeMobile,CONVERT(varchar(10),dStartFrom,105) " +
                "dStartFrom,isActive,vUnitType from MASTER.tbUnitInfo where vUnitID = '" + unitID + "'";
            UnitInformationModel modelUnit = new UnitInformationModel();
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                modelUnit.vCompanyID = dataReader["vCompanyId"].ToString();
                modelUnit.vUnitID = dataReader["vUnitID"].ToString();
                modelUnit.vUnitName = dataReader["vUnitName"].ToString();
                modelUnit.vAddress = dataReader["vAddress"].ToString();
                modelUnit.vPhone = dataReader["vPhone"].ToString();
                modelUnit.vMobileNo = dataReader["vMobileNo"].ToString();
                modelUnit.vEmail = dataReader["vEmail"].ToString();
                modelUnit.vUnitInCharge = dataReader["vUnitInCharge"].ToString();
                modelUnit.vInChargeMobile = dataReader["vInChargeMobile"].ToString();
                modelUnit.dStartedDate = dataReader["dStartFrom"].ToString();
                modelUnit.vStatus = dataReader["isActive"].ToString();
                modelUnit.vUnitType = dataReader["vUnitType"].ToString();
            }
            connection.Close();
            return View(modelUnit);
        }

        //
        // POST: /UnitInformation/Create
        [HttpPost]
        public JsonResult Create(UnitInformationModel modelUnit)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if(modelUnit.vUnitID != null)
                {
                    sqlQuery = "insert into MASTER.tbUDUnitInfo (vCompanyId,vUnitId,vUnitName,vAddress,vPhone,vMobileNo,vEmail,vUnitType,vUnitInCharge,vInChargeMobile," +
                        "dStartFrom,isActive,vEditFlag,vUserName,dEntryTime) select vCompanyId,vUnitId,vUnitName,vAddress,vPhone,vMobileNo,vEmail,vUnitType,vUnitInCharge,vInChargeMobile," +
                        "dStartFrom,isActive,'Edit',vUserName,dEntryTime from MASTER.tbUnitInfo where vUnitID = '" + modelUnit.vUnitID + "'";
                    sqlQuery += "update MASTER.tbUnitInfo set vCompanyID = '" + modelUnit.vCompanyID + "', vUnitName = '" + modelUnit.vUnitName + "', "+
                        "vAddress = '" + modelUnit.vAddress + "',vPhone = '" + modelUnit.vPhone + "', vMobileNo = '" + modelUnit.vMobileNo + "', vEmail = '" + modelUnit.vEmail + "'," +
                        "vUnitType = '" + modelUnit.vUnitType + "', vUnitInCharge = '" + modelUnit.vUnitInCharge + "', vInChargeMobile = '" + modelUnit.vInChargeMobile + "',"+
                        "dStartFrom = convert(date,'" + modelUnit.dStartedDate + "',105), isActive = '" + modelUnit.vStatus + "', vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where "+
                        "vUnitID = '" + modelUnit.vUnitID + "'";
                    message = "All information updated successfully.";
                }
                else
                {
                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbUnitInfo (vCompanyId,vUnitId,vUnitName,vAddress,vPhone,vMobileNo,vEmail,vUnitType,vUnitInCharge,vInChargeMobile," +
                        "dStartFrom,isActive,vUserName,dEntryTime) values ('" + modelUnit.vCompanyID + "',(select 'U-' + CONVERT(varchar,ISNULL(MAX(CAST(SUBSTRING(vUnitId,3,LEN(vUnitID)) as int)),'0') + 1) from MASTER.tbUnitInfo),'" + modelUnit.vUnitName + "'," +
                    "'" + modelUnit.vAddress + "','" + modelUnit.vPhone + "','" + modelUnit.vMobileNo + "','" + modelUnit.vEmail + "','" + modelUnit.vUnitType + "'," +
                    "'" + modelUnit.vUnitInCharge + "','" + modelUnit.vInChargeMobile + "',convert(date,'" + modelUnit.dStartedDate + "',105),'" + modelUnit.vStatus + "'," +
                    "'" + Session["userName"] + "',GETDATE())";
                }
                
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
