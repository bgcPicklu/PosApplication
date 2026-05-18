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
    public class SalesInformationController : Controller
    {
        public JsonResult GetAllInvoice(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<SalesMasterModel> lstInvoice = new List<SalesMasterModel>();
            string sqlQuery = "select vInvoiceNo,vChallanNo from POS.tbSalesMaster where dInvoiceDate = CONVERT(date,'" + dDate + "',103) and vUnitID = '" + Session["UnitName"] + "' order by vChallanNo desc";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                SalesMasterModel modelMaster = new SalesMasterModel();
                modelMaster.vInvoiceNo = reader["vInvoiceNo"].ToString();
                modelMaster.vChallanNo = reader["vChallanNo"].ToString();
                lstInvoice.Add(modelMaster);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult { Data = lstInvoice, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetAllProduct(string supplierID, string packID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<ProductInfoModel> lstProduct = new List<ProductInfoModel>();
            string sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vSupplierId like '" + supplierID + "' and vPackId like '" + packID + "' and isActive = 'Active' order by vProductName";
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

        public JsonResult GetAllProductName(string product)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<ProductInfoModel> lstProduct = new List<ProductInfoModel>();
            string sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vProductName like '" + product + "%' and isActive = 'Active' order by vProductName";
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

        public JsonResult GetProductData(string vProductID, string dSalesDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPackId,vLargeUnit,vSmallUnit,(select [POS].StockQty(vProductID,Convert(date,'" + dSalesDate + "', 103),'" + Session["UnitName"] + "')) cStock,(select " +
            "[POS].PurchaseRateCalculation(vProductID,Convert(date,'" + dSalesDate + "', 103),'" + Session["UnitName"] + "')) PurchaseRate,mSalesRate from MASTER.tbProductInfo where vProductId = '" + vProductID + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            var packID = "";
            var largeUnit = "";
            var smallUnit = "";
            var currentStock = "";
            var purchaseRate = "";
            var salesRate = "";
            while (reader.Read())
            {
                ProductInfoModel modelProduct = new ProductInfoModel();
                packID = reader["vPackId"].ToString();
                largeUnit = reader["vLargeUnit"].ToString();
                smallUnit = reader["vSmallUnit"].ToString();
                currentStock = reader["cStock"].ToString();
                purchaseRate = reader["PurchaseRate"].ToString();
                salesRate = reader["mSalesRate"].ToString();
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return Json(new { packID = packID, largeUnit = largeUnit, smallUnit = smallUnit, currentStock = currentStock, purchaseRate = purchaseRate, salesRate = salesRate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDataForEdit(string dDate, string InvoiceNo, string vChallanNo, string ReceiverName, string SoldBy)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string checkEdit = "select count(*) count_Row from INFO.tbUserAuthentication where vUnitID = '" + Session["UnitName"] + "' and vUserName = '" + Session["userName"] + "' and MenuID = 'SalesInfo' and iEdit = 1";
            SqlCommand command = new SqlCommand(checkEdit, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            SalesMasterModel masterModel = new SalesMasterModel();
            List<SalesDetailsModel> lstDetailModel = new List<SalesDetailsModel>();
            int chkCount = 0;
            while (dataRead.Read())
            {
                chkCount = Convert.ToInt16(dataRead["count_Row"].ToString());
            }
            dataRead.Close();
            dataRead.Dispose();
            if (chkCount == 1 || Session["AccountType"].ToString().Equals("Super Admin") || Session["AccountType"].ToString().Equals("Admin"))
            {
                string sqlQuery = "select count(*) countCheck from POS.tbSalesMaster where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                int count = 0;
                command.CommandText = sqlQuery;
                dataRead = command.ExecuteReader();
                while (dataRead.Read())
                {
                    count = Convert.ToInt16(dataRead["countCheck"].ToString());
                }
                dataRead.Close();
                dataRead.Dispose();
                if (count > 0)
                {
                    sqlQuery = "select sm.vInvoiceNo,sm.vInvoiceNoAuto,sm.vChallanNo,Convert(varchar,sm.dInvoiceDate,105) dInvoiceDate,sm.vReceiverName,sm.vSoldBy,"+
                        "sm.mNetAmount,sd.vProductId,pin.vProductName,pai.vPackSize,pin.vLargeUnit,pin.vSmallUnit,sd.vMaxUnit,sd.vMinUnit,sd.mSalesQty,sd.mSalesRate,"+
                        "sd.mAmount,(select [POS].StockQty(sd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) cStock,(select [POS].PurchaseRateCalculation(sd.vProductId,"+
                        "GETDATE(),'" + Session["UnitName"] + "')) PurchaseRate from POS.tbSalesMaster sm inner join POS.tbSalesDetails sd on sm.vInvoiceNo = sd.vInvoiceNo "+
                        "and sm.vUnitId = sd.vUnitId inner join MASTER.tbProductInfo pin on sd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pai on "+
                        "pin.vPackId = pai.vPackId where sm.vChallanNo = '" + vChallanNo + "' and sm.vUnitId = '" + Session["UnitName"] + "'";
                    command.CommandText = sqlQuery;
                    dataRead = command.ExecuteReader();
                    int i = 0;
                    while (dataRead.Read())
                    {
                        if (i == 0)
                        {
                            masterModel.dInvoiceDate = dataRead["dInvoiceDate"].ToString();
                            masterModel.vInvoiceNo = dataRead["vInvoiceNo"].ToString();
                            masterModel.vInvoiceNoAuto = dataRead["vInvoiceNoAuto"].ToString();
                            masterModel.vChallanNo = dataRead["vChallanNo"].ToString();
                            masterModel.vReceiverName = dataRead["vReceiverName"].ToString();
                            masterModel.vSoldBy = dataRead["vSoldBy"].ToString();
                            masterModel.mNetAmount = dataRead["mNetAmount"].ToString();
                        }

                        SalesDetailsModel detailsModel = new SalesDetailsModel();
                        detailsModel.vProductID = dataRead["vProductId"].ToString();
                        detailsModel.vProductName = dataRead["vProductName"].ToString();
                        detailsModel.vPackSize = dataRead["vPackSize"].ToString();
                        detailsModel.mMaxQty = Convert.ToDecimal(dataRead["vLargeUnit"].ToString());
                        detailsModel.mMinQty = Convert.ToDecimal(dataRead["vSmallUnit"].ToString());
                        detailsModel.mCurrentStock = Convert.ToDecimal(dataRead["cStock"].ToString());
                        detailsModel.mLuQty = Convert.ToDecimal(dataRead["vMaxUnit"].ToString());
                        detailsModel.mSuQty = Convert.ToDecimal(dataRead["vMinUnit"].ToString());
                        detailsModel.mSalesQty = Convert.ToDecimal(dataRead["mSalesQty"].ToString());
                        detailsModel.mPurchaseRate = Convert.ToDecimal(dataRead["PurchaseRate"].ToString());
                        detailsModel.mSalesRate = Convert.ToDecimal(dataRead["mSalesRate"].ToString());
                        detailsModel.mAmount = Convert.ToDecimal(dataRead["mAmount"].ToString());
                        lstDetailModel.Add(detailsModel);
                        i++;
                    }
                    dataRead.Close();
                    dataRead.Dispose();
                }
                else
                {
                    masterModel.dInvoiceDate = dDate;
                    masterModel.vInvoiceNo = InvoiceNo;
                    masterModel.vChallanNo = vChallanNo;
                    masterModel.vReceiverName = ReceiverName;
                    masterModel.vSoldBy = SoldBy;
                }
            }
            else
            {
                masterModel.dInvoiceDate = dDate;
                masterModel.vInvoiceNo = InvoiceNo;
                masterModel.vChallanNo = vChallanNo;
                masterModel.vReceiverName = ReceiverName;
                masterModel.vSoldBy = SoldBy;
            }

            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailModel }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkBillNo(string challanNo, string InvoiceNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select count(*) countBill from POS.tbSalesMaster where vChallanNo = '" + challanNo + "' and vInvoiceNo != '" + InvoiceNo + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            var success = false;
            int countRow = 0;
            while (dataReader.Read())
            {
                countRow = Convert.ToInt16(dataReader["countBill"].ToString());
            }
            dataReader.Close();
            dataReader.Dispose();

            if (countRow > 0)
            {
                success = true;
            }
            connection.Close();
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PurchaserInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<IssueMasterModel> masterList = new List<IssueMasterModel>();
            string sqlQuery = "select CONVERT(varchar,dPurchaseDate,105) dPurchaseDate,vReferenceNo,vChallanNo,mTotalAmount,mTotalVatAmount,(mTotalAmount + mTotalVatAmount) VATWithAmount,mTotalDiscount,mNetAmount from POS.tbPurchaseReceiveMaster Order by dPurchaseDate,vChallanNo";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader readData = command.ExecuteReader();
            while (readData.Read())
            {
                IssueMasterModel modelMaster = new IssueMasterModel();
                modelMaster.dIssueDate = readData["dPurchaseDate"].ToString();
                modelMaster.vReferenceNo = readData["vReferenceNo"].ToString();
                modelMaster.vIssueNo = readData["vChallanNo"].ToString();
                modelMaster.mTotalAmount = Convert.ToDouble(readData["mTotalAmount"].ToString());

                masterList.Add(modelMaster);
            }
            readData.Close();
            readData.Dispose();
            connection.Close();
            return View(masterList);
        }

        public JsonResult getReferenceNo(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            IssueMasterModel modelMaster = new IssueMasterModel();
            string sqlQuery = "select 'INV-' + CONVERT(VARCHAR(4),YEAR(CONVERT(date,'" + dDate + "',103))) + '/' + CONVERT(VARCHAR(2),MONTH(CONVERT(date,'" + dDate + "',103))) + '-' + CONVERT(VARCHAR,ISNULL(MAX(CAST(SUBSTRING(vInvoiceNo,12,LEN(vInvoiceNo)) as int)),0) + 1) vInvoiceNo from POS.tbSalesMaster where MONTH(dInvoiceDate) = MONTH(CONVERT(date,'" + dDate + "',103)) and vUnitID = '" + Session["UnitName"] + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader readData = command.ExecuteReader();
            var vInvoiceNo = "";
            while (readData.Read())
            {
                vInvoiceNo = readData["vInvoiceNo"].ToString();
            }
            readData.Close();
            readData.Dispose();
            connection.Close();
            return Json(new { vInvoiceNo = vInvoiceNo }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PurchaserInfo/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SalesMasterModel modelMaster = new SalesMasterModel();
            modelMaster.vSoldBy = Session["userName"].ToString();
            return View(modelMaster);
        }

        //
        // POST: /PurchaserInfo/Create
        [HttpPost]
        public JsonResult Create(SalesMasterModel masterModel, SalesDetailsList detailsListModel)
        {
            try
            {
                // TODO: Add insert logic here
                //select vUnitId,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,vUserName,dEntryTime from POS.tbPurchaseReceiveMaster
                //select dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount,mNetAmount,mPurchaseRate,mSalesRate from POS.tbPurchaseReceiveDetails
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                var message = "All information saved successfully";
                string sqlQuery = "insert into POS.tbSalesMaster (vUnitId,vInvoiceNoAuto,vInvoiceNo,vChallanNo,dInvoiceDate,vReceiverName,vCustomerName,vSoldBy,mNetAmount,"+
                    "vUserName,dEntryTime,iStatus) values ('" + Session["UnitName"] + "','" + masterModel.vInvoiceNo + "','" + masterModel.vInvoiceNo + "'," +
                    "'" + masterModel.vChallanNo + "',CONVERT(date,'" + masterModel.dInvoiceDate + "',103),'" + masterModel.vReceiverName + "',''," +
                    "'" + masterModel.vSoldBy + "','" + masterModel.mNetAmount + "','" + Session["userName"] + "',GETDATE(),1)";

                if (masterModel.vInvoiceNoAuto != null)
                {
                    message = "All information updated successfully";
                    sqlQuery = "insert into POS.tbUDSalesMaster (vUnitId,vInvoiceNoAuto,vInvoiceNo,vChallanNo,dInvoiceDate,vReceiverName,vCustomerName,vSoldBy,mNetAmount," +
                    "vUserName,dEntryTime,vEditFlag,iStatus) select vUnitId,vInvoiceNoAuto,vInvoiceNo,vChallanNo,dInvoiceDate,vReceiverName,vCustomerName,vSoldBy,mNetAmount," +
                    "vUserName,dEntryTime,'Edit',iStatus from POS.tbSalesMaster where vInvoiceNo = '" + masterModel.vInvoiceNo + "' and vUnitId = '" + Session["UnitName"] + "'";

                    sqlQuery += "insert into POS.tbUDSalesDetails (vUnitId,vInvoiceNo,vProductId,vMaxUnit,vMinUnit,dExpDate,mSalesQty,mPurchaseRate,mSalesRate,mAmount,"+
                        "vEditFlag) select vUnitId,vInvoiceNo,vProductId,vMaxUnit,vMinUnit,dExpDate,mSalesQty,mPurchaseRate,mSalesRate,mAmount,'Edit' from " +
                        "POS.tbSalesDetails where vInvoiceNo = '" + masterModel.vInvoiceNo + "' and vUnitId = '" + Session["UnitName"] + "'";

                    sqlQuery += "Delete from POS.tbSalesDetails where vInvoiceNo = '" + masterModel.vInvoiceNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                    sqlQuery += "update POS.tbSalesMaster set dInvoiceDate = CONVERT(date,'" + masterModel.dInvoiceDate + "',103)," +
                    "vChallanNo = '" + masterModel.vChallanNo + "',vReceiverName = '" + masterModel.vReceiverName + "',vSoldBy = '" + masterModel.vSoldBy + "'," +
                    "mNetAmount = '" + masterModel.mNetAmount + "',vUserName = '" + Session["userName"] + "',dEntryTime = GETDATE() where " +
                    "vInvoiceNo = '" + masterModel.vInvoiceNo + "' and vUnitId = '" + Session["UnitName"] + "'";
                }

                sqlQuery += "insert into POS.tbSalesDetails (vUnitId,vInvoiceNo,vProductId,vMaxUnit,vMinUnit,mSalesQty,mPurchaseRate,mSalesRate,mAmount) values ";
                foreach (var getData in detailsListModel.lstSalesDetails)
                {
                    sqlQuery += "('" + Session["UnitName"] + "','" + masterModel.vInvoiceNo + "','" + getData.vProductID + "','" + getData.mLuQty + "'," +
                    "'" + getData.mSuQty + "','" + getData.mSalesQty + "','" + getData.mPurchaseRate + "','" + getData.mSalesRate + "','" + getData.mAmount + "'),";
                }

                SqlCommand command = new SqlCommand(sqlQuery.Substring(0, sqlQuery.Length - 1), connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getSalesReceivedData(string vChallanNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            SalesMasterModel masterModel = new SalesMasterModel();
            List<SalesDetailsModel> lstDetailsModel = new List<SalesDetailsModel>();
            string sqlQuery = "select CONVERT(varchar,prm.dInvoiceDate,105) dInvoiceDate,prm.vInvoiceNo,prm.vChallanNo,prm.vSoldBy,prm.vReceiverName,prd.vProductId," +
                "pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) cStock,"+
                "mPurchaseRate,prd.vMaxUnit,prd.vMinUnit,prd.mSalesRate from POS.tbSalesMaster prm inner join POS.tbSalesDetails prd on prm.vInvoiceNo = prd.vInvoiceNo "+
                "inner join MASTER.tbProductInfo pin on prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId inner join "+
                "MASTER.tbUnitInfo ui on ui.vUnitId = prm.vUnitId where prm.vInvoiceNo = '" + vChallanNo + "' and prm.iStatus = 1 and prm.iReturnStatus != 1";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            int i = 0;
            while (dataRead.Read())
            {
                if (i == 0)
                {
                    masterModel.dInvoiceDate = dataRead["dInvoiceDate"].ToString();
                    masterModel.vInvoiceNo = dataRead["vInvoiceNo"].ToString();
                    masterModel.vChallanNo = dataRead["vChallanNo"].ToString();
                    masterModel.vSoldBy = dataRead["vSoldBy"].ToString();
                    masterModel.vReceiverName = dataRead["vReceiverName"].ToString();
                }

                SalesDetailsModel detailsModel = new SalesDetailsModel();
                detailsModel.vProductID = dataRead["vProductId"].ToString();
                detailsModel.vProductName = dataRead["vProductName"].ToString();
                detailsModel.vPackSize = dataRead["vPackSize"].ToString();
                detailsModel.mMaxQty = Convert.ToDecimal(dataRead["vLargeUnit"].ToString());
                detailsModel.mMinQty = Convert.ToDecimal(dataRead["vSmallUnit"].ToString());
                detailsModel.mCurrentStock = Convert.ToDecimal(dataRead["cStock"].ToString());
                detailsModel.mPurchaseRate = Convert.ToDecimal(dataRead["mPurchaseRate"].ToString());
                detailsModel.mLuQty = Convert.ToDecimal(dataRead["vMaxUnit"].ToString());
                detailsModel.mSuQty = Convert.ToDecimal(dataRead["vMinUnit"].ToString());
                detailsModel.mSalesRate = Convert.ToDecimal(dataRead["mSalesRate"].ToString());
                lstDetailsModel.Add(detailsModel);
                i++;
            }
            dataRead.Close();
            dataRead.Dispose();
            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailsModel }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalesReturn()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult SalesReturn(SalesDetailsModel masterModel, SalesDetailsList detailsListModel)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "update POS.tbSalesMaster set iReturnStatus = 1 where vInvoiceNo = '" + masterModel.vInvoiceNo + "' and vUnitID = '" + Session["UnitName"] + "'";
            foreach (var getdata in detailsListModel.lstSalesDetails)
            {
                sqlQuery += "update POS.tbSalesDetails set nReturnSalesQty = '" + getdata.mReturnSalesQty + "' where vInvoiceNo = '" + masterModel.vInvoiceNo + "' and " +
                    "vUnitID = '" + Session["UnitName"] + "' and vProductId = '" + getdata.vProductID + "' ";
            }
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
            return Json(new { success = true, message = "All Information Saved Successfully" }, JsonRequestBehavior.AllowGet);
        }

        //Sales Invoice
        public ActionResult SalesInvoiceReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult SalesInvoiceReport(string vBillNo)
        {
            if (Session["userName"] != null)
            {
                Session["vBillNo"] = vBillNo;
                Session["ReportName"] = "Sales Invoice";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult SalesSummary()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult SalesSummary(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Sales Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult ProductWiseSalesSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWiseSalesSummaryReport(string vProductID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Product Wise Sales Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult InvoiceWiseSalesSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWiseSalesSummaryReport(string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Invoice Wise Sales Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Invoice Wise Profit
        public ActionResult InvoiceWiseProfit()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWiseProfit(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Invoice Wise Profit Report";
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
