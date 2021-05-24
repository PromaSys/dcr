using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
using System.Web.Configuration;
using WebApp;
using WebApp.App_Code;
using System.Web.UI;
using System.Data;

namespace WebApp
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            GlobalClass gc = new GlobalClass();
            DataTableCollection dtc = null;

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

            AuthenticationSection authSection = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");
            Application["AuthenticationMode"] = authSection.Mode.ToString();

            Application["ApplicationName"] = ConfigurationManager.AppSettings["ApplicationName"].ToString();
            Application["FromEmail"] = ConfigurationManager.AppSettings["FromEmail"].ToString();
            Application["SMTP"] = ConfigurationManager.AppSettings["SMTP"].ToString();
            Application["SMTPPort"] = ConfigurationManager.AppSettings["SMTPPort"].ToString();
            Application["EnableSSL"] = ConfigurationManager.AppSettings["EnableSSL"].ToString();
            Application["EmailUser"] = ConfigurationManager.AppSettings["EmailUser"].ToString();
            Application["EmailPassword"] = ConfigurationManager.AppSettings["EmailPassword"].ToString();
            Application["EmailCC"] = ConfigurationManager.AppSettings["EmailCC"].ToString();
            Application["EncryptionKey"] = ConfigurationManager.AppSettings["EncryptionKey"].ToString();
            
            Application["ApplicationLogo"] = ConfigurationManager.AppSettings["ApplicationLogo"].ToString();
            Application["ApplicationMenuLogo"] = ConfigurationManager.AppSettings["ApplicationMenuLogo"].ToString();
            Application["ApplicationURL"] = ConfigurationManager.AppSettings["URL"].ToString();
            Application["ApplicationTutorialLink"] = ConfigurationManager.AppSettings["ApplicationTutorialLink"].ToString();
            Application["ApplicationAddress"] = ConfigurationManager.AppSettings["ApplicationAddress"].ToString();

            Application["ApplicationDomain"] = ConfigurationManager.AppSettings["Domain"].ToString();

            Application["MaximumExportColumns"] = ConfigurationManager.AppSettings["MaximumExportColumns"].ToString();

            //GlobalClass gc = new GlobalClass();
            GlobalClass.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"].ToString();

            // connection string
            GlobalClass.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            // email
            GlobalClass.FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
            GlobalClass.SMTP = ConfigurationManager.AppSettings["SMTP"].ToString();
            GlobalClass.SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
            GlobalClass.EnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"].ToString());
            GlobalClass.EmailUser = ConfigurationManager.AppSettings["EmailUser"].ToString();
            GlobalClass.EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
            GlobalClass.EmailCC = ConfigurationManager.AppSettings["EmailCC"].ToString();

            // backgroound images
            string[] BackgroundImages = new string[3];
            BackgroundImages[0] = ConfigurationManager.AppSettings["BackgroundImage1"].ToString();
            BackgroundImages[1] = ConfigurationManager.AppSettings["BackgroundImage2"].ToString();
            BackgroundImages[2] = ConfigurationManager.AppSettings["BackgroundImage3"].ToString();
            Application["BackgroundImages"] = BackgroundImages;

            Application["ThemeIsSet"] = bool.Parse(ConfigurationManager.AppSettings["ThemeIsSet"].ToString());

        }

        void Session_Start(object sender, EventArgs e)
        {
            Session["UserMenu"] = "";
            Session.Timeout = 60;
            //GlobalClass gc = new GlobalClass();
            //gc.GetTables("exec app_sp_start")

            Session["UserCartCount"] = "(0 items)";

            if (Request.Url.AbsolutePath.ToUpper().EndsWith("/HOME"))
            {
                Response.Redirect("~/Home");
            }
            else
            {
                Response.Redirect("~/Account/Login");
            }
            //Response.Redirect("Home");
        }

        void Session_End(object sender, EventArgs e)
        {
            Session["UserMenu"] = "";
        }
    }
}