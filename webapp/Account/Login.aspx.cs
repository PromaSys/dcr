using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using WebApp.Models;
using WebApp;
using WebApp.App_Code;

public partial class Account_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        GlobalClass gc = new GlobalClass();

        string[] BackgroundImages = (string[])Application["BackgroundImages"];

        int value = new Random().Next(0, BackgroundImages.Length);

        divLogin.Attributes.Add("style", "width: 100%; background: url('../images/" + BackgroundImages[value] + "'); background-size:cover;");

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1200);

        // set panel height
        divLogin.Height = AvailableClientHeight;

        //RegisterHyperLink.NavigateUrl = "Register";
        // Enable this once you have account confirmation enabled for password reset functionality
        ForgotPasswordHyperLink.NavigateUrl = "Forgot";
        OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
        var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        if (!String.IsNullOrEmpty(returnUrl))
        {
            //RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
        }

        Session["UserMenu"] = "";
    }

    protected void LogIn(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Validate the user password
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

            // This doen't count login failures towards account lockout
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                    break;
                case SignInStatus.LockedOut:
                    //Response.Redirect("~/Account/Lockout");
                    FailureText.Text = "You have exceeded the number of allowed consecutive login attempts.<br />Your account has been locked up.<br />Please try again after 5 minutes.<br /><br />";
                    ErrorMessage.Visible = true;
                    break;
                case SignInStatus.RequiresVerification:
                    Response.Redirect(String.Format("~/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}", Request.QueryString["ReturnUrl"], RememberMe.Checked), true);
                    break;
                case SignInStatus.Failure:
                default:
                    FailureText.Text = "Invalid login attempt";
                    ErrorMessage.Visible = true;
                    break;
            }
        }
    }
}