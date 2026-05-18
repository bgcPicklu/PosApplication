<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script runat="server">
        void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userName"] == null)
                    Response.Redirect("/Account/Login");
                if (Session["ReportName"].Equals("Product Information"))
                {
                    if (Session["vProductID"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[MASTER].[prcProductInformation]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductID", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@categoryID", Session["vCategoryID"]));
                        command.Parameters.Add(new SqlParameter("@packID", Session["vPackID"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductInformation.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("PosAppDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["vCategoryID"] = null;
                        Session["vPackID"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Purchase Invoice"))
                {
                    if (Session["vBillNo"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcPurchaseInvoice]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@InvoiceNo", Session["vBillNo"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/PurchaseInvoice.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("PurchaseInvoiceDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vBillNo"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Purchase Summary"))
                {
                    if (Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcPurchseSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/PurchaseSummary.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("PurchaseSummary", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["vFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["vToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Purchase Summary"))
                {
                    if (Session["vProductID"] != null && Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcProductWisePurchaseSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductId", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWisePurchaseSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("ProductWisePurchaseSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Invoice Wise Purchase Summary"))
                {
                    if (Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcInvoiceWisePurchaseSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWisePurchaseSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("InvoiceWisePurchaseSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Issue Invoice"))
                {
                    if (Session["vBillNo"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueChallan]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@unitID", Session["UnitName"]));
                        command.Parameters.Add(new SqlParameter("@IssueNo", Session["vBillNo"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueInvoice.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueInvoiceDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vBillNo"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Issue Summary"))
                {
                    if (Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueSummary.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["vFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["vToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Issue Summary"))
                {
                    if (Session["vProductID"] != null && Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcProductWiseIssueSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductId", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseIssueSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("ProductWiseIssueSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Invoice Wise Issue Summary"))
                {
                    if (Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcInvoiceWiseIssueSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWiseIssueSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("InvoiceWiseIssueSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Stock Register"))
                {
                    if (Session["vProductID"] != null && Session["vFromDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[ProductWiseStockRegister]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductID", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["UnitName"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseStockRegister.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("PosAppDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter param = new ReportParameter("asOnDate", Session["vFromDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(param);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["vFromDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Receive Invoice"))
                {
                    if (Session["vBillNo"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueReceiveChallan]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@unitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@IssueNo", Session["vBillNo"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueReceiveInvoice.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueReceiveInvoiceDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vBillNo"] = null;
                        Session["vIssueUnit"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Receive Summary"))
                {
                    if (Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueReceiveSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueReceiveSummary.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueReceiveSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["vFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["vToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Receive Summary"))
                {
                    if (Session["vProductID"] != null && Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcProductWiseIssueReceiveSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@ProductId", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseIssueReceiveSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("ProductWiseIssueReceiveSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["vProductID"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Invoice Wise Receive Summary"))
                {
                    if (Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcInvoiceWiseIssueReceiveSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWiseIssueReceiveSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("InvoiceWiseIssueReceiveSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Return Invoice"))
                {
                    if (Session["vBillNo"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueReturnChallan]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@unitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@IssueNo", Session["vBillNo"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueReturnInvoice.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueReturnInvoiceDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vBillNo"] = null;
                        Session["vIssueUnit"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Return Summary"))
                {
                    if (Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcIssueReturnSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/IssueReturnSummary.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("IssueReturnSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["vFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["vToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Return Summary"))
                {
                    if (Session["vProductID"] != null && Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcProductWiseIssueReturnSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@ProductId", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseIssueReturnSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("ProductWiseIssueReturnSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["vProductID"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Invoice Wise Return Summary"))
                {
                    if (Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcInvoiceWiseIssueReturnSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vIssueUnit"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWiseIssueReturnSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("InvoiceWiseIssueReturnSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vIssueUnit"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Sales Invoice"))
                {
                    if (Session["vBillNo"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcSalesInvoice]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@invoiceNo", Session["vBillNo"]));
                        command.Parameters.Add(new SqlParameter("@unitID", Session["UnitName"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/SalesInvoice.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("SalesInformation", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vBillNo"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Sales Summary"))
                {
                    if (Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcSalesSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/SalesSummary.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("SalesSummary", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["vFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["vToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[]{FromDate,ToDate});
                        ReportViewer1.LocalReport.Refresh();
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Product Wise Sales Summary"))
                {
                    if (Session["vProductID"] != null && Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcProductWiseSalesSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductId", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseSalesSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("ProductWiseSalesSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Invoice Wise Sales Summary"))
                {
                    if (Session["dFromDate"] != null && Session["dToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcInvoiceWiseSalesSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["dFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["dToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWiseSalesSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("InvoiceWiseSalesSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter FromDate = new ReportParameter("FromDate", Session["dFromDate"].ToString());
                        ReportParameter ToDate = new ReportParameter("ToDate", Session["dToDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { FromDate, ToDate });
                        ReportViewer1.LocalReport.Refresh();
                        Session["dFromDate"] = null;
                        Session["dToDate"] = null;
                    }
                }
                
                else if (Session["ReportName"].Equals("Product Wise Stock Register"))
                {
                    if (Session["vProductID"] != null && Session["vFromDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[ProductWiseStockRegister]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ProductID", Session["vProductID"]));
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["UnitName"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseStockRegister.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("PosAppDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter param = new ReportParameter("asOnDate", Session["vFromDate"].ToString());
                        ReportViewer1.LocalReport.SetParameters(param);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vProductID"] = null;
                        Session["vFromDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Stock Summary"))
                {
                    if (Session["vUnitID"] != null && Session["vFromDate"] != null && Session["vToDate"] != null)
                    {
                        DataTable dataTable = new DataTable();
                        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("[POS].[prcStockSummary]", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@UnitID", Session["vUnitID"]));
                        command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                        command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        connection.Close();
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/StockSummaryReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rds = new ReportDataSource("StockSummaryDataSet", dataTable);
                        ReportViewer1.LocalReport.DataSources.Add(rds);
                        ReportParameter [] param = new ReportParameter[]{new ReportParameter("FromDate", Session["vFromDate"].ToString()),new ReportParameter("ToDate", Session["vToDate"].ToString())};
                        ReportViewer1.LocalReport.SetParameters(param);
                        ReportViewer1.LocalReport.Refresh();
                        Session["vUnitID"] = null;
                        Session["vFromDate"] = null;
                        Session["vToDate"] = null;
                    }
                }

                else if (Session["ReportName"].Equals("Short Stock Report"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[MASTER].[prcProductShortStockReport]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@UnitID", Session["UnitName"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ShortStockReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("ShortStockDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportViewer1.LocalReport.Refresh();
                }

                else if (Session["ReportName"].Equals("Expense Entry Report"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcExpenseEntry]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ExpenseReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("ExpenseEntryDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }

                else if (Session["ReportName"].Equals("Product Wise Profit Report"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcProductWiseProfit]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@ProductID", Session["vProductID"]));
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProductWiseProfitReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("ProductWiseProfitDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds); 
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vProductID"] = null;
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }

                else if (Session["ReportName"].Equals("Date Wise Profit Report"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcDateWiseProfit]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/DateWiseProfitReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("DateWiseProfitDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }

                else if (Session["ReportName"].Equals("Invoice Wise Profit Report"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcInvoiceWiseProfit]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/InvoiceWiseProfitReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("InvoiceWiseProfitDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }

                else if (Session["ReportName"].Equals("Profit & Loss Statement (Unit Wise)"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcProfitAndLossStatementUnitWise]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@unitID", Session["vUnitID"]));
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProfitAndLossStatementUnitWiseReport.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("ProfitAndLossStatementUnitWiseDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vUnitID"] = null;
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }

                else if (Session["ReportName"].Equals("Profit & Loss Statement"))
                {
                    DataTable dataTable = new DataTable();
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("[POS].[prcProfitAndLossStatement]", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@FromDate", Session["vFromDate"]));
                    command.Parameters.Add(new SqlParameter("@ToDate", Session["vToDate"]));
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    connection.Close();
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/ProfitAndLossStatement.rdlc");
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportDataSource rds = new ReportDataSource("ProfitAndLossStatementDataSet", dataTable);
                    ReportViewer1.LocalReport.DataSources.Add(rds);
                    ReportParameter[] param = new ReportParameter[] { new ReportParameter("FromDate", Session["vFromDate"].ToString()), new ReportParameter("ToDate", Session["vToDate"].ToString()) };
                    ReportViewer1.LocalReport.SetParameters(param);
                    ReportViewer1.LocalReport.Refresh();
                    Session["vFromDate"] = null;
                    Session["vToDate"] = null;
                }
            }
        }
    </script>
    <script type="text/javascript">
        function Print() {
            var divToPrint = document.getElementById("ReportViewer1_ctl09");
            newWin = window.open("");
            newWin.document.write(divToPrint.outerHTML);
            newWin.print();
            newWin.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input class="btn btn-primary" type="button" id="print" value="Print" onclick="Print()" />
        <div class="table-responsive">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="false" SizeToReportContent="true" Width="80%"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
