using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using WebApp;
using WebApp.App_Code;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Reporting.WebForms;


public partial class Process_Request : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));

        string RequestAction = "";
        string RequestQS = "";

        if (Request.RequestType == "GET")
        {
            RequestQS = GetChange(Request.QueryString.ToString());
        }
        else if (Request.RequestType == "POST")
        {
            Request.InputStream.Position = 0;
            StreamReader reader = new StreamReader(Request.InputStream);
            RequestQS = reader.ReadToEnd();
            RequestQS = GetChange(RequestQS);
        }

        if (RequestQS != "")
        {
            GlobalClass gc = new GlobalClass();
            DataTable dt = new DataTable();
            DataTableCollection dtc;
            NameValueCollection req = HttpUtility.ParseQueryString(RequestQS);

            RequestAction = req["action"];

            if (RequestAction != "")
            {
                Response.ContentType = "text/json";

                if (RequestAction == "Unlock")
                {
                    try
                    {
                        string Id = gc.GetTable("select Id from AspNetUsers where User_ID = " + gc.TryDecrypt(req["kfv"])).Rows[0]["Id"].ToString();

                        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        IdentityResult result = manager.SetLockoutEndDate(Id, DateTimeOffset.MinValue);

                        if (result.Succeeded)
                        {
                            result = manager.ResetAccessFailedCount(Id);
                        }

                        if (result.Succeeded)
                        {
                            Response.Write("{\"message\": \"Account unlocked\", \"messagetype\": \"success\", \"kfv\": \"" + req["kfv"] + "\", \"tableid\": \"" + req["tableid"] + "\", \"tdindex\": \"" + req["tdindex"] + "\"}");
                        }
                        else
                        {
                            string ErrorMessage = result.Errors.FirstOrDefault();
                            Response.Write("{\"message\": \"" + ErrorMessage + "\", \"messagetype\": \"danger\"}");
                        }

                        manager.Dispose();

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }

                else if (RequestAction == "ResetPassword")
                {
                    try
                    {
                        string Password = req["password"];
                        string OldPassword = req["oldpassword"];

                        dt = gc.GetTable("select Id from AspNetUsers where User_ID = " + gc.TryDecrypt(req["kfv"]));

                        string Id = dt.Rows[0]["Id"].ToString();

                        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                        // validate password
                        var newPasswordHash = manager.PasswordHasher.HashPassword(Password);
                        ApplicationUser user = manager.FindById(Id);

                        // check old password
                        if (OldPassword != "" && OldPassword != null)
                        {
                            var signinmanager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
                            var validcredentials = signinmanager.UserManager.CheckPassword(user, OldPassword);
                            if (!validcredentials)
                            {
                                Response.Write("{\"message\": \"The old password is not valid.\", \"messagetype\": \"danger\"}");
                                signinmanager.Dispose();
                                return;
                            }
                        }

                        PasswordValidator pv = manager.PasswordValidator as PasswordValidator;

                        //Password validation
                        string ValidationMessage = "";

                        if (Password.Length < pv.RequiredLength)
                        {
                            ValidationMessage += "Password is shorter than the minimum length required (" + pv.RequiredLength.ToString() + ").<br/>";
                        }

                        if (new Regex("[A-Z]").Matches(Password).Count == 0 && pv.RequireUppercase)
                        {
                            ValidationMessage += "Passwords must have at least one character that is upper case.<br/>";
                        }

                        if (new Regex("[a-z]").Matches(Password).Count == 0 && pv.RequireLowercase)
                        {
                            ValidationMessage += "Passwords must have at least one character that is lower case.<br/>";
                        }

                        if (new Regex("[0-9]").Matches(Password).Count == 0 && pv.RequireDigit)
                        {
                            ValidationMessage += "Passwords must have at least one character that is a number.<br/>";
                        }

                        if (new Regex("[^0-9a-zA-Z]").Matches(Password).Count == 0 && pv.RequireNonLetterOrDigit)
                        {
                            ValidationMessage += "Passwords must have at least one character that is not a letter or a number.<br/>";
                        }

                        if (ValidationMessage != "")
                        {
                            ValidationMessage += "Please edit the password and try again.";
                            Response.Write("{\"message\": \"" + ValidationMessage + "\", \"messagetype\": \"danger\"}");
                            manager.Dispose();
                            return;
                        }

                        user.PasswordHash = newPasswordHash;

                        IdentityResult result = manager.Update(user);

                        if (result.Succeeded)
                        {
                            Response.Write("{\"message\": \"Password changed\", \"messagetype\": \"success\", \"kfv\": \"" + req["kfv"] + "\", \"tableid\": \"" + req["tableid"] + "\", \"tdindex\": \"" + req["tdindex"] + "\"}");
                        }
                        else
                        {
                            string ErrorMessage = result.Errors.FirstOrDefault();
                            Response.Write("{\"message\": \"" + ErrorMessage + "\", \"messagetype\": \"danger\"}");
                        }

                        manager.Dispose();

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }
                else if (RequestAction == "EmailResetLink")
                {
                    try
                    {
                        dt = gc.GetTable("select Id from AspNetUsers where User_ID = " + gc.TryDecrypt(req["kfv"]));
                        string Id = dt.Rows[0]["Id"].ToString();

                        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        string code = manager.GeneratePasswordResetToken(Id);
                        string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request);

                        //string callbackUrl = Application["ApplicationURL"].ToString() + "account/resetpassword?code=" + code;

                        ApplicationUser user = manager.FindById(Id);

                        //manager.SendEmail(Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                        string ErrorMessage = gc.SendEmail(user.Email, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                        if (ErrorMessage == "")
                        {
                            Response.Write("{\"message\": \"Email to reset password has been queued.\", \"messagetype\": \"success\"}");
                        }
                        else
                        {
                            Response.Write("{\"message\": \"" + ErrorMessage + "\", \"messagetype\": \"danger\"}");
                        }

                        manager.Dispose();

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }
                else if (RequestAction == "EmailWelcomeLink")
                {
                    try
                    {
                        dt = gc.GetTable("select Id, Name from AspNetUsers where User_ID = " + gc.TryDecrypt(req["kfv"]));
                        string Id = dt.Rows[0]["Id"].ToString();

                        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        string code = manager.GeneratePasswordResetToken(Id);
                        string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request);

                        ApplicationUser user = manager.FindById(Id);

                        string UserName = dt.Rows[0]["Name"].ToString();
                        string UserLogin = user.UserName;

                        dt = gc.GetTable("select Template from app_Templates where Name ='Welcome Email'");
                        string Template = dt.Rows[0]["Template"].ToString();

                        Template = Template.Replace("[USER NAME]", UserName);
                        Template = Template.Replace("[USER LOGIN]", UserLogin);
                        Template = Template.Replace("[WELCOME EMAIL LINK]", "<a href=\"" + callbackUrl + "\">here</a>");
                        Template = Template.Replace("[APPLICATION TUTORIAL LINK]", Application["ApplicationTutorialLink"].ToString());
                        Template = Template.Replace("[APPLICATION NAME]", Application["ApplicationName"].ToString());
                        Template = Template.Replace("[APPLICATION ADDRESS]", Application["ApplicationAddress"].ToString());
                        // Message
                        string Message = Template;

                        string ErrorMessage = gc.SendEmail(user.Email, "Welcome to " + Application["ApplicationName"].ToString(), Message);

                        if (ErrorMessage == "")
                        {
                            Response.Write("{\"message\": \"Welcome Email has been queued.\", \"messagetype\": \"success\"}");
                        }
                        else
                        {
                            Response.Write("{\"message\": \"" + ErrorMessage + "\", \"messagetype\": \"danger\"}");
                        }


                        manager.Dispose();

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }
                else if (RequestAction == "RenewSession")
                {
                    Response.Write("{\"message\": \"Session Renewed.\", \"messagetype\": \"success\"}");
                }
                else if (RequestAction == "Login")
                {
                    try
                    {
                        string Email = req["email"];
                        string Password = req["password"];
                        string RememberMe = req["rememberme"];
                        bool RememberMeChecked = false;

                        if (RememberMe == "true")
                        {
                            RememberMeChecked = true;
                        }

                        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                        var result = signinManager.PasswordSignIn(Email, Password, RememberMeChecked, shouldLockout: false);

                        switch (result)
                        {
                            case SignInStatus.Success:
                                dt = gc.GetTable("select Id from AspNetUsers where UserName = '" + Email + "'");
                                Session["UserGUID"] = dt.Rows[0]["Id"].ToString();
                                Session["UserMenu"] = "";

                                gc.GetTable("exec wba_sp_new_destination_reset '" + Session["UserGUID"].ToString() + "'");

                                Response.Write("{\"message\": \"\", \"messagetype\": \"success\"}");
                                break;
                            case SignInStatus.LockedOut:
                                Response.Redirect("/Account/Lockout");
                                break;
                            case SignInStatus.RequiresVerification:
                                Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                                Request.QueryString["ReturnUrl"],
                                                                RememberMeChecked),
                                                  true);
                                break;
                            case SignInStatus.Failure:
                            default:
                                Response.Write("{\"message\": \"Invalid sign in attempt.\", \"messagetype\": \"danger\"}");
                                break;
                        }



                        manager.Dispose();

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }



                else if (RequestAction == "DownloadCSV")
                {

                    string FileName = req["filename"];
                    string BlockID = req["blockid"];

                    FileName += ".csv";
                    dt = gc.GetTable("exec fnd_sp_csv " + BlockID);

                    if (dt.Rows.Count == 0)
                    {
                        Response.Write("No data to download");
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {

                        string csv = dt.Rows[0]["csv"].ToString();

                        dt.Dispose();
                        gc = null;



                        Response.Buffer = true;
                        Response.Clear();
                        //Response.ContentType = "application/vnd.ms-excel";  
                        Response.ContentType = "text/csv";
                        //Response.AddHeader("content-disposition", "Attachment; filename=" + FileName + ".xls");
                        Response.AddHeader("content-disposition", "Attachment; filename=" + FileName);
                        //Response.AddHeader("Content-Length", bytes.Length.ToString());
                        //Response.BinaryWrite(bytes);
                        Response.Write(csv);
                        Response.Flush();
                        Response.End();
                    }
                }

                else if (RequestAction == "UpdateAddSQL")
                {
                    try
                    {
                        string fn = gc.TryDecrypt(req["fn"]);
                        string fv = gc.TryDecrypt(req["fv"]);
                        string AS = gc.TryDecrypt(req["as"]);

                        AS = gc.Encrypt(AS + "|" + fn + "|" + fv);


                        Response.Write("{\"AS\": \"" + AS + "\", \"messagetype\": \"success\"}");
                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }


                }
                else if (RequestAction == "PreviewTheme")
                {
                    try
                    {
                        var ThemeId = gc.TryDecrypt(req["kfv"]);
                        dtc = gc.GetTables("exec app_sp_get_stylesheet " + ThemeId.ToString());
                        Application["Theme"] = dtc[0].Rows[0][1].ToString();
                        Response.Write("{\"AS\": \"" + "Theme preview success" + "\", \"messagetype\": \"success\"}");
                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }
                else if (RequestAction == "SetTheme")
                {
                    try
                    {
                        var ThemeId = gc.TryDecrypt(req["kfv"]);
                        dtc = gc.GetTables("exec app_sp_get_stylesheet " + ThemeId.ToString());
                        Application["Theme"] = null;
                        // Edit foundation and navbar.css files.
                        File.WriteAllText(HttpContext.Current.Server.MapPath("~/Content/foundation.css"), dtc[0].Rows[0][2].ToString());
                        File.WriteAllText(HttpContext.Current.Server.MapPath("~/Content/navbar_primary.css"), dtc[0].Rows[0][3].ToString());

                        Application["ThemeIsSet"] = true;
                        gc.ReplaceXMLNodeValue(HttpContext.Current.Server.MapPath("~/web.config"), "//add[@key='ThemeIsSet']", "value", "True");
                        Response.Write("{\"AS\": \"" + "Theme set success" + "\", \"messagetype\": \"success\"}");

                    }
                    catch (Exception Ex)
                    {
                        Response.Write("{\"message\": \"" + Ex.Message + "\", \"messagetype\": \"danger\"}");
                    }
                }
            }

            dt.Dispose();
            gc = null;
        }
    }

    private string GetChange(string Text)
    {
        Text = Server.UrlDecode(Text);
        Text = Text.Replace("-[_", "<").Replace("_]-", ">");
        return Text;
    }
    /*
    private string GetJSON(string t)
    {
        var j = new JavaScriptSerializer().Serialize(t);
    }
    */
}