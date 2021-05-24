using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string LogoutReason = "";

        if (Request.Cookies["LogoutReason"] != null)
        {
            LogoutReason = Request.Cookies["LogoutReason"].Value;
        }

        /*

        if (LogoutReason != "Expiry")
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        Response.Cache.SetValidUntilExpires(false);
        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();

        HttpContext.Current.Session.Clear();
        HttpContext.Current.Session.Abandon();
        //System.Web.Security.FormsAuthentication.SignOut();
        */

        if (LogoutReason == "")
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            //System.Web.Security.FormsAuthentication.SignOut();

            //Response.Redirect("~/account/login");
        }
    }
}