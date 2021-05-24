using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using WebApp.App_Code;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;

public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // no account
        string VirtualPath = Page.AppRelativeVirtualPath.ToUpper();

        //Response.Cookies["VirtualPath" + DateTime.Now.ToLongTimeString()].Value = VirtualPath;
        //if (Session["UserGUID"] == null && VirtualPath.IndexOf("ACCOUNT/LOGIN") == -1 && VirtualPath.IndexOf("ACCOUNT/FORGOT") == -1 && VirtualPath.IndexOf("ACCOUNT/RESETPASSWORD") == -1 && VirtualPath.IndexOf("ACCOUNT/RESETPASSWORDCONFIRMATION") == -1 && VirtualPath.IndexOf("ACCOUNT/TWOFACTORAUTHENTICATIONSIGNIN") == -1)


        if (Session["UserGUID"] == null && VirtualPath.IndexOf("ACCOUNT/") == -1)
        {
            // redirect to login
            Response.Redirect("~/Account/Login.aspx");
        }

        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                //throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["DeviceMode"] == null)
        {
            Response.Cookies["DeviceMode"].Value = "Mobile";
            Response.Cookies["DeviceMode"].Expires = DateTime.Now.AddDays(1);
        }

        if (Session["LastUpdated"] == null)
        {
            Session["LastUpdated"] = "";
        }

        Session["DevCSS"] = "";

        if (GlobalClass.ConnectionString.ToUpper().IndexOf("_DEV") > -1)
        {
            Session["DevCSS"] = "border: 1px solid white;";
        }

        if (Session["UserGUID"] == null)
        {
            Session["UserGUID"] = Context.User.Identity.GetUserId();
        }

        if (Request.QueryString["l"] != null && Session["InitialUserGroupID"].ToString() == "1")
        {
            // login as                
            if (Application["AuthenticationMode"].ToString() == "Windows")
            {
                Session["UserMenu"] = "";
                Session["UserLoginAs"] = "domain\\" + Request.QueryString["l"].ToString();
            }
            else
            {
                Session["UserLoginAs"] = Request.QueryString["l"].ToString();

                // get user
                GlobalClass gc = new GlobalClass();
                DataTable dt = new DataTable();

                dt = gc.GetTable("select id from ASPNetUsers where UserName = '" + Session["UserLoginAs"].ToString() + "'");

                if (dt != null)
                {
                    if (dt.Rows.Count == 1)
                    {
                        Session["UserGUID"] = dt.Rows[0]["id"].ToString();
                        Session["UserMenu"] = "";
                    }
                }

                dt.Dispose();
                gc = null;
            }
        }

        // no account
        string VirtualPath = Page.AppRelativeVirtualPath.ToUpper();
        //if (Session["UserGUID"] == null && VirtualPath.IndexOf("ACCOUNT/LOGIN") == -1 && VirtualPath.IndexOf("ACCOUNT/FORGOT") == -1 && VirtualPath.IndexOf("ACCOUNT/RESETPASSWORD") == -1 && VirtualPath.IndexOf("ACCOUNT/RESETPASSWORDCONFIRMATION") == -1 && VirtualPath.IndexOf("ACCOUNT/REGISTER") == -1 && VirtualPath.IndexOf("ACCOUNT/TWOFACTORAUTHENTICATIONSIGNIN") == -1)
        if (Session["UserGUID"] == null && VirtualPath.IndexOf("ACCOUNT/") == -1)
        {
            // redirect to login
            Response.Redirect("~/Account/Login.aspx");
        }


        // check if user has access to page
        if (Session["UserMenu"].ToString() != "" && Session["UserGUID"] != null)
        {
            string CurrentPage = Path.GetFileName(Page.AppRelativeVirtualPath.Replace("~", ""));
            if (Session["UserPages"].ToString().IndexOf(CurrentPage, StringComparison.OrdinalIgnoreCase) == -1)
            {
                Response.Write("<div style=\"color: red; padding: 20px; width: 100%; text-align: center;\" ><br /><br />You do not have access to this page.</div>");
                Response.End();
            }

        }


        if (Session["UserMenu"].ToString() == "" && (Session["UserGUID"] != null || Application["AuthenticationMode"].ToString() == "Windows"))
        {
            string WindowsUserIdentity = "";
            if (Application["AuthenticationMode"].ToString() == "Windows")
            {
                WindowsUserIdentity = Page.User.Identity.Name;
                if (Session["UserLoginAs"] != null)
                {
                    if (Session["UserLoginAs"].ToString() != "")
                    {
                        // login as
                        WindowsUserIdentity = Session["UserLoginAs"].ToString();
                    }
                }
                Session["UserGUID"] = "";
            }

            // get menu
            GlobalClass gc = new GlobalClass();
            DataTable dt = new DataTable();
            DataTableCollection dtc = gc.GetTables("exec app_sp_get_user_data_new '" + Session["UserGUID"].ToString() + "','" + WindowsUserIdentity + "'");

            // User menu
            dt = dtc[0];
            Session["UserMenu"] = Server.HtmlDecode(dt.Rows[0]["User_Menu"].ToString());

            Session["UserStartPage"] = dt.Rows[0]["User_Start_Page"].ToString();
            Session["UserPages"] = dt.Rows[0]["User_Pages"].ToString();

            //User data
            dt = dtc[1];
            Session["UserID"] = dt.Rows[0]["User_ID"].ToString();
            Session["UserGroupID"] = dt.Rows[0]["User_Group_ID"].ToString();
            Session["UserLogin"] = dt.Rows[0]["Login"].ToString();
            Session["UserName"] = dt.Rows[0]["UserName"].ToString();
            Session["UserEmail"] = dt.Rows[0]["Email"].ToString();
            Session["UserPhoneNumber"] = dt.Rows[0]["PhoneNumber"].ToString();
            //Session["UserCompany"] = dt.Rows[0]["Company"].ToString();
            //Session["UserCUSTNMBR"] = dt.Rows[0]["CUSTNMBR"].ToString();
            //Session["UserCustomer"] = dt.Rows[0]["Customer"].ToString();
            //Session["UserCustomerID"] = dt.Rows[0]["Customer_ID"].ToString();
            //Session["UserCompanyCUSTNMBR"] = dt.Rows[0]["Company_CUSTNMBR"].ToString();
            //Session["UserTranslation"] = dt.Rows[0]["Translation"].ToString();

            if (Session["InitialUserGroupID"] == null)
            {
                Session["InitialUserGroupID"] = dt.Rows[0]["User_Group_ID"].ToString();
            }

            if (WindowsUserIdentity != "")
            {
                try
                {
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Application["ApplicationDomain"].ToString());

                    var User = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, WindowsUserIdentity);


                    if (User != null)
                    {
                        Session["UserLogin"] = WindowsUserIdentity;
                        Session["UserName"] = User.Name;
                        Session["UserEmail"] = User.EmailAddress;

                        // get additional user fields
                        dt = gc.GetTable("exec app_sp_windows_users '" + WindowsUserIdentity + "', '" + Session["UserName"].ToString() + "','" + Session["UserEmail"].ToString() + "'");

                        if (dt.Rows.Count > 0)
                        {
                            Session["UserGroupID"] = dt.Rows[0]["User_Group_ID"].ToString();
                        }
                    }
                }
                catch (Exception Ex)
                {
                    // get additional user fields
                    dt = gc.GetTable("exec app_sp_windows_users '" + WindowsUserIdentity + "'");

                    if (dt.Rows.Count > 0)
                    {
                        Session["UserGroupID"] = dt.Rows[0]["User_Group_ID"].ToString();
                        Session["UserName"] = dt.Rows[0]["UserName"].ToString();
                        Session["UserEmail"] = dt.Rows[0]["Email"].ToString();
                    }
                }

            }

            if (Session["UserStartPage"].ToString() == "")
            {
                Response.Write("<div style=\"color: red; padding: 20px; width: 100%; text-align: center;\" ><br /><br />You do not have access to this application.</div>");
                Response.End();
            }
            else
            {
                // redirect to user start page
                Response.Redirect("~/" + Session["UserStartPage"].ToString());
            }

            dt.Dispose();
            gc = null;
        }
    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
    }

    /*
    protected override void Render(HtmlTextWriter writer)
    {
        //if (Session["UserTranslation"].ToString() == "") return;

        StringBuilder htmlString = new StringBuilder();
        StringWriter stringWriter = new StringWriter(htmlString);
        HtmlTextWriter tmpWriter = new HtmlTextWriter(stringWriter);
        base.Render(tmpWriter);

        tmpWriter.Close();

        string Contents = tmpWriter.InnerWriter.ToString();

        // Translate      
        if ((Request.Url.AbsoluteUri.ToUpper().IndexOf("/CABLE_NETWORKS") > -1 || Request.Url.AbsoluteUri.ToUpper().IndexOf("/ORDERS") > -1 || Request.Url.AbsoluteUri.ToUpper().IndexOf("/MEDIA") > -1 || Request.Url.AbsoluteUri.ToUpper().IndexOf("/CLIENT_SETUP") > -1) && Session["UserTranslation"].ToString() != "")
        {
            var Translation = Session["UserTranslation"].ToString().Split('|');

            for (int i = 0; i < Translation.Length; i = i + 2)
            {

                Contents = Regex.Replace(Contents, ">" + Translation[i] + " <", ">" + Translation[i + 1] + " <");
                Contents = Regex.Replace(Contents, ">" + Translation[i].ToUpper() + " <", ">" + Translation[i + 1].ToUpper() + " <");

                Contents = Regex.Replace(Contents, ">" + Translation[i] + "<", ">" + Translation[i + 1] + "<");
                Contents = Regex.Replace(Contents, ">" + Translation[i].ToUpper() + "<", ">" + Translation[i + 1].ToUpper() + "<");

                Contents = Regex.Replace(Contents, ">" + Translation[i] + ",", ">" + Translation[i + 1] + ",");
                Contents = Regex.Replace(Contents, ">" + Translation[i].ToUpper() + ",", ">" + Translation[i + 1].ToUpper() + ",");

                Contents = Regex.Replace(Contents, ">" + Translation[i] + " ", ">" + Translation[i + 1] + " ");
                Contents = Regex.Replace(Contents, ">" + Translation[i].ToUpper() + " ", ">" + Translation[i + 1].ToUpper() + " ");
            }
        }
        writer.Flush();

        writer.Write(Contents);
    }
    */
    private string HighlighText(string Source, string SearchText)
    {
        return Regex.Replace(Source, SearchText, new MatchEvaluator(ReplaceKeyWords), RegexOptions.IgnoreCase);
    }

    public string ReplaceKeyWords(Match m)
    {
        //return "<span style='background-color:yellow;'>" + m.Value + "</span>";
        return m.Value;
    }
}
