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
    public class TradeOfferController : Controller
    {
        //
        // GET: /TradeOffer/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<TradeOfferModel> lstTradeOfferInfo = new List<TradeOfferModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select tof.vProductID,pin.vProductName,tof.nShortUnitQty,tof.nAmount from MASTER.tbTradeOffer tof inner join MASTER.tbProductInfo pin on tof.vProductID = pin.vProductId Order By pin.vProductName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                TradeOfferModel tradeOfferModel = new TradeOfferModel();
                tradeOfferModel.vProductID = dataReader["vProductUnitAutoID"].ToString();
                tradeOfferModel.vProductName = dataReader["vProductName"].ToString();
                tradeOfferModel.nShortUnitQty = Convert.ToDecimal(dataReader["nShortUnitQty"].ToString());
                tradeOfferModel.nAmount = Convert.ToDecimal(dataReader["nAmount"].ToString());

                lstTradeOfferInfo.Add(tradeOfferModel);
            }
            connection.Close();
            return View(lstTradeOfferInfo);
        }

        //
        // GET: /TradeOffer/Create
        public ActionResult Create(string ProductID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            if (ProductID != null)
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                TradeOfferModel tradeOfferModel = new TradeOfferModel();
                ProductID = ProductID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select tof.vProductID,pin.vProductName,tof.nShortUnitQty,tof.nAmount from MASTER.tbTradeOffer tof inner join MASTER.tbProductInfo pin on tof.vProductID = pin.vProductId  where vProductID = '" + ProductID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    tradeOfferModel.vProductID = dataReader["vProductID"].ToString();
                    tradeOfferModel.vProductName = dataReader["vProductName"].ToString();
                    tradeOfferModel.nShortUnitQty = Convert.ToDecimal(dataReader["nShortUnitQty"].ToString());
                    tradeOfferModel.nAmount = Convert.ToDecimal(dataReader["nAmount"].ToString());
                }
                connection.Close();
                return View(tradeOfferModel);
            }
            return View();
        }

        //
        // POST: /TradeOffer/Create
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
