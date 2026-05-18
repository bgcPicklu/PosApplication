using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PosApplication.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace PosApplication.Controllers
{
    public class PartyInfoController : Controller
    {
        //
        // GET: /SupplierInfo/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            List<PartyInformationModel> lstPartyInfo = new List<PartyInformationModel>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "select vPartyIdAuto,vPartyId,vPartyName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile,isActive from " +
                "MASTER.tbPartyInfo order by vPartyName";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                PartyInformationModel partyModel = new PartyInformationModel();
                partyModel.vPartyIdAuto = dataReader["vPartyIdAuto"].ToString();
                partyModel.vPartyId = dataReader["vPartyId"].ToString();
                partyModel.vPartyName = dataReader["vPartyName"].ToString();
                partyModel.vAddress = dataReader["vAddress"].ToString();
                partyModel.vPhone = dataReader["vPhone"].ToString();
                partyModel.vMobileNo = dataReader["vMobileNo"].ToString();
                partyModel.vEmail = dataReader["vEmail"].ToString();
                partyModel.vContactPerson = dataReader["vContactPerson"].ToString();
                partyModel.vPersonMobile = dataReader["vPersonMobile"].ToString();
                partyModel.vStatus = dataReader["isActive"].ToString();

                lstPartyInfo.Add(partyModel);
            }
            connection.Close();
            return View(lstPartyInfo);
        }

        //
        // GET: /SupplierInfo/Create
        public ActionResult Create(string partyID)
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            PartyInformationModel modelParty = new PartyInformationModel();
            if (partyID == null)
            {
                partyID = "";
                string sqlQuery = "select ISNULL(MAX(CAST(SUBSTRING(vPartyId,5,LEN(vPartyId)) as int)),0)+1 partyID from MASTER.tbPartyInfo";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelParty.vPartyId = "PAR-" + dataReader["partyID"].ToString();
                }
                dataReader.Close();
                dataReader.Dispose();
            }
            else
            {
                partyID = partyID.Replace(" ", "").Replace("\n", "");
                string sqlQuery = "select vPartyIdAuto,vPartyId,vPartyName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile,isActive from " +
                    "MASTER.tbPartyInfo where vPartyIdAuto = '" + partyID + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    modelParty.vPartyIdAuto = dataReader["vPartyIdAuto"].ToString();
                    modelParty.vPartyId = dataReader["vPartyId"].ToString();
                    modelParty.vPartyName = dataReader["vPartyName"].ToString();
                    modelParty.vAddress = dataReader["vAddress"].ToString();
                    modelParty.vPhone = dataReader["vPhone"].ToString();
                    modelParty.vMobileNo = dataReader["vMobileNo"].ToString();
                    modelParty.vEmail = dataReader["vEmail"].ToString();
                    modelParty.vContactPerson = dataReader["vContactPerson"].ToString();
                    modelParty.vPersonMobile = dataReader["vPersonMobile"].ToString();
                    modelParty.vStatus = dataReader["isActive"].ToString();
                }
            }
            connection.Close();
            return View(modelParty);
        }

        //
        // POST: /SupplierInfo/Create
        [HttpPost]
        public ActionResult Create(PartyInformationModel modelParty)
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "";
                var message = "All information saved successfully.";
                if (modelParty.vPartyIdAuto != null)
                {
                    sqlQuery = "insert into MASTER.tbUDPartyInfo (vPartyIdAuto,vPartyId,vPartyName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile," +
                    "vEditFlag,isActive,vUserName,dEntryTime) select vPartyIdAuto,vPartyId,vPartyName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile," +
                    "'Edit',isActive,vUserName,dEntryTime from MASTER.tbPartyInfo where vPartyIdAuto = '" + modelParty.vPartyIdAuto + "'";
                    sqlQuery += "update MASTER.tbPartyInfo set vPartyId = '" + modelParty.vPartyId + "',vPartyName = '" + modelParty.vPartyName + "', vAddress = '" + modelParty.vAddress + "'," +
                        "vPhone = '" + modelParty.vPhone + "', vMobileNo = '" + modelParty.vMobileNo + "', vEmail = '" + modelParty.vEmail + "'," +
                        "vContactPerson = '" + modelParty.vContactPerson + "', vPersonMobile = '" + modelParty.vPersonMobile + "', isActive = '" + modelParty.vStatus + "'," +
                        "vUserName = '" + Session["userName"] + "', dEntryTime = GETDATE() where vPartyIdAuto = '" + modelParty.vPartyIdAuto + "'";
                    message = "All information updated successfully.";
                }
                else
                {

                    string gu_ID = Guid.NewGuid().ToString();
                    sqlQuery = "insert into MASTER.tbPartyInfo (vPartyIdAuto,vPartyId,vPartyName,vAddress,vPhone,vMobileNo,vEmail,vContactPerson,vPersonMobile," +
                    "isActive,vUserName,dEntryTime) values ('" + gu_ID.ToUpper() + "','" + modelParty.vPartyId + "','" + modelParty.vPartyName + "'," +
                    "'" + modelParty.vAddress + "','" + modelParty.vPhone + "','" + modelParty.vMobileNo + "','" + modelParty.vEmail + "','" + modelParty.vContactPerson + "'," +
                    "'" + modelParty.vPersonMobile + "','" + modelParty.vStatus + "','" + Session["userName"] + "',GETDATE())";
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
