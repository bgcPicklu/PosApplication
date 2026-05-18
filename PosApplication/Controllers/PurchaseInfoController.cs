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
    public class PurchaseInfoController : Controller
    {
        public JsonResult GetAllIssueNo(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<PurchaseMasterModel> lstInvoice = new List<PurchaseMasterModel>();
            string sqlQuery = "select vReferenceNo,vChallanNo from POS.tbPurchaseReceiveMaster where dPurchaseDate = CONVERT(date,'" + dDate + "',103) order by vChallanNo desc";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                PurchaseMasterModel modelMaster = new PurchaseMasterModel();
                modelMaster.vReferenceNo = reader["vReferenceNo"].ToString();
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
            List <ProductInfoModel> lstProduct = new List<ProductInfoModel>();
            string sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vSupplierId like '" + supplierID + "' and vPackId like '" + packID + "' and isActive = 'Active' order by vProductName";
            SqlCommand command = new SqlCommand(sqlQuery,connection);
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read()){
                ProductInfoModel modelProduct = new ProductInfoModel();
                modelProduct.vProductID = reader["vProductID"].ToString();
                modelProduct.vProductName = reader["vProductName"].ToString();
                lstProduct.Add(modelProduct);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult{ Data = lstProduct, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetProductData(string vProductID, string purchaseDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPackId,vLargeUnit,vSmallUnit,(select [POS].StockQty(vProductID, CONVERT(date, '" + purchaseDate + "', 103),'" + Session["UnitName"] + "')) current_Stock,mSalesRate from MASTER.tbProductInfo where vProductId = '" + vProductID + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            var packID = "";
            var largeUnit = "";
            var smallUnit = "";
            var currentStock = "";
            var salesRate = "";
            while (reader.Read())
            {
                ProductInfoModel modelProduct = new ProductInfoModel();
                packID = reader["vPackId"].ToString();
                largeUnit = reader["vLargeUnit"].ToString();
                smallUnit = reader["vSmallUnit"].ToString();
                currentStock = reader["current_Stock"].ToString();
                salesRate = reader["mSalesRate"].ToString();
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return Json(new { packID = packID, largeUnit = largeUnit, smallUnit = smallUnit, currentStock = currentStock, salesRate = salesRate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDataForEdit(string dDate, string ReferenceNo, string vatPercentage, string vChallanNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string checkEdit = "select count(*) count_Row from INFO.tbUserAuthentication where vUnitID = '" + Session["UnitName"] + "' and vUserName = '" + Session["userName"] + "' and MenuID = 'PurchaseInfo' and iEdit = 1";
            SqlCommand command = new SqlCommand(checkEdit, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            PurchaseMasterModel masterModel = new PurchaseMasterModel();
            List<PurchaseDetailsModel> lstDetailModel = new List<PurchaseDetailsModel>();
            int chkCount = 0;
            while(dataRead.Read())
            {
                chkCount = Convert.ToInt16(dataRead["count_Row"].ToString());
            }
            dataRead.Close();
            dataRead.Dispose();
            if (chkCount == 1 || Session["AccountType"].ToString().Equals("Super Admin") || Session["AccountType"].ToString().Equals("Admin"))
            {
                string sqlQuery = "select count(*) countCheck from POS.tbPurchaseReceiveMaster where vChallanNo = '" + vChallanNo + "'";
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
                    //sqlQuery = "select prm.vSupplierID,CONVERT(varchar,prm.dPurchaseDate,105) dPurchaseDate,prm.vReferenceNo,prm.mVatPercent,prm.mTotalVatAmount,prm.mTotalAmount,prm.mTotalDiscount," +
                    //   "prm.mNetAmount,(select ISNULL(SUM(ISNULL(mNetAmount,0) - ISNULL(nPaidAmount,0)),0) from POS.tbPurchaseReceiveMaster where vSupplierID = prm.vSupplierID and vChallanNo != '" + vChallanNo + "') + (select ISNULL(nPaidAmount,0) from " +
                    //   "POS.tbPurchaseReceiveMaster where vSupplierID = prm.vSupplierID and vChallanNo = '" + vChallanNo + "')  previousDues,ISNULL(prm.nPaidAmount,0) nPaidAmount,prd.vProductId," +
                    //   "pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) " +
                    //   "mCurrentStock,prd.mLuQty,prd.mSuQty,prd.mTotalQty,prd.mAmount,prd.mVat,prd.mDiscount,prd.mNetAmount mProductNetAmount,prd.mPurchaseRate,prd.mSalesRate from "+
                    //   "POS.tbPurchaseReceiveMaster prm inner join POS.tbPurchaseReceiveDetails prd on prm.vReferenceNo = prd.vReferenceNo inner join MASTER.tbProductInfo pin on "+
                    //   "prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId where vChallanNo = '" + vChallanNo + "'";
                    sqlQuery = "select prm.vSupplierID,CONVERT(varchar,prm.dPurchaseDate,105) dPurchaseDate,prm.vReferenceNo,prm.mVatPercent,prm.mTotalVatAmount,prm.mTotalAmount,prm.mTotalDiscount," +
                       "prm.mNetAmount,(select ISNULL(SUM(ISNULL(mNetAmount,0) - ISNULL(nPaidAmount,0)),0) from POS.tbPurchaseReceiveMaster where vSupplierID = prm.vSupplierID and vChallanNo != '" + vChallanNo + "') previousDues," +
                       "ISNULL(prm.nPaidAmount,0) nPaidAmount,prd.vProductId,pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) " +
                       "mCurrentStock,prd.mLuQty,prd.mSuQty,prd.mTotalQty,prd.mAmount,prd.mVat,prd.mDiscount,prd.mNetAmount mProductNetAmount,prd.mPurchaseRate,prd.mSalesRate from " +
                       "POS.tbPurchaseReceiveMaster prm inner join POS.tbPurchaseReceiveDetails prd on prm.vReferenceNo = prd.vReferenceNo inner join MASTER.tbProductInfo pin on " +
                       "prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId where vChallanNo = '" + vChallanNo + "'";
                    command.CommandText = sqlQuery;
                    dataRead = command.ExecuteReader();
                    int i = 0;
                    while (dataRead.Read())
                    {
                        if (i == 0)
                        {
                            masterModel.vSupplierID = dataRead["vSupplierID"].ToString();
                            masterModel.dPurchaseDate = dataRead["dPurchaseDate"].ToString();
                            masterModel.mPaidAmount = Convert.ToDouble(dataRead["nPaidAmount"].ToString());
                            masterModel.vReferenceNo = dataRead["vReferenceNo"].ToString();
                            masterModel.vReferenceNoAuto = dataRead["vReferenceNo"].ToString();
                            masterModel.mVatPercent = Convert.ToDouble(dataRead["mVatPercent"].ToString());
                            masterModel.mTotalVatAmount = Convert.ToDouble(dataRead["mTotalVatAmount"].ToString());
                            masterModel.mTotalAmount = Convert.ToDouble(dataRead["mTotalAmount"].ToString());
                            masterModel.mTotalDiscount = Convert.ToDouble(dataRead["mTotalDiscount"].ToString());
                            masterModel.mNetAmount = Convert.ToDouble(dataRead["mNetAmount"].ToString());
                            masterModel.mPreviousDues = Convert.ToDouble(dataRead["previousDues"].ToString());
                        }

                        PurchaseDetailsModel detailsModel = new PurchaseDetailsModel();
                        detailsModel.vProductID = dataRead["vProductId"].ToString();
                        detailsModel.vProductName = dataRead["vProductName"].ToString();
                        detailsModel.vPackName = dataRead["vPackSize"].ToString();
                        detailsModel.mMaxQty = Convert.ToDouble(dataRead["vLargeUnit"].ToString());
                        detailsModel.mMinQty = Convert.ToDouble(dataRead["vSmallUnit"].ToString());
                        detailsModel.mCurrentStock = Convert.ToDouble(dataRead["mCurrentStock"].ToString());
                        detailsModel.mLuQty = Convert.ToDouble(dataRead["mLuQty"].ToString());
                        detailsModel.mSuQty = Convert.ToDouble(dataRead["mSuQty"].ToString());
                        detailsModel.mTotalQty = Convert.ToDouble(dataRead["mTotalQty"].ToString());
                        detailsModel.mAmount = Convert.ToDouble(dataRead["mAmount"].ToString());
                        detailsModel.mVat = Convert.ToDouble(dataRead["mVat"].ToString());
                        detailsModel.mDiscount = Convert.ToDouble(dataRead["mDiscount"].ToString());
                        detailsModel.mNetAmount = Convert.ToDouble(dataRead["mProductNetAmount"].ToString());
                        detailsModel.mPurchaseRate = Convert.ToDouble(dataRead["mPurchaseRate"].ToString());
                        detailsModel.mSalesRate = Convert.ToDouble(dataRead["mSalesRate"].ToString());
                        lstDetailModel.Add(detailsModel);
                        i++;
                    }
                    dataRead.Close();
                    dataRead.Dispose();
                }
                else
                {
                    masterModel.dPurchaseDate = dDate;
                    masterModel.vReferenceNo = ReferenceNo;
                    masterModel.mVatPercent = Convert.ToDouble((vatPercentage == "" ? "0" : vatPercentage));
                }
            }
            
            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailModel }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkBillNo(string challanNo, string referenceNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select count(*) countBill from POS.tbPurchaseReceiveMaster where vChallanNo = '" + challanNo + "' and vReferenceNo != '" + referenceNo + "'";
            SqlCommand command = new SqlCommand(sqlQuery,connection);
            SqlDataReader dataReader = command.ExecuteReader();
            var success = false;
            int countRow = 0;
            while (dataReader.Read())
            {
                countRow = Convert.ToInt16(dataReader["countBill"].ToString());
            }
            dataReader.Close();
            dataReader.Dispose();

            if(countRow > 0)
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
            List<PurchaseMasterModel> masterList = new List<PurchaseMasterModel>();
            string sqlQuery = "select CONVERT(varchar,dPurchaseDate,105) dPurchaseDate,vReferenceNo,vChallanNo,mTotalAmount,mTotalVatAmount,(mTotalAmount + mTotalVatAmount) VATWithAmount,mTotalDiscount,mNetAmount from POS.tbPurchaseReceiveMaster Order by dPurchaseDate,vChallanNo";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader readData = command.ExecuteReader();
            while (readData.Read())
            {
                PurchaseMasterModel modelMaster = new PurchaseMasterModel();
                modelMaster.dPurchaseDate = readData["dPurchaseDate"].ToString();
                modelMaster.vReferenceNo = readData["vReferenceNo"].ToString();
                modelMaster.vChallanNo = readData["vChallanNo"].ToString();
                modelMaster.mTotalAmount = Convert.ToDouble(readData["mTotalAmount"].ToString());
                modelMaster.mTotalVatAmount = Convert.ToDouble(readData["mTotalVatAmount"].ToString());
                modelMaster.mTotalVatWithAmount = Convert.ToDouble(readData["VATWithAmount"].ToString());
                modelMaster.mTotalDiscount = Convert.ToDouble(readData["mTotalDiscount"].ToString());
                modelMaster.mNetAmount = Convert.ToDouble(readData["mNetAmount"].ToString());

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
            string sqlQuery = "select 'Bill-' + CONVERT(VARCHAR(4),YEAR(CONVERT(date,'" + dDate + "',103))) + '/' + CONVERT(VARCHAR(2),MONTH(CONVERT(date,'" + dDate + "',103))) + '-' + CONVERT(VARCHAR,ISNULL(MAX(CAST(SUBSTRING(vReferenceNo,13,LEN(vReferenceNo)) as int)),0) + 1) ReferenceNo from POS.tbPurchaseReceiveMaster where MONTH(dPurchaseDate) = MONTH(CONVERT(date,'" + dDate + "',103))";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader readData = command.ExecuteReader();
            var vReferenceNo = "";
            while (readData.Read())
            {
                vReferenceNo = readData["ReferenceNo"].ToString();
            }
            readData.Close();
            readData.Dispose();
            connection.Close();
            return Json(new { vReferenceNo = vReferenceNo }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPreviousDues(string vSupplierID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select SUM(mNetAmount - ISNULL(nPaidAmount,0)) mNetAmount from POS.tbPurchaseReceiveMaster where vSupplierID = '" + vSupplierID + "'";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader readData = command.ExecuteReader();
            var nPreviousDues = "";
            while (readData.Read())
            {
                nPreviousDues = readData["mNetAmount"].ToString();
            }
            readData.Close();
            readData.Dispose();
            connection.Close();
            return Json(new { nPreviousDues = nPreviousDues }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PurchaserInfo/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        //
        // POST: /PurchaserInfo/Create
        [HttpPost]
        public JsonResult Create(PurchaseMasterModel masterModel, PurchaseDetailsList detailsListModel)
        {
            try
            {
                // TODO: Add insert logic here
                //select vUnitId,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,vUserName,dEntryTime from POS.tbPurchaseReceiveMaster
                //select dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount,mNetAmount,mPurchaseRate,mSalesRate from POS.tbPurchaseReceiveDetails
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open(); 
                var message = "All information saved successfully";
                string sqlQuery = "insert into POS.tbPurchaseReceiveMaster (vUnitId,vSupplierID,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount," +
                "mTotalVatAmount,mTotalDiscount,mNetAmount,nPaidAmount,vUserName,dEntryTime) values ('','" + masterModel.vSupplierID + "',CONVERT(date,'" + masterModel.dPurchaseDate + "',103),'" + masterModel.vReferenceNo + "'," +
                "'" + masterModel.vReferenceNo + "','" + masterModel.vChallanNo + "','" + masterModel.mVatPercent + "','" + masterModel.mTotalAmount + "'," +
                "'" + masterModel.mTotalVatAmount + "','" + masterModel.mTotalDiscount + "','" + masterModel.mNetAmount + "','" + masterModel.mPaidAmount + "','" + Session["userName"] + "',GETDATE())";

                if(masterModel.vReferenceNoAuto != null)
                {
                    message = "All information updated successfully";
                    sqlQuery = "insert into POS.tbUDPurchaseReceiveMaster (vUnitId,vSupplierID,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount," +
                    "mTotalVatAmount,mTotalDiscount,mNetAmount,nPaidAmount,vEditFlag,vUserName,dEntryTime) select vUnitId,vSupplierID,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo," +
                    "mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,nPaidAmount,'Edit',vUserName,dEntryTime from POS.tbPurchaseReceiveMaster where vReferenceNo = '" + masterModel.vReferenceNo + "'";
                    
                    sqlQuery += "insert into POS.tbUDPurchaseReceiveDetails (vUnitId,dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount," +
                    "mNetAmount,mPurchaseRate,mSalesRate) select vUnitId,dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount," +
                    "mNetAmount,mPurchaseRate,mSalesRate from POS.tbPurchaseReceiveDetails where vReferenceNo = '" + masterModel.vReferenceNo + "'";
                    
                    sqlQuery += "Delete from POS.tbPurchaseReceiveDetails where vReferenceNo = '" + masterModel.vReferenceNo + "'";
                    sqlQuery += "update POS.tbPurchaseReceiveMaster set dPurchaseDate = CONVERT(date,'" + masterModel.dPurchaseDate + "',103),"+
                    "vChallanNo = '" + masterModel.vChallanNo + "',mVatPercent = '" + masterModel.mVatPercent + "',mTotalAmount = '" + masterModel.mTotalAmount + "'," +
                    "mTotalVatAmount = '" + masterModel.mTotalVatAmount + "',mTotalDiscount = '" + masterModel.mTotalDiscount + "',"+
                    "mNetAmount = '" + masterModel.mNetAmount + "',nPaidAmount = '" + masterModel.mPaidAmount + "',vUserName = '" + Session["userName"] + "',dEntryTime = GETDATE() where vReferenceNo = '" + masterModel.vReferenceNo + "'";
                }

                sqlQuery += "insert into POS.tbPurchaseReceiveDetails (vUnitId,dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount," +
                "mNetAmount,mPurchaseRate,mSalesRate) values ";
                foreach (var getData in detailsListModel.purchaseDetailList)
                {
                    sqlQuery += "('',CONVERT(date,'" + masterModel.dPurchaseDate + "',103),'" + masterModel.vReferenceNo + "','" + getData.vProductID + "','" + getData.vPackId + "'," +
                    "'" + getData.mLuQty + "','" + getData.mSuQty + "','" + getData.mTotalQty + "','" + getData.mAmount + "','" + getData.mVat + "','" + getData.mDiscount + "'," +
                    "'" + getData.mNetAmount + "','" + getData.mPurchaseRate + "','" + getData.mSalesRate + "'),";
                }

                SqlCommand command = new SqlCommand(sqlQuery.Substring(0,sqlQuery.Length - 1), connection);
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PurchaseSummary()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult PurchaseSummary(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Purchase Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult PurchaseInvoiceReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult PurchaseInvoiceReport(string vIssueNo)
        {
            if (Session["userName"] != null)
            {
                Session["vBillNo"] = vIssueNo;
                Session["ReportName"] = "Purchase Invoice";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult ProductWisePurchaseSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWisePurchaseSummaryReport(string vProductID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Product Wise Purchase Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult InvoiceWisePurchaseSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWisePurchaseSummaryReport( string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Invoice Wise Purchase Summary";
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
