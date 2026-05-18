using PosApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace PosApplication.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login","Account");
            return View();
        }

        //
        // GET: /Register/Create
        public ActionResult Create()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        //
        // POST: /Register/Create
        [HttpPost]
        public ActionResult Create(RegisterModel modelRegister)
        {
            try
            {
                // TODO: Add insert logic here
                if (Session["userName"] == null)
                    return RedirectToAction("Login", "Account");

                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
                connection.Open();
                string sqlQuery = "select count(*) count_Row from INFO.tbLogin where vUnitName = '" + modelRegister.UnitID + "' and vUserName = '" + modelRegister.UserName + "'";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                int count = 0;
                var message = "Create new user successfully";
                while(dataReader.Read())
                {
                    count = Convert.ToInt16(dataReader["count_Row"].ToString());
                }
                dataReader.Close();
                dataReader.Dispose();
                if(count == 0)
                {
                    sqlQuery = "insert into INFO.tbLogin(vUnitName,vUserName,vPassword,vAccountType,vStatus,dEntryTIme) values ('" + modelRegister.UnitID + "',"+
                    "'" + modelRegister.UserName + "','" + modelRegister.Password + "','" + modelRegister.AccountType + "','" + modelRegister.Status + "',GETDATE())";
                }
                else
                {
                    sqlQuery = "update INFO.tbLogin set vUnitName = '" + modelRegister.UnitID + "',vPassword = '" + modelRegister.Password + "',vAccountType = '" + modelRegister.AccountType + "',"+
                        "vStatus = '" + modelRegister.Status + "',dEntryTIme = GETDATE() where vUserName = '" + modelRegister.UserName + "'";
                    message = "User information updated successfully";
                }
                command.CommandText = sqlQuery;
                command.ExecuteNonQuery();
                connection.Close();
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception exp)
            {
                return Json(new { success = false, message = exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
