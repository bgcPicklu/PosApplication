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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["userName"] == null)
                return RedirectToAction("Login", "Account");
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "SELECT vUnitName,dli.vUserName,REPLACE(REPLACE(CONVERT(varchar,CONVERT(time,vLoginTime),100),'AM',' AM'),'PM',' PM') vLoginTime FROM INFO.tbDailyLoginInformation dli inner join MASTER.tbUnitInfo ui on dli.vUnitID = ui.vUnitId WHERE CONVERT(date,vLoginTime,105) = CONVERT(date,GETDATE(),105) ORDER BY vUnitName,dli.vUserName";
            SqlCommand command = new SqlCommand(sqlQuery,connection);
            SqlDataReader dataReader = command.ExecuteReader();
            List<DailyLoginInformationModel> lstDlyLoginInfo = new List<DailyLoginInformationModel>();
            while(dataReader.Read())
            {
                DailyLoginInformationModel dlInfo = new DailyLoginInformationModel();
                dlInfo.vUnitName = dataReader["vUnitName"].ToString();
                dlInfo.vUserName = dataReader["vUserName"].ToString();
                dlInfo.vLoginTime = dataReader["vLoginTime"].ToString();
                lstDlyLoginInfo.Add(dlInfo);
            }
            connection.Close();
            return View(lstDlyLoginInfo);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}