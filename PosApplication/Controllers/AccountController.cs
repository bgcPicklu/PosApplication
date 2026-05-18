using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using PosApplication.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace PosApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            int validity = 0;
            string keyCode = "";
            string sqlQuery = "SELECT DATEDIFF(DD,GETDATE(),vValidity) validity,vKeyCode from INFO.tbChkSoft";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                validity = Convert.ToInt16(dataReader["validity"].ToString());
                keyCode = dataReader["vKeyCode"].ToString();
            }
            dataReader.Close();
            dataReader.Dispose();
            connection.Close();
            if(validity >= 0)
            {
                if (keyCode.Equals("31479F0C-37A2-4738-B0C1-A54001B09BC4"))
                {
                    sqlQuery = "SELECT l.vUnitName,u.vUnitName vUnit,l.vUserName,vPassword,vAccountType,vStatus,l.vEmail FROM INFO.tbLogin l inner join MASTER.tbUnitInfo u on l.vUnitName = u.vUnitId where l.vUserName = '" + model.UserName + "' and vPassword = '" + model.Password + "' and l.vUnitName = '" + model.UnitID + "' and vStatus = 'Active'";
                    connection.Open();
                    command.CommandText = sqlQuery;
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        Session["UnitName"] = dataReader["vUnitName"].ToString();
                        Session["Unit"] = dataReader["vUnit"].ToString();
                        Session["userName"] = dataReader["vUserName"].ToString();
                        Session["Password"] = dataReader["vPassword"].ToString();
                        Session["AccountType"] = dataReader["vAccountType"].ToString();
                        Session["vStatus"] = dataReader["vStatus"].ToString();
                        Session["vEmail"] = dataReader["vEmail"].ToString();
                    }
                    dataReader.Close();
                    dataReader.Dispose();
                    connection.Close();
                    if (Session["userName"] != null)
                    {
                        connection.Open();
                        sqlQuery = "select MenuID, iShow from INFO.tbUserAuthentication where vUnitID = '" + model.UnitID + "' and vUserName = '" + model.UserName + "'";
                        command.CommandText = sqlQuery;
                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            Session[dataReader["MenuID"].ToString()] = dataReader["iShow"].ToString();
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                        connection.Close();
                        connection.Open();
                        string ipAddress = Request.UserHostAddress;
                        sqlQuery = "insert into INFO.tbDailyLoginInformation (vUnitID,vUserName,vLoginTime,vUserMachineAddress) values ('" + Session["UnitName"] + "','" + Session["userName"] + "',GETDATE(),'" + ipAddress + "')";
                        command.CommandText = sqlQuery;
                        command.ExecuteNonQuery();
                        connection.Close();
                        return RedirectToLocal("~/Home/Index");//return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                connection.Open();
                command.CommandText = "update INFO.tbChkSoft set vKeyCode = '" + Guid.NewGuid().ToString().ToUpper() + "'";
                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToLocal("~/Home/Index");
            }
            return View(model);//Json(new { success = false }, JsonRequestBehavior.AllowGet);
            // If we got this far, something failed, redisplay form
            //return View(model);
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            if (Session["userName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public JsonResult Register(RegisterViewModel model)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString);
            connection.Open();
            string sqlQuery = "insert into INFO.tbLogin(vUserName,vPassword,vAccountType) value ('" + model.UserName + "','" + model.Password + "','" + model.AccountType + "')";
            SqlCommand command = new SqlCommand(sqlQuery,connection);
            command.ExecuteNonQuery();
            connection.Close();
            // If we got this far, something failed, redisplay form
            return Json(new { success = true}, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}