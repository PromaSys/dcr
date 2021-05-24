using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.UI.WebControls;
using WebApp;
using WebApp.App_Code;
using WebApp.Models;

public partial class ForgotPassword : Page
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
    }

    protected void Forgot(object sender, EventArgs e)
    {
        if (IsValid)
        {
            // Validate the user's email address
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //ApplicationUser user = manager.FindByName(Email.Text);
            ApplicationUser user = manager.FindByEmail(Email.Text);
            if (user == null || !manager.IsEmailConfirmed(user.Id))
            {
                FailureText.Text = "Cannot find a user with this email or login.";
                ErrorMessage.Visible = true;
                return;
            }
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send email with the code and the redirect to reset password page
            string code = manager.GeneratePasswordResetToken(user.Id);
            string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request);
            //string callbackUrl = Application["ApplicationURL"].ToString() + "account/resetpassword?code=" + code;

            //manager.SendEmail(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");

            GlobalClass gc = new GlobalClass();
            gc.SendEmail(user.Email, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");
            gc = null;

            loginForm.Visible = false;
            DisplayEmail.Visible = true;
        }
    }
}