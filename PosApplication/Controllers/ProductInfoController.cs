using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using PosApplication.Models;
using System.IO;

namespace PosApplication.Controllers
{
    public class ProductInfoController : Controller
    {
        public JsonResult GetAllProduct(string categoryID, string packID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<ProductInfoModel> lstProduct = new List<ProductInfoModel>();
            string sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vCategoryID like '" + categoryID + "' and vPackId like '" + packID + "' and isActive = 'Active' order by vProductName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ProductInfoModel modelProduct = new ProductInfoModel();
                modelProduct.vProductID = reader["vProductID"].ToString();
                modelProduct.vProductName = reader["vProductName"].ToString();
                lstProduct.Add(modelProduct);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult { Data = lstProduct, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /SupplierInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<ProductInfoModel> lstProductInfo = new List<ProductInfoModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vProductAutoID,vProductID,vProductManualCode,vProductName,pui.vProductUnitShortName,proin.vCategoryID,cin.vCategoryName,proin.vPackId,pain.vPackSize,proin.vRackId,Rin.vRackName," +
                              "vMaxLevel,vMinLevel,vReOrderLevel,vLargeUnit,vSmallUnit,vTotalQty,mSalesRate,proin.isActive,vProductImage from MASTER.tbProductInfo proin inner join "+
                              "MASTER.tbPackInfo pain on proin.vPackId = pain.vPackId inner join MASTER.tbRackInfo Rin on proin.vRackId = Rin.vRackId inner join MASTER.tbCategoryInformation cin "+
                              "on cin.vCategoryID = proin.vCategoryID inner join MASTER.tbProductUnitInfo pui on proin.vProductUnit = pui.vProductUnitID order by vProductName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                ProductInfoModel productModel = new ProductInfoModel();
                productModel.vProductAutoID = dataReader["vProductAutoID"].ToString();
                productModel.vProductID = dataReader["vProductID"].ToString();
                productModel.vProductManualCode = dataReader["vProductManualCode"].ToString();
                productModel.vProductName = dataReader["vProductName"].ToString();
                productModel.vProductUnit = dataReader["vProductUnitShortName"].ToString();
                productModel.vCategoryID = dataReader["vCategoryID"].ToString();
                productModel.vCategoryName = dataReader["vCategoryName"].ToString();
                productModel.vPackId = dataReader["vPackId"].ToString();
                productModel.vPackSize = dataReader["vPackSize"].ToString();
                productModel.vRackId = dataReader["vRackId"].ToString();
                productModel.vRackSize = dataReader["vRackName"].ToString();
                productModel.vMinLevel = Convert.ToDouble(dataReader["vMinLevel"].ToString());
                productModel.vLargeUnit = Convert.ToDouble(dataReader["vLargeUnit"].ToString());
                productModel.vSmallUnit = Convert.ToDouble(dataReader["vSmallUnit"].ToString());
                productModel.vTotalQty = Convert.ToDouble(dataReader["vTotalQty"].ToString());
                productModel.mSalesRate = dataReader["mSalesRate"].ToString();
                productModel.vStatus = dataReader["isActive"].ToString();
                productModel.vProductImage = dataReader["vProductImage"].ToString();

                lstProductInfo.Add(productModel);
            }
            connection.Close();
            return View(lstProductInfo);
        }

        //
        // GET: /SupplierInfo/Create
        public ActionResult Create(string productID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            ProductInfoModel modelProduct = new ProductInfoModel();
            if (productID == null)
            {
                productID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vProductID,5,LEN(vProductID)) as int)),0)+1 vProductID from MASTER.tbProductInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelProduct.vProductID = "PRO-" + dataReader["vProductID"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                productID = productID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vProductAutoID,vProductID,vProductManualCode,vProductName,vProductUnit,vCategoryID,vPackId,vRackId,vMaxLevel,vMinLevel,vReOrderLevel,vLargeUnit," +
                    "vSmallUnit,vTotalQty,mSalesRate,isActive,vProductImage from MASTER.tbProductInfo where vProductAutoID = '" + productID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelProduct.vProductAutoID = dataReader["vProductAutoID"].ToString();
                    modelProduct.vProductID = dataReader["vProductID"].ToString();
                    modelProduct.vProductManualCode = dataReader["vProductManualCode"].ToString();
                    modelProduct.vProductName = dataReader["vProductName"].ToString();
                    modelProduct.vProductUnit = dataReader["vProductUnit"].ToString();
                    modelProduct.vCategoryID = dataReader["vCategoryID"].ToString();
                    modelProduct.vPackId = dataReader["vPackId"].ToString();
                    modelProduct.vRackId = dataReader["vRackId"].ToString();
                    modelProduct.vMinLevel = Convert.ToDouble(dataReader["vMinLevel"].ToString());
                    modelProduct.vLargeUnit = Convert.ToDouble(dataReader["vLargeUnit"].ToString());
                    modelProduct.vSmallUnit = Convert.ToDouble(dataReader["vSmallUnit"].ToString());
                    modelProduct.vTotalQty = Convert.ToDouble(dataReader["vTotalQty"].ToString());
                    modelProduct.mSalesRate = dataReader["mSalesRate"].ToString();
                    modelProduct.vStatus = dataReader["isActive"].ToString();
                    modelProduct.vProductImage = dataReader["vProductImage"].ToString();
                }
            }
            connection.Close();
            return View(modelProduct);
        }

        //
        // POST: /SupplierInfo/Create
        [HttpPost]
        public ActionResult Create(ProductInfoModel modelProduct)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                string imagePath = modelProduct.vProductName;
                string imageExtension = "";
                if (modelProduct.productImageUpload != null)
                {
                    imageExtension = Path.GetExtension(modelProduct.productImageUpload.FileName);
                    imagePath = (imagePath.Replace("/","")).Replace("'","") + imageExtension;
                    modelProduct.vProductImage = "~/Content/image/" + imagePath;
                    modelProduct.productImageUpload.SaveAs(Server.MapPath("~/Content/image/" + imagePath));
                }
                else
                {
                    modelProduct.vProductImage = "";
                }

                var message = "All information saved successfully.";
                if (modelProduct.vProductAutoID != null)
                {
                    sqlQuery = "insert into MASTER.tbUDProductInfo (vProductAutoID,vProductManualCode,vProductID,vProductName,vProductUnit,vCategoryID,vPackId,vRackId,vMinLevel,vLargeUnit,vSmallUnit," +
                    "vTotalQty,mSalesRate,vEditFlag,isActive,vProductImage,vUserName,dEntryTime) select vProductAutoID,vProductManualCode,vProductID,vProductName,vProductUnit,vCategoryID,vPackId," +
                    "vRackId,vMinLevel,vLargeUnit,vSmallUnit,vTotalQty,mSalesRate,'Edit',isActive,vProductImage,vUserName,dEntryTime from MASTER.tbProductInfo where "+
                    "vProductAutoID = '" + modelProduct.vProductAutoID + "'";
                    sqlQuery += "update MASTER.tbProductInfo set vProductID = '" + modelProduct.vProductID + "',vProductManualCode = '" + modelProduct.vProductManualCode + "',vProductName = '" + modelProduct.vProductName + "',"+
                        "vProductUnit = '" + modelProduct.vProductUnit + "', vCategoryID = '" + modelProduct.vCategoryID + "',vPackId = '" + modelProduct.vPackId + "', vRackId = '" + modelProduct.vRackId + "', vMinLevel = '" + modelProduct.vMinLevel + "'," +
                        "vLargeUnit = '" + modelProduct.vLargeUnit + "', vSmallUnit = '" + modelProduct.vSmallUnit + "',vTotalQty = '" + modelProduct.vTotalQty + "',"+
                        "mSalesRate = '" + modelProduct.mSalesRate + "',isActive = '" + modelProduct.vStatus + "',";
                    if (modelProduct.vProductImage != "")
                    {
                        sqlQuery += "vProductImage = '" + modelProduct.vProductImage + "',";
                    }
                    sqlQuery += "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vProductAutoID = '" + modelProduct.vProductAutoID + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbProductInfo (vProductAutoID,vProductID,vProductManualCode,vProductName,vProductUnit,vCategoryID,vPackId,vRackId,vMinLevel,vLargeUnit,vSmallUnit," +
                    "vTotalQty,mSalesRate,isActive,vProductImage,vUserName,dEntryTime) values ('" + gu_ID.ToUpper() + "','" + modelProduct.vProductID + "','" + modelProduct.vProductManualCode + "','" + modelProduct.vProductName + "'," +
                    "'" + modelProduct.vProductUnit + "','" + modelProduct.vCategoryID + "','" + modelProduct.vPackId + "','" + modelProduct.vRackId + "','" + modelProduct.vMinLevel + "','" + modelProduct.vLargeUnit + "'," +
                    "'" + modelProduct.vSmallUnit + "','" + modelProduct.vTotalQty + "','" + modelProduct.mSalesRate + "','" + modelProduct.vStatus + "','" + modelProduct.vProductImage + "',"+
                    "'" + Session["userName"] + "',GETDATE())";
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

        //Product Wise Stock Register
        public ActionResult Report()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult Report(string vProductID, string vFromDate)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["vFromDate"] = vFromDate;
                Session["ReportName"] = "Product Wise Stock Register";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Product Information
        public ActionResult ProductInfoReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductInfoReport(string vProductID, string vCategoryID, string vPackID)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["vCategoryID"] = vCategoryID;
                Session["vPackID"] = vPackID;
                Session["ReportName"] = "Product Information";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Stock Summary
        public ActionResult StockRegister()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult StockRegister(string vUnitID, string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vUnitID"] = vUnitID;
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Stock Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Short Stock
        public ActionResult ShortStock()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ShortStock(int? id)
        {
            if (Session["userName"] != null)
            {
                Session["ReportName"] = "Short Stock Report";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Product Wise Profit
        public ActionResult ProductWiseProfit()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWiseProfit(string vProductID, string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Product Wise Profit Report";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Date Wise Profit
        public ActionResult DateWiseProfit()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult DateWiseProfit(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Date Wise Profit Report";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Profit & Loss Statement Unit Wise
        public ActionResult ProfitAndLossStatementUnitWise()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProfitAndLossStatementUnitWise(string vUnitID, string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vUnitID"] = vUnitID;
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Profit & Loss Statement (Unit Wise)";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Profit & Loss Statement
        public ActionResult ProfitAndLossStatement()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProfitAndLossStatement(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Profit & Loss Statement";
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
