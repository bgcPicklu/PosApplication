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
    public class RackInfoController : Controller
    {
        //
        // GET: /SupplierInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<RackInfoModel> lstRackInfo = new List<RackInfoModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vRackIdAuto,vRackId,vRackName,isActive from MASTER.tbRackInfo order by vRackName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                RackInfoModel rackModel = new RackInfoModel();
                rackModel.vRackIDAuto = dataReader["vRackIdAuto"].ToString();
                rackModel.vRackID = dataReader["vRackId"].ToString();
                rackModel.vRackName = dataReader["vRackName"].ToString();
                rackModel.vStatus = dataReader["isActive"].ToString();

                lstRackInfo.Add(rackModel);
            }
            connection.Close();
            return View(lstRackInfo);
        }

        //
        // GET: /SupplierInfo/Create
        public ActionResult Create(string rackID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            RackInfoModel modelrack = new RackInfoModel();
            if (rackID == null)
            {
                rackID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vRackId,6,LEN(vRackId)) as int)),0)+1 vRackId from MASTER.tbRackInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelrack.vRackID = "RACK-" + dataReader["vRackId"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                rackID = rackID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vRackIdAuto,vRackId,vRackName,isActive from MASTER.tbRackInfo where vRackIdAuto = '" + rackID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelrack.vRackIDAuto = dataReader["vRackIdAuto"].ToString();
                    modelrack.vRackID = dataReader["vRackId"].ToString();
                    modelrack.vRackName = dataReader["vRackName"].ToString();
                    modelrack.vStatus = dataReader["isActive"].ToString();
                }
            }
            connection.Close();
            return View(modelrack);
        }

        //
        // POST: /SupplierInfo/Create
        [HttpPost]
        public ActionResult Create(RackInfoModel modelrack)
        {
            try
            {//select vRackIdAuto,vRackId,vRackName,isActive,vUserName,dEntryTime from MASTER.tbRackInfo
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelrack.vRackIDAuto != null)
                {
                    sqlQuery = "insert into MASTER.tbUDRackInfo (vRackIdAuto,vRackId,vRackName,vEditFlag,isActive,vUserName,dEntryTime) select vRackIdAuto,vRackId,"+
                        "vRackName,'Edit',isActive,vUserName,dEntryTime from MASTER.tbRackInfo where vRackIdAuto = '" + modelrack.vRackIDAuto + "'";
                    sqlQuery += "update MASTER.tbRackInfo set vRackId = '" + modelrack.vRackID + "', vRackName = '" + modelrack.vRackName + "', isActive = '" + modelrack.vStatus + "'," +
                        "vUserName = '', dEntryTime = GETDATE() where vRackIdAuto = '" + modelrack.vRackIDAuto + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbRackInfo (vRackIdAuto,vRackId,vRackName,isActive,vUserName,dEntryTime) values ('" + gu_ID.ToUpper() + "'," +
                    "'" + modelrack.vRackID + "','" + modelrack.vRackName + "','" + modelrack.vStatus + "','',GETDATE())";
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
