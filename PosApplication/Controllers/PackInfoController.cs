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
    public class PackInfoController : Controller
    {
        //
        // GET: /SupplierInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<PackSizeInfo> lstPackInfo = new List<PackSizeInfo>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPackIDAuto,vPackId,vPackSize,isActive from MASTER.tbPackInfo order by vPackSize";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                PackSizeInfo packModel = new PackSizeInfo();
                packModel.vPackIDAuto = dataReader["vPackIDAuto"].ToString();
                packModel.vPackID = dataReader["vPackId"].ToString();
                packModel.vPackName = dataReader["vPackSize"].ToString();
                packModel.vStatus = dataReader["isActive"].ToString();

                lstPackInfo.Add(packModel);
            }
            connection.Close();
            return View(lstPackInfo);
        }

        //
        // GET: /SupplierInfo/Create
        public ActionResult Create(string packID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            PackSizeInfo modelPack = new PackSizeInfo();
            if (packID == null)
            {
                packID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vPackId,6,LEN(vPackId)) as int)),0)+1 vPackId from MASTER.tbPackInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelPack.vPackID = "PACK-" + dataReader["vPackId"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                packID = packID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vPackIDAuto,vPackId,vPackSize,isActive from MASTER.tbPackInfo where vPackIdAuto = '" + packID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelPack.vPackIDAuto = dataReader["vPackIDAuto"].ToString();
                    modelPack.vPackID = dataReader["vPackId"].ToString();
                    modelPack.vPackName = dataReader["vPackSize"].ToString();
                    modelPack.vStatus = dataReader["isActive"].ToString();
                }
            }
            connection.Close();
            return View(modelPack);
        }

        //
        // POST: /SupplierInfo/Create
        [HttpPost]
        public ActionResult Create(PackSizeInfo modelPack)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelPack.vPackIDAuto != null)
                {
                    sqlQuery = "insert into MASTER.tbUDPackInfo (vPackIdAuto,vPackId,vPackSize,vEditFlag,vUserName,dEntryTime,isActive) "+
                        "select vPackIdAuto,vPackId,vPackSize,'Edit',vUserName,dEntryTime,isActive from MASTER.tbPackInfo where vPackIdAuto = '" + modelPack.vPackIDAuto + "'";
                    sqlQuery += "update MASTER.tbPackInfo set vPackId = '" + modelPack.vPackID + "', vPackSize = '" + modelPack.vPackName + "', isActive = '" + modelPack.vStatus + "'," +
                        "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vPackIdAuto = '" + modelPack.vPackIDAuto + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbPackInfo (vPackIdAuto,vPackId,vPackSize,vUserName,dEntryTime,isActive) values ('" + gu_ID.ToUpper() + "'," +
                    "'" + modelPack.vPackID + "','" + modelPack.vPackName + "','" + Session["userName"] + "',GETDATE(),'" + modelPack.vStatus + "')";
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
