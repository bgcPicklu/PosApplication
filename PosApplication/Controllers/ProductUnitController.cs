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
    public class ProductUnitController : Controller
    {
        //
        // GET: /ProductUnit/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<ProductUnitModel> lstUnitInfo = new List<ProductUnitModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vProductUnitAutoID,vProductUnitID,vProductUnit,vProductUnitShortName from MASTER.tbProductUnitInfo order by vProductUnit";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                ProductUnitModel unitModel = new ProductUnitModel();
                unitModel.vProductUnitAutoID = dataReader["vProductUnitAutoID"].ToString();
                unitModel.vProductUnitID = dataReader["vProductUnitID"].ToString();
                unitModel.vProductUnitName = dataReader["vProductUnit"].ToString();
                unitModel.vProductUnitShortName = dataReader["vProductUnitShortName"].ToString();

                lstUnitInfo.Add(unitModel);
            }
            connection.Close();
            return View(lstUnitInfo);
        }

        //
        // GET: /ProductUnit/Create
        public ActionResult Create(string unitID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            ProductUnitModel modelUnit = new ProductUnitModel();
            if (unitID == null)
            {
                unitID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vProductUnitID,4,LEN(vProductUnitID)) as int)),0)+1 vProductUnitID from MASTER.tbProductUnitInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelUnit.vProductUnitID = "PU-" + dataReader["vProductUnitID"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                unitID = unitID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vProductUnitAutoID,vProductUnitID,vProductUnit,vProductUnitShortName from MASTER.tbProductUnitInfo where vProductUnitAutoID = '" + unitID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelUnit.vProductUnitAutoID = dataReader["vProductUnitAutoID"].ToString();
                    modelUnit.vProductUnitID = dataReader["vProductUnitID"].ToString();
                    modelUnit.vProductUnitName = dataReader["vProductUnit"].ToString();
                    modelUnit.vProductUnitShortName = dataReader["vProductUnitShortName"].ToString();
                }
            }
            connection.Close();
            return View(modelUnit);
        }

        //
        // POST: /ProductUnit/Create
        [HttpPost]
        public ActionResult Create(ProductUnitModel modelUnit)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelUnit.vProductUnitAutoID != null)
                {
                    sqlQuery = "insert into MASTER.tbUDProductUnitInfo (vProductUnitAutoID,vProductUnitID,vProductUnit,vProductUnitShortName,vEditFlag,vUserName,dEntryTime) " +
                        "select vProductUnitAutoID,vProductUnitID,vProductUnit,vProductUnitShortName,'Edit',vUserName,dEntryTime from MASTER.tbProductUnitInfo where vProductUnitAutoID = '" + modelUnit.vProductUnitAutoID + "'";
                    sqlQuery += "update MASTER.tbProductUnitInfo set vProductUnitID = '" + modelUnit.vProductUnitID + "', vProductUnit = '" + modelUnit.vProductUnitName + "', vProductUnitShortName = '" + modelUnit.vProductUnitShortName + "'," +
                        "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vProductUnitAutoID = '" + modelUnit.vProductUnitAutoID + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbProductUnitInfo (vProductUnitAutoID,vProductUnitID,vProductUnit,vProductUnitShortName,vUserName,dEntryTime) values ('" + gu_ID.ToUpper() + "'," +
                    "'" + modelUnit.vProductUnitID + "','" + modelUnit.vProductUnitName + "','" + modelUnit.vProductUnitShortName + "','" + Session["userName"] + "',GETDATE())";
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
