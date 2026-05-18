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
    public class OpeningBalanceController : Controller
    {
        public JsonResult GetAllProduct(string supplierID, string packID, string ReferenceNoAuto)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<ProductInfoModel> lstProduct = new List<ProductInfoModel>();
            string sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vSupplierId like '" + supplierID + "' and vPackId like '" + packID + "' and "+
                "isActive = 'Active' and vProductId not in (select vProductId from POS.tbOpeningBalanceDetails where vUnitId = '" + Session["UnitName"] + "') order by vProductName";
            if(ReferenceNoAuto != "")
            {
                sqlQuery = "select vProductID,vProductName from MASTER.tbProductInfo where vSupplierId like '" + supplierID + "' and vPackId like '" + packID + "' and " +
                "isActive = 'Active' order by vProductName";
            }
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

        public JsonResult GetProductData(string vProductID,string dOpeningDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPackId,vLargeUnit,vSmallUnit,(select [POS].StockQty(vProductID, CONVERT(date, '" + dOpeningDate + "', 103),'" + Session["UnitName"] + "')) current_Stock,mSalesRate from MASTER.tbProductInfo where vProductId = '" + vProductID + "'";
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

        public JsonResult getDataForEdit(string dDate, string ReferenceNoAuto, string vReferenceNo, string vChallanNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string checkEdit = "select count(*) count_Row from INFO.tbUserAuthentication where vUnitID = '" + Session["UnitName"] + "' and vUserName = '" + Session["userName"] + "' and MenuID = 'OpeningBalance' and iEdit = 1";
            SqlCommand command = new SqlCommand(checkEdit, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            OpeningBalanceMasterModel masterModel = new OpeningBalanceMasterModel();
            List<OpeningBalanceDetailModel> lstDetailModel = new List<OpeningBalanceDetailModel>();
            int chkCount = 0;
            while (dataRead.Read())
            {
                chkCount = Convert.ToInt16(dataRead["count_Row"].ToString());
            }
            dataRead.Close();
            dataRead.Dispose();
            if (chkCount == 1 || Session["AccountType"].ToString().Equals("Super Admin") || Session["AccountType"].ToString().Equals("Admin"))
            {
                string sqlQuery = "select count(*) countCheck from POS.tbOpeningBalanceMaster where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
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
                    sqlQuery = "select CONVERT(varchar,prm.dOpeningDate,105) dOpeningDate,prm.vReferenceNo,prm.vReferenceNoAuto,prm.vChallanNo,prm.nTotalAmount,prm.vRemarks,prd.vProductId,"+
                    "pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) mCurrentStock,"+
                    "prd.mLuQty,prd.mSuQty,prd.mTotalQty,prd.mTotalAmount,prd.mPurchaseRate,prd.mSalesRate from POS.tbOpeningBalanceMaster prm inner join POS.tbOpeningBalanceDetails "+
                    "prd on prm.vReferenceNo = prd.vReferenceNo and prm.vUnitID = prd.vUnitID inner join MASTER.tbProductInfo pin on prd.vProductId = pin.vProductId inner join "+
                    "MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId where vChallanNo = '" + vChallanNo + "' and prm.vUnitID = '" + Session["UnitName"] + "'";
                    command.CommandText = sqlQuery;
                    dataRead = command.ExecuteReader();
                    int i = 0;
                    while (dataRead.Read())
                    {
                        if (i == 0)
                        {
                            masterModel.dOpeningDate = dataRead["dOpeningDate"].ToString();
                            masterModel.vReferenceNo = dataRead["vReferenceNo"].ToString();
                            masterModel.vChallanNo = dataRead["vChallanNo"].ToString();
                            masterModel.vReferenceNoAuto = dataRead["vReferenceNoAuto"].ToString();
                            ReferenceNoAuto = masterModel.vReferenceNoAuto;
                            masterModel.mTotalAmount = Convert.ToDouble(dataRead["nTotalAmount"].ToString());
                            masterModel.vRemarks = dataRead["vRemarks"].ToString();
                        }

                        OpeningBalanceDetailModel detailsModel = new OpeningBalanceDetailModel();
                        detailsModel.vProductId = dataRead["vProductId"].ToString();
                        detailsModel.vProductName = dataRead["vProductName"].ToString();
                        detailsModel.vPackName = dataRead["vPackSize"].ToString();
                        detailsModel.mMaxQty = Convert.ToDouble(dataRead["vLargeUnit"].ToString());
                        detailsModel.mMinQty = Convert.ToDouble(dataRead["vSmallUnit"].ToString());
                        detailsModel.mCurrentStock = Convert.ToDouble(dataRead["mCurrentStock"].ToString());
                        detailsModel.mLuQty = Convert.ToDouble(dataRead["mLuQty"].ToString());
                        detailsModel.mSuQty = Convert.ToDouble(dataRead["mSuQty"].ToString());
                        detailsModel.mTotalQty = Convert.ToDouble(dataRead["mTotalQty"].ToString());
                        detailsModel.mPurchaseRate = Convert.ToDouble(dataRead["mPurchaseRate"].ToString());
                        detailsModel.TotalAmount = Convert.ToDouble(dataRead["mTotalAmount"].ToString());
                        detailsModel.mSalesRate = Convert.ToDouble(dataRead["mSalesRate"].ToString());
                        lstDetailModel.Add(detailsModel);
                        i++;
                    }
                    dataRead.Close();
                    dataRead.Dispose();
                }
                else
                {
                    masterModel.dOpeningDate = dDate;
                    masterModel.vReferenceNoAuto = ReferenceNoAuto;
                    masterModel.vReferenceNo = vReferenceNo;
                    masterModel.vChallanNo = vChallanNo;
                }
            }
            if (ReferenceNoAuto == "")
            {
                string sqlQuery = "select count(*) countCheck from POS.tbOpeningBalanceMaster where vChallanNo = '" + vChallanNo + "' and vUnitID = '" + Session["UnitName"] + "'";
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
                    masterModel.dOpeningDate = dDate;
                    masterModel.vReferenceNo = vReferenceNo;
                }
                else
                {
                    masterModel.dOpeningDate = dDate;
                    masterModel.vReferenceNo = vReferenceNo;
                    masterModel.vChallanNo = vChallanNo;
                }
            }

            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailModel }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult checkBillNo(string challanNo, string referenceNo)
        //{
        //    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
        //    connection.Open();
        //    string sqlQuery = "select count(*) countBill from POS.tbPurchaseReceiveMaster where vChallanNo = '" + challanNo + "' and vReferenceNo != '" + referenceNo + "'";
        //    SqlCommand command = new SqlCommand(sqlQuery, connection);
        //    SqlDataReader dataReader = command.ExecuteReader();
        //    var success = false;
        //    int countRow = 0;
        //    while (dataReader.Read())
        //    {
        //        countRow = Convert.ToInt16(dataReader["countBill"].ToString());
        //    }
        //    dataReader.Close();
        //    dataReader.Dispose();

        //    if (countRow > 0)
        //    {
        //        success = true;
        //    }
        //    connection.Close();
        //    return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        //}

        //////////////////////Index (view still is not created)//////////////////////////////
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
        //////////////////////Index (view still is not created)//////////////////////////////
        
        public JsonResult getReferenceNo(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            IssueMasterModel modelMaster = new IssueMasterModel();
            string sqlQuery = "select 'OP-' + CONVERT(VARCHAR(4),YEAR(CONVERT(date,'" + dDate + "',103))) + '/' + CONVERT(VARCHAR(2),MONTH(CONVERT(date,'" + dDate + "',103))) + '-' + CONVERT(VARCHAR,ISNULL(MAX(CAST(SUBSTRING(vReferenceNo,11,LEN(vReferenceNo)) as int)),0) + 1) ReferenceNo from POS.tbOpeningBalanceMaster where MONTH(dOpeningDate) = MONTH(CONVERT(date,'" + dDate + "',103)) and vUnitId = '" + Session["UnitName"] + "'";
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
        public JsonResult Create(OpeningBalanceMasterModel masterModel, OpeningBalanceDetailList detailsListModel)
        {
            try
            {
                // TODO: Add insert logic here
                //select vUnitId,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,vUserName,dEntryTime from POS.tbPurchaseReceiveMaster
                //select dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount,mNetAmount,mPurchaseRate,mSalesRate from POS.tbPurchaseReceiveDetails
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                var message = "All information saved successfully";
                string sqlQuery = "insert into POS.tbOpeningBalanceMaster (vUnitId,vReferenceNo,vReferenceNoAuto,vChallanNo,dOpeningDate,vSupplierId,nTotalAmount,vRemarks,"+
                "vUserName,dEntryTime) values ('" + Session["UnitName"] + "','" + masterModel.vReferenceNo + "','" + masterModel.vReferenceNo + "','" + masterModel.vChallanNo + "'," +
                "CONVERT(date,'" + masterModel.dOpeningDate + "',103),'','" + masterModel.mTotalAmount + "','" + masterModel.vRemarks + "'," +
                "'" + Session["userName"] + "',GETDATE())";

                if (masterModel.vReferenceNoAuto != null)
                {
                    message = "All information updated successfully";
                    sqlQuery = "insert into POS.tbUDOpeningBalanceMaster (vUnitId,vReferenceNo,vReferenceNoAuto,vChallanNo,dOpeningDate,vSupplierId,nTotalAmount,vRemarks," +
                    "vEditFlag,vUserName,dEntryTime) select vUnitId,vReferenceNo,vReferenceNoAuto,vChallanNo,dOpeningDate,vSupplierId,nTotalAmount,vRemarks," +
                    "'Edit',vUserName,dEntryTime from POS.tbOpeningBalanceMaster where vReferenceNo = '" + masterModel.vReferenceNo + "' and vUnitID = '" + Session["UnitName"] + "'";

                    sqlQuery += "insert into POS.tbUDOpeningBalanceDetails (vUnitID,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mPurchaseRate,mSalesRate,mTotalAmount,"+
                        "mTotalQty) select vUnitID,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mPurchaseRate,mSalesRate,mTotalAmount,mTotalQty from POS.tbOpeningBalanceDetails "+
                        "where vReferenceNo = '" + masterModel.vReferenceNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                    
                    sqlQuery += "Delete from POS.tbOpeningBalanceDetails where vReferenceNo = '" + masterModel.vReferenceNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                    sqlQuery += "update POS.tbOpeningBalanceMaster set dOpeningDate = CONVERT(date,'" + masterModel.dOpeningDate + "',103)," +
                    "vChallanNo = '" + masterModel.vChallanNo + "',nTotalAmount = '" + masterModel.mTotalAmount + "'," +
                    "vUserName = '" + Session["userName"] + "',dEntryTime = GETDATE() where vReferenceNo = '" + masterModel.vReferenceNo + "' and vUnitID = '" + Session["UnitName"] + "'";
                }

                sqlQuery += "insert into POS.tbOpeningBalanceDetails (vUnitID,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mPurchaseRate,mSalesRate,mTotalAmount,mTotalQty) values ";
                foreach (var getData in detailsListModel.OpeningDetailsList)
                {
                    sqlQuery += "('" + Session["UnitName"] + "','" + masterModel.vReferenceNo + "','" + getData.vProductId + "','" + getData.vPackId + "','" + getData.mLuQty + "'," +
                    "'" + getData.mSuQty + "','" + getData.mPurchaseRate + "','" + getData.mSalesRate + "','" + getData.TotalAmount + "','" + getData.mTotalQty + "'),";
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
    }
}
