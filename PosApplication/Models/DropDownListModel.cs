using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PosApplication.Models
{
    public class DropDownListModel
    {
        public static List<SelectListItem> GetAccountType
        {
            get
            {
                List<SelectListItem> lstAccountType = new List<SelectListItem>();
                lstAccountType.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                lstAccountType.Add(new SelectListItem { Value = "Super Admin", Text = "Super Admin" });
                lstAccountType.Add(new SelectListItem { Value = "Admin", Text = "Admin" });
                lstAccountType.Add(new SelectListItem { Value = "General", Text = "General" });
                return lstAccountType;
            }
        }
        public static List<SelectListItem> GetCompany
        {
            get
            {
                List<SelectListItem> lstCompany = new List<SelectListItem>();
                lstCompany.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vCompanyId,vCompanyName from MASTER.tbCompanyInfo";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstCompany.Add(new SelectListItem { Value = reader["vCompanyId"].ToString(), Text = reader["vCompanyName"].ToString() });
                }
                connection.Close();
                return lstCompany;
            }
        }
        public static List<SelectListItem> GetUnitType
        {
            get
            {
                List<SelectListItem> lstUnitType = new List<SelectListItem>();
                lstUnitType.Add(new SelectListItem { Value = "Branch", Text = "Branch" });
                lstUnitType.Add(new SelectListItem { Value = "Main", Text = "Main" });
                return lstUnitType;
            }
        }
        public static List<SelectListItem> GetStatus
        {
            get
            {
                List<SelectListItem> lstStatus = new List<SelectListItem>();
                lstStatus.Add(new SelectListItem { Value = "Active", Text = "Active" });
                lstStatus.Add(new SelectListItem { Value = "Inactive", Text = "Inactive" });
                return lstStatus;
            }
        }
        public static List<SelectListItem> GetPaymentMethod
        {
            get
            {
                List<SelectListItem> lstStatus = new List<SelectListItem>();
                lstStatus.Add(new SelectListItem { Value = "Cash", Text = "Cash" });
                lstStatus.Add(new SelectListItem { Value = "Cheque", Text = "Cheque" });
                lstStatus.Add(new SelectListItem { Value = "BKash", Text = "BKash" });
                lstStatus.Add(new SelectListItem { Value = "Others", Text = "Others" });
                return lstStatus;
            }
        }
        public static List<SelectListItem> GetSupplier
        {
            get
            {
                List<SelectListItem> lstSupplier = new List<SelectListItem>();
                lstSupplier.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vSupplierId,vSupplierName from MASTER.tbSupplierInfo where isActive = 'Active' Order by vSupplierName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstSupplier.Add(new SelectListItem { Value = reader["vSupplierId"].ToString(), Text = reader["vSupplierName"].ToString() });
                }
                connection.Close();
                return lstSupplier;
            }
        }
        public static List<SelectListItem> GetRackName
        {
            get
            {
                List<SelectListItem> lstRackName = new List<SelectListItem>();
                lstRackName.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vRackId,vRackName from MASTER.tbRackInfo where isActive = 'Active' order by vRackName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstRackName.Add(new SelectListItem { Value = reader["vRackId"].ToString(), Text = reader["vRackName"].ToString() });
                }
                connection.Close();
                return lstRackName;
            }
        }
        public static List<SelectListItem> GetPackSize
        {
            get
            {
                List<SelectListItem> lstPackSize = new List<SelectListItem>();
                lstPackSize.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vPackId,vPackSize from MASTER.tbPackInfo where isActive = 'Active'  Order by vPackSize";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstPackSize.Add(new SelectListItem { Value = reader["vPackId"].ToString(), Text = reader["vPackSize"].ToString() });
                }
                connection.Close();
                return lstPackSize;
            }
        }
        public static List<SelectListItem> GetUnitName
        {
            get
            {
                List<SelectListItem> lstUnitName = new List<SelectListItem>();
                lstUnitName.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vUnitId,vUnitName from MASTER.tbUnitInfo order by vUnitName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstUnitName.Add(new SelectListItem { Value = reader["vUnitId"].ToString(), Text = reader["vUnitName"].ToString() });
                }
                connection.Close();
                return lstUnitName;
            }
        }
        public static List<SelectListItem> GetActiveUnitName
        {
            get
            {
                List<SelectListItem> lstUnitName = new List<SelectListItem>();
                lstUnitName.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vUnitId,vUnitName from MASTER.tbUnitInfo where isActive = 'Active' order by vUnitName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstUnitName.Add(new SelectListItem { Value = reader["vUnitId"].ToString(), Text = reader["vUnitName"].ToString() });
                }
                connection.Close();
                return lstUnitName;
            }
        }
        public static List<SelectListItem> GetIssueUnitName
        {
            get
            {
                List<SelectListItem> lstIssueUnitName = new List<SelectListItem>();
                lstIssueUnitName.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select distinct vUnitId, vUnitName from POS.tbIssueMaster im inner join  MASTER.tbUnitInfo ui on im.vIssueTo = ui.vUnitId "+
                               "where vUnitId = '" + HttpContext.Current.Session["UnitName"] + "' order by vUnitName";
                if (HttpContext.Current.Session["UnitName"].Equals("U-1"))
                    query = "select distinct vUnitId, vUnitName from POS.tbIssueMaster im inner join  MASTER.tbUnitInfo ui on im.vIssueTo = ui.vUnitId order by vUnitName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstIssueUnitName.Add(new SelectListItem { Value = reader["vUnitId"].ToString(), Text = reader["vUnitName"].ToString() });
                }
                connection.Close();
                return lstIssueUnitName;
            }
        }
        public static List<SelectListItem> GetIssueNo
        {
            get
            {
                List<SelectListItem> lstIssueNo = new List<SelectListItem>();
                lstIssueNo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vIssueNo,vReferenceNo from POS.tbIssueMaster where iStatus = 0 and vIssueTo = '" + HttpContext.Current.Session["UnitName"] + "'";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstIssueNo.Add(new SelectListItem { Value = reader["vReferenceNo"].ToString(), Text = reader["vIssueNo"].ToString() });
                }
                connection.Close();
                return lstIssueNo;
            }
        }
        public static List<SelectListItem> GetReceivedIssueNo
        {
            get
            {
                List<SelectListItem> lstReceivedIssueNo = new List<SelectListItem>();
                lstReceivedIssueNo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vIssueNo,vReferenceNo from POS.tbIssueMaster where iStatus = 1 and iReturnStatus != 1 and vIssueTo = '" + HttpContext.Current.Session["UnitName"] + "'";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstReceivedIssueNo.Add(new SelectListItem { Value = reader["vReferenceNo"].ToString(), Text = reader["vIssueNo"].ToString() });
                }
                connection.Close();
                return lstReceivedIssueNo;
            }
        }
        public static List<SelectListItem> GetReceivedInvoiceNo
        {
            get
            {
                List<SelectListItem> lstInvoiceNo = new List<SelectListItem>();
                lstInvoiceNo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vInvoiceNo,vChallanNo from POS.tbSalesMaster where iStatus = 1 and iReturnStatus != 1 and vUnitID = '" + HttpContext.Current.Session["UnitName"] + "'";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstInvoiceNo.Add(new SelectListItem { Value = reader["vInvoiceNo"].ToString(), Text = reader["vChallanNo"].ToString() });
                }
                connection.Close();
                return lstInvoiceNo;
            }
        }

        public static List<SelectListItem> GetCategory
        {
            get
            {
                List<SelectListItem> lstCategory = new List<SelectListItem>();
                lstCategory.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vCategoryID, vCategoryName from MASTER.tbCategoryInformation where vStatus = 'Active' Order by vCategoryName";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstCategory.Add(new SelectListItem { Value = reader["vCategoryID"].ToString(), Text = reader["vCategoryName"].ToString() });
                }
                connection.Close();
                return lstCategory;
            }
        }

        public static List<SelectListItem> GetProductUnit
        {
            get
            {
                List<SelectListItem> lstUnit = new List<SelectListItem>();
                lstUnit.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "select vProductUnitID,vProductUnitShortName from MASTER.tbProductUnitInfo";
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lstUnit.Add(new SelectListItem { Value = reader["vProductUnitID"].ToString(), Text = reader["vProductUnitShortName"].ToString() });
                }
                connection.Close();
                return lstUnit;
            }
        }
    }
}