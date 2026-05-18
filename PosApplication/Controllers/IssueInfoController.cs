using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using PosApplication.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace PosApplication.Controllers
{
    public class IssueInfoController : Controller
    {
        public JsonResult GetAllInvoice(string dDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<IssueMasterModel> lstIssue = new List<IssueMasterModel>();
            string sqlQuery = "select vReferenceNo,vIssueNo from POS.tbIssueMaster where dIssueDate = CONVERT(date,'" + dDate + "',103) and vIssueFrom = '" + Session["UnitName"] + "' order by vIssueNo desc";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                IssueMasterModel modelMaster = new IssueMasterModel();
                modelMaster.vReferenceNo = reader["vReferenceNo"].ToString();
                modelMaster.vIssueNo = reader["vIssueNo"].ToString();
                lstIssue.Add(modelMaster);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult { Data = lstIssue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetAllReceivedInvoice(string dDate, string UnitID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<IssueMasterModel> lstIssue = new List<IssueMasterModel>();
            string sqlQuery = "select vReferenceNo,vIssueNo from POS.tbIssueMaster where dIssueDate = CONVERT(date,'" + dDate + "',103) and vIssueTo = '" + UnitID + "' and iStatus = 1 order by vIssueNo desc";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                IssueMasterModel modelMaster = new IssueMasterModel();
                modelMaster.vReferenceNo = reader["vReferenceNo"].ToString();
                modelMaster.vIssueNo = reader["vIssueNo"].ToString();
                lstIssue.Add(modelMaster);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult { Data = lstIssue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetAllReturnInvoice(string dDate, string UnitID)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            List<IssueMasterModel> lstIssue = new List<IssueMasterModel>();
            string sqlQuery = "select vReferenceNo,vIssueNo from POS.tbIssueMaster where dIssueDate = CONVERT(date,'" + dDate + "',103) and vIssueTo = '" + UnitID + "' and iReturnStatus = 1 order by vIssueNo desc";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                IssueMasterModel modelMaster = new IssueMasterModel();
                modelMaster.vReferenceNo = reader["vReferenceNo"].ToString();
                modelMaster.vIssueNo = reader["vIssueNo"].ToString();
                lstIssue.Add(modelMaster);
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return new JsonResult { Data = lstIssue, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        public JsonResult GetProductData(string vProductID, string dIssueDate)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPackId,vLargeUnit,vSmallUnit,(select [POS].StockQty(vProductID, CONVERT(date, '" + dIssueDate + "', 103),"+
                "'" + Session["UnitName"] + "')) current_Stock,(select [POS].PurchaseRateCalculation(vProductID, CONVERT(date, '" + dIssueDate + "', 103),"+
                "'" + Session["UnitName"] + "')) PurchaseRate,mSalesRate from MASTER.tbProductInfo where vProductId = '" + vProductID + "'";
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
                currentStock = reader["current_Stock"].ToString();
                purchaseRate = reader["PurchaseRate"].ToString();
                salesRate = reader["mSalesRate"].ToString();
            }
            reader.Close();
            reader.Dispose();
            connection.Close();
            return Json(new { packID = packID, largeUnit = largeUnit, smallUnit = smallUnit, currentStock = currentStock, purchaseRate = purchaseRate, salesRate = salesRate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDataForEdit(string dDate, string ReferenceNo, string vatPercentage, string vIssueNo, string vIssuedBy, string vIssueTo, string vReceivedBy)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string checkEdit = "select count(*) count_Row from INFO.tbUserAuthentication where vUnitID = '" + Session["UnitName"] + "' and vUserName = '" + Session["userName"] + "' and MenuID = 'PurchaseInfo' and iEdit = 1";
            SqlCommand command = new SqlCommand(checkEdit, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            IssueMasterModel masterModel = new IssueMasterModel();
            List<IssueDetailsModel> lstDetailModel = new List<IssueDetailsModel>();
            int chkCount = 0;
            while (dataRead.Read())
            {
                chkCount = Convert.ToInt16(dataRead["count_Row"].ToString());
            }
            dataRead.Close();
            dataRead.Dispose();
            if (chkCount == 1 || Session["AccountType"].ToString().Equals("Super Admin") || Session["AccountType"].ToString().Equals("Admin"))
            {
                string sqlQuery = "select count(*) countCheck from POS.tbIssueMaster where vIssueNo = '" + vIssueNo + "'";
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
                    sqlQuery = "select CONVERT(varchar,prm.dIssueDate,105) dIssueDate,prm.vReferenceNo,prm.vIssueNo,prm.vIssuedBy,prm.vIssueTo,prm.vReceivedBy,prm.vRemarks,"+
                               "prd.vProductId,pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),"+
                               "'" + Session["UnitName"] + "')) cStock,(select [POS].PurchaseRateCalculation(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) PurchaseRate,prd.mLuQty," +
                               "prd.mSuQty,prd.mSalesRate from POS.tbIssueMaster prm inner join POS.tbIssueDetails prd on prm.vReferenceNo = prd.vReferenceNo inner join "+
                               "MASTER.tbProductInfo pin on prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId where "+
                               "vIssueNo = '" + vIssueNo + "' and prm.iStatus = 0";
                    command.CommandText = sqlQuery;
                    dataRead = command.ExecuteReader();
                    int i = 0;
                    while (dataRead.Read())
                    {
                        if (i == 0)
                        {
                            masterModel.dIssueDate = dataRead["dIssueDate"].ToString();
                            masterModel.vReferenceNo = dataRead["vReferenceNo"].ToString();
                            masterModel.vReferenceNoAuto = dataRead["vReferenceNo"].ToString();
                            masterModel.vIssueNo = dataRead["vIssueNo"].ToString();
                            masterModel.vIssuedBy = dataRead["vIssuedBy"].ToString();
                            masterModel.vIssueTo = dataRead["vIssueTo"].ToString();
                            masterModel.vReceivedBy = dataRead["vReceivedBy"].ToString();
                            masterModel.vRemarks = dataRead["vRemarks"].ToString();
                        }

                        IssueDetailsModel detailsModel = new IssueDetailsModel();
                        detailsModel.vProductID = dataRead["vProductId"].ToString();
                        detailsModel.vProductName = dataRead["vProductName"].ToString();
                        detailsModel.vPackName = dataRead["vPackSize"].ToString();
                        detailsModel.mMaxQty = Convert.ToDouble(dataRead["vLargeUnit"].ToString());
                        detailsModel.mMinQty = Convert.ToDouble(dataRead["vSmallUnit"].ToString());
                        detailsModel.mCurrentStock = Convert.ToDouble(dataRead["cStock"].ToString());
                        detailsModel.mIssueRate = Convert.ToDouble(dataRead["PurchaseRate"].ToString());
                        detailsModel.mLuQty = Convert.ToDouble(dataRead["mLuQty"].ToString());
                        detailsModel.mSuQty = Convert.ToDouble(dataRead["mSuQty"].ToString());
                        detailsModel.mSalesRate = Convert.ToDouble(dataRead["mSalesRate"].ToString());
                        lstDetailModel.Add(detailsModel);
                        i++;
                    }
                    dataRead.Close();
                    dataRead.Dispose();
                }
                else
                {
                    masterModel.dIssueDate = dDate;
                    masterModel.vReferenceNo = ReferenceNo;
                    masterModel.vIssueNo = vIssueNo;
                    masterModel.vIssuedBy = vIssuedBy;
                    masterModel.vIssueTo = vIssueTo;
                    masterModel.vReceivedBy = vReceivedBy;
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
            string sqlQuery = "select 'ISN-' + CONVERT(VARCHAR(4),YEAR(CONVERT(date,'" + dDate + "',103))) + '/' + CONVERT(VARCHAR(2),MONTH(CONVERT(date,'" + dDate + "',103))) + '-' + CONVERT(VARCHAR,ISNULL(MAX(CAST(SUBSTRING(vReferenceNo,12,LEN(vReferenceNo)) as int)),0) + 1) ReferenceNo from POS.tbIssueMaster where MONTH(dIssueDate) = MONTH(CONVERT(date,'" + dDate + "',103))";
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
            return Json(new { vReferenceNo = vReferenceNo },JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PurchaserInfo/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            IssueMasterModel modelMaster = new IssueMasterModel();
            modelMaster.vIssuedBy = Session["userName"].ToString();
            return View(modelMaster);
        }

        //
        // POST: /PurchaserInfo/Create
        [HttpPost]
        public JsonResult Create(IssueMasterModel masterModel, IssueDetailsList detailsListModel)
        {
            try
            {
                // TODO: Add insert logic here
                //select vUnitId,dPurchaseDate,vReferenceNoAuto,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,vUserName,dEntryTime from POS.tbPurchaseReceiveMaster
                //select dPurchaseDate,vReferenceNo,vProductId,vPackId,mLuQty,mSuQty,mTotalQty,mAmount,mVat,mDiscount,mNetAmount,mPurchaseRate,mSalesRate from POS.tbPurchaseReceiveDetails
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                var message = "All information saved successfully";
                string sqlQuery = "insert into POS.tbIssueMaster (vReferenceNo,vReferenceNoAuto,vIssueNo,vIssueFrom,vIssueTo,dIssueDate,vReceivedBy," +
                    "vIssuedBy,vRemarks,vUserName,dEntryTime,iStatus) values ('" + masterModel.vReferenceNo + "','" + masterModel.vReferenceNo + "',"+
                    "'" + masterModel.vIssueNo + "','" + Session["UnitName"] + "','" + masterModel.vIssueTo + "',CONVERT(date,'" + masterModel.dIssueDate + "',103)," +
                    "'" + masterModel.vReceivedBy + "','" + Session["userName"] + "','" + masterModel.vRemarks + "','" + Session["userName"] + "',GETDATE(),0)";

                if (masterModel.vReferenceNoAuto != null)
                {
                    message = "All information updated successfully";
                    sqlQuery = "insert into POS.tbUDIssueMaster (vReferenceNo,vReferenceNoAuto,vIssueNo,vIssueFrom,vIssueTo,dIssueDate,vReceivedBy," +
                    "vIssuedBy,vRemarks,vEditFlag,vUserName,dEntryTime,iStatus) select vReferenceNo,vReferenceNoAuto,vIssueNo,vIssueFrom,vIssueTo,dIssueDate,vReceivedBy," +
                    "vIssuedBy,vRemarks,'Edit',vUserName,dEntryTime,iStatus from POS.tbIssueMaster where vReferenceNo = '" + masterModel.vReferenceNo + "' and "+
                    "vIssueFrom = '" + Session["UnitName"] + "'";

                    sqlQuery += "insert into POS.tbUDIssueDetails (vReferenceNo,vIssueFrom,vIssueTo,vProductId,mStockQty,mLUQty,mSUQty,mIssueQty,nReturnLUQty,nReturnSUQty,"+
                        "nReturnIssueQty,mSalesRate,iStatus) select vReferenceNo,vIssueFrom,vIssueTo,vProductId,mStockQty,mLUQty,mSUQty,mIssueQty,nReturnLUQty,nReturnSUQty,"+
                        "nReturnIssueQty,mSalesRate,iStatus from POS.tbIssueDetails where vReferenceNo = '" + masterModel.vReferenceNo + "' and vIssueFrom = '" + Session["UnitName"] + "'";

                    sqlQuery += "Delete from POS.tbIssueDetails where vReferenceNo = '" + masterModel.vReferenceNo + "' and vIssueFrom = '" + Session["UnitName"] + "'";
                    sqlQuery += "update POS.tbIssueMaster set dIssueDate = CONVERT(date,'" + masterModel.dIssueDate + "',103)," +
                    "vIssueNo = '" + masterModel.vIssueNo + "',vIssueTo = '" + masterModel.vIssueTo + "',vReceivedBy = '" + masterModel.vReceivedBy + "'," +
                    "vRemarks = '" + masterModel.vRemarks + "',vUserName = '" + Session["userName"] + "',dEntryTime = GETDATE() where "+
                    "vReferenceNo = '" + masterModel.vReferenceNo + "' and vIssueFrom = '" + Session["UnitName"] + "'";
                }

                sqlQuery += "insert into POS.tbIssueDetails (vReferenceNo,vIssueFrom,vIssueTo,vProductId,mStockQty,mLUQty,mSUQty,mIssueQty,nReturnLUQty,nReturnSUQty,"+
                    "nReturnIssueQty,mIssueRate,mSalesRate,iStatus) values ";
                foreach (var getData in detailsListModel.lstIssueDetails)
                {
                    sqlQuery += "('" + masterModel.vReferenceNo + "','" + Session["UnitName"] + "','" + masterModel.vIssueTo + "','" + getData.vProductID + "'," +
                    "'" + getData.mCurrentStock + "','" + getData.mLuQty + "','" + getData.mSuQty + "','" + getData.mTotalQty + "','0','0','0','"+ getData.mIssueRate+"',"+
                    "'" + getData.mSalesRate + "','0'),";
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

        public JsonResult getIssueReceiveData(string vIssueNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            IssueMasterModel masterModel = new IssueMasterModel();
            List<IssueDetailsModel> lstDetailsModel = new List<IssueDetailsModel>();
            string sqlQuery = "select CONVERT(varchar,prm.dIssueDate,105) dIssueDate,prm.vReferenceNo,prm.vIssueNo,prm.vIssuedBy,ui.vUnitName vIssueTo,prm.vReceivedBy,prm.vRemarks," +
                        "prd.vProductId,pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) cStock,"+
                        "mIssueRate,prd.mLuQty,prd.mSuQty,prd.mSalesRate from POS.tbIssueMaster prm inner join POS.tbIssueDetails prd on prm.vReferenceNo = prd.vReferenceNo "+
                        "inner join MASTER.tbProductInfo pin on prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId inner join "+
                        "MASTER.tbUnitInfo ui on ui.vUnitId = prm.vIssueTo where vIssueNo = '" + vIssueNo + "' and prm.iStatus = 0";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            int i = 0;
            while (dataRead.Read())
            {
                if (i == 0)
                {
                    masterModel.dIssueDate = dataRead["dIssueDate"].ToString();
                    masterModel.vReferenceNo = dataRead["vReferenceNo"].ToString();
                    masterModel.vReferenceNoAuto = dataRead["vReferenceNo"].ToString();
                    masterModel.vIssueNo = dataRead["vIssueNo"].ToString();
                    masterModel.vIssuedBy = dataRead["vIssuedBy"].ToString();
                    masterModel.vIssueTo = dataRead["vIssueTo"].ToString();
                    masterModel.vReceivedBy = dataRead["vReceivedBy"].ToString();
                    masterModel.vRemarks = dataRead["vRemarks"].ToString();
                }

                IssueDetailsModel detailsModel = new IssueDetailsModel();
                detailsModel.vProductID = dataRead["vProductId"].ToString();
                detailsModel.vProductName = dataRead["vProductName"].ToString();
                detailsModel.vPackName = dataRead["vPackSize"].ToString();
                detailsModel.mMaxQty = Convert.ToDouble(dataRead["vLargeUnit"].ToString());
                detailsModel.mMinQty = Convert.ToDouble(dataRead["vSmallUnit"].ToString());
                detailsModel.mCurrentStock = Convert.ToDouble(dataRead["cStock"].ToString());
                detailsModel.mIssueRate = Convert.ToDouble(dataRead["mIssueRate"].ToString());
                detailsModel.mLuQty = Convert.ToDouble(dataRead["mLuQty"].ToString());
                detailsModel.mSuQty = Convert.ToDouble(dataRead["mSuQty"].ToString());
                detailsModel.mSalesRate = Convert.ToDouble(dataRead["mSalesRate"].ToString());
                lstDetailsModel.Add(detailsModel);
                i++;
            }
            dataRead.Close();
            dataRead.Dispose();
            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailsModel }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssueReceive()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult IssueReceive(string vReferenceNo)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "update POS.tbIssueMaster set iStatus = 1 where vReferenceNo = '" + vReferenceNo + "'";
                sqlQuery += "update POS.tbIssueDetails set iStatus = 1 where vReferenceNo = '" + vReferenceNo + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.ExecuteNonQuery();
                connection.Close();
                connection.Open();
                command.CommandText = "select vIssueNo from POS.tbIssueMaster where vReferenceNo = '" + vReferenceNo + "'";
                SqlDataReader dataReader = command.ExecuteReader();
                var vIssueNo = "";
                while(dataReader.Read())
                {
                    vIssueNo = dataReader["vIssueNo"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
                connection.Close();
                bool mailStatus = sendEmail("Received Issue No.: " + vIssueNo, "Received all products against Issue No." + vIssueNo);
                return Json(new { success = true, mailStatus, message = "All information saved successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool sendEmail(string Subject, string EmailBody)
        {
            try
            {
                var senderAddress = Session["vEmail"].ToString();// "shoumendatasoftbd@gmail.com";
                var receiverAddress = "naserenterprise@yahoo.com";
                var senderPassword = "en123456"; 
                //SmtpClient client = new SmtpClient("mail.meridiangroupbd.com", 25);
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                
                //----------Yahoo
                //client.Host = "smtp.mail.yahoo.com";
                //client.Port = 25;
                //----------Yahoo

                //----------gmail
                //client.Host = "smtp.gmail.com";
                //client.Port = 587;
                //----------gmail
                
                client.EnableSsl = true;
                //client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderAddress, senderPassword);

                MailMessage message_mail = new MailMessage(senderAddress, receiverAddress, Subject, EmailBody);
                //message_mail.Bcc.Add(senderAddress);
                message_mail.IsBodyHtml = true;
                message_mail.BodyEncoding = UTF8Encoding.UTF8;
                message_mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(message_mail);
            }
            catch (Exception exp)
            {
                exp.ToString();
            }
            return true;
        }

        public JsonResult getIssueReceivedData(string vIssueNo)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            IssueMasterModel masterModel = new IssueMasterModel();
            List<IssueDetailsModel> lstDetailsModel = new List<IssueDetailsModel>();
            string sqlQuery = "select CONVERT(varchar,prm.dIssueDate,105) dIssueDate,prm.vReferenceNo,prm.vIssueNo,prm.vIssuedBy,ui.vUnitName vIssueTo,prm.vReceivedBy,prm.vRemarks," +
                        "prd.vProductId,pin.vProductName,pki.vPackSize,pin.vLargeUnit,pin.vSmallUnit,(select [POS].StockQty(prd.vProductId,GETDATE(),'" + Session["UnitName"] + "')) cStock," +
                        "mIssueRate,prd.mLuQty,prd.mSuQty,prd.mSalesRate from POS.tbIssueMaster prm inner join POS.tbIssueDetails prd on prm.vReferenceNo = prd.vReferenceNo " +
                        "inner join MASTER.tbProductInfo pin on prd.vProductId = pin.vProductId inner join MASTER.tbPackInfo pki on pin.vPackId = pki.vPackId inner join " +
                        "MASTER.tbUnitInfo ui on ui.vUnitId = prm.vIssueTo where prm.vReferenceNo = '" + vIssueNo + "' and prm.iStatus = 1 and prm.iReturnStatus != 1";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataRead = command.ExecuteReader();
            int i = 0;
            while (dataRead.Read())
            {
                if (i == 0)
                {
                    masterModel.dIssueDate = dataRead["dIssueDate"].ToString();
                    masterModel.vReferenceNo = dataRead["vReferenceNo"].ToString();
                    masterModel.vReferenceNoAuto = dataRead["vReferenceNo"].ToString();
                    masterModel.vIssueNo = dataRead["vIssueNo"].ToString();
                    masterModel.vIssuedBy = dataRead["vIssuedBy"].ToString();
                    masterModel.vIssueTo = dataRead["vIssueTo"].ToString();
                    masterModel.vReceivedBy = dataRead["vReceivedBy"].ToString();
                    masterModel.vRemarks = dataRead["vRemarks"].ToString();
                }

                IssueDetailsModel detailsModel = new IssueDetailsModel();
                detailsModel.vProductID = dataRead["vProductId"].ToString();
                detailsModel.vProductName = dataRead["vProductName"].ToString();
                detailsModel.vPackName = dataRead["vPackSize"].ToString();
                detailsModel.mMaxQty = Convert.ToDouble(dataRead["vLargeUnit"].ToString());
                detailsModel.mMinQty = Convert.ToDouble(dataRead["vSmallUnit"].ToString());
                detailsModel.mCurrentStock = Convert.ToDouble(dataRead["cStock"].ToString());
                detailsModel.mIssueRate = Convert.ToDouble(dataRead["mIssueRate"].ToString());
                detailsModel.mLuQty = Convert.ToDouble(dataRead["mLuQty"].ToString());
                detailsModel.mSuQty = Convert.ToDouble(dataRead["mSuQty"].ToString());
                detailsModel.mSalesRate = Convert.ToDouble(dataRead["mSalesRate"].ToString());
                lstDetailsModel.Add(detailsModel);
                i++;
            }
            dataRead.Close();
            dataRead.Dispose();
            connection.Close();
            return Json(new { masterData = masterModel, detailData = lstDetailsModel }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IssueReturn() {
            if (Session["userName"] == null)
                return RedirectToAction("Login","Account");
            return View();
        }

        [HttpPost]
        public JsonResult IssueReturn(IssueMasterModel masterModel, IssueDetailsList detailsListModel)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "update POS.tbIssueMaster set iReturnStatus = 1 where vReferenceNo = '" + masterModel.vReferenceNo + "' and vIssueTo = '" + Session["UnitName"] + "'";
            foreach(var getdata in detailsListModel.lstIssueDetails)
            {
                sqlQuery += "update POS.tbIssueDetails set nReturnIssueQty = '" + getdata.mReturnTotalQty + "' where vReferenceNo = '" + masterModel.vReferenceNo + "' and " +
                    "vIssueTo = '" + Session["UnitName"] + "' and vProductId = '" + getdata.vProductID + "' ";
            }
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            connection.Close();
            return Json(new { success = true, message = "All Information Saved Successfully"}, JsonRequestBehavior.AllowGet);
        }

        //Issue Invoice
        public ActionResult IssueInvoiceReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult IssueInvoiceReport(string vBillNo)
        {
            if (Session["userName"] != null)
            {
                Session["vBillNo"] = vBillNo;
                Session["ReportName"] = "Issue Invoice";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult IssueSummary()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult IssueSummary(string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Issue Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult ProductWiseIssueSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWiseIssueSummaryReport(string vProductID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vProductID"] = vProductID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Product Wise Issue Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult InvoiceWiseIssueSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWiseIssueSummaryReport(string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Invoice Wise Issue Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult ReceiveInvoiceReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ReceiveInvoiceReport(string vBillNo, string vUnitID)
        {
            if (Session["userName"] != null)
            {
                Session["vBillNo"] = vBillNo;
                Session["vIssueUnit"] = vUnitID;
                Session["ReportName"] = "Receive Invoice";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult ReceiveSummary()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ReceiveSummary(string vUnitID, string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Receive Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult ProductWiseReceiveSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWiseReceiveSummaryReport(string vUnitID, string vProductID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["vProductID"] = vProductID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Product Wise Receive Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult InvoiceWiseReceiveSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWiseReceiveSummaryReport(string vUnitID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Invoice Wise Receive Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult ReturnInvoiceReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ReturnInvoiceReport(string vBillNo, string vUnitID)
        {
            if (Session["userName"] != null)
            {
                Session["vBillNo"] = vBillNo;
                Session["vIssueUnit"] = vUnitID;
                Session["ReportName"] = "Return Invoice";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        public ActionResult ReturnSummary()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ReturnSummary(string vUnitID, string vFromDate, string vToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["vFromDate"] = vFromDate;
                Session["vToDate"] = vToDate;
                Session["ReportName"] = "Return Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult ProductWiseReturnSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult ProductWiseReturnSummaryReport(string vUnitID, string vProductID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["vProductID"] = vProductID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Product Wise Return Summary";
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null);
        }

        //Purchase Invoice
        public ActionResult InvoiceWiseReturnSummaryReport()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public JsonResult InvoiceWiseReturnSummaryReport(string vUnitID, string dFromDate, string dToDate)
        {
            if (Session["userName"] != null)
            {
                Session["vIssueUnit"] = vUnitID;
                Session["dFromDate"] = dFromDate;
                Session["dToDate"] = dToDate;
                Session["ReportName"] = "Invoice Wise Return Summary";
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
