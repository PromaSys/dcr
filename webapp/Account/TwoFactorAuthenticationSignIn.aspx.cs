using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApp;
using WebApp.Models;
using WebApp.App_Code;

public partial class TwoFactorAuthenticationSignIn : System.Web.UI.Page
{
    private ApplicationSignInManager signinManager;
    private ApplicationUserManager manager;

    public TwoFactorAuthenticationSignIn()
    {
        manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        signinManager =  Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

    }

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

        var userId = signinManager.GetVerifiedUserId<ApplicationUser, string>();
        if (userId == null)
        {
            //Response.Redirect("~/Account/Error", true);
            FailureText.Text = "The user cannot be verified.";
            ErrorMessage.Visible = true;
            Response.End();
        }
        var userFactors = manager.GetValidTwoFactorProviders(userId);
        Providers.DataSource = userFactors.Select(x => x).ToList();
        Providers.DataBind();
        
        // select provider automatically, if only one
        if (userFactors.Count == 1 && Providers.Visible)
        {
            ProviderSubmit_Click(this, new EventArgs());
        }
        
    }

    protected void CodeSubmit_Click(object sender, EventArgs e)
    {
        bool rememberMe = false;
        bool.TryParse(Request.QueryString["RememberMe"], out rememberMe);

        var result = signinManager.TwoFactorSignIn<ApplicationUser, string>(SelectedProvider.Value, Code.Text, isPersistent: rememberMe, rememberBrowser: RememberBrowser.Checked);
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
            case SignInStatus.Failure:
            default:
                FailureText.Text = "Invalid code";
                ErrorMessage.Visible = true;
                break;
        }
    }

    protected void ProviderSubmit_Click(object sender, EventArgs e)
    {

        if(Providers.SelectedValue.ToString() == "Email Code")
        {
            lblCode.Text = "To verify your identity, we sent you a code by email.<br />Please check your email and enter that code below.";
        }
        else if (Providers.SelectedValue.ToString() == "Phone Code")
        {
            lblCode.Text = "To verify your identity, we sent a code to your phone.<br />Please check your phone and enter that code below.";
        }


        if (!signinManager.SendTwoFactorCode(Providers.SelectedValue))
        {
            //Response.Redirect("~/Account/Error");
            FailureText.Text = "Error sending two-factor code.<br />";
            ErrorMessage.Visible = true;
        }

        var user = manager.FindById(signinManager.GetVerifiedUserId<ApplicationUser, string>());
        if (user != null)
        {
            var code = manager.GenerateTwoFactorToken(user.Id, Providers.SelectedValue);
        }

        SelectedProvider.Value = Providers.SelectedValue;
        sendcode.Visible = false;
        verifycode.Visible = true;
    }
}