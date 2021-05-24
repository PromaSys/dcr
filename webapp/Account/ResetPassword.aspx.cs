using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApp;
using WebApp.App_Code;

public partial class ResetPassword : Page
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

    protected string StatusMessage
    {
        get;
        private set;
    }

    protected void Reset_Click(object sender, EventArgs e)
    {
        string code = IdentityHelper.GetCodeFromRequest(Request);
        if (code != null)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = manager.FindByEmail(Email.Text);
            if (user == null)
            {
                ErrorMessage.Text = "No user found";
                return;
            }
            var result = manager.ResetPassword(user.Id, code, Password.Text);
            if (result.Succeeded)
            {
                Response.Redirect("~/Account/ResetPasswordConfirmation");
                return;
            }
            ErrorMessage.Text = result.Errors.FirstOrDefault();
            return;
        }

        ErrorMessage.Text = "An error has occurred";
    }
}