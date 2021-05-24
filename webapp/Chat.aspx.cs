using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Chat : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));

            GlobalClass gc = new GlobalClass();
            DataTableCollection dtc = null;

            //Available Client Height
            int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 400);

            #region DefaultForm

            if (Request.QueryString.ToString() != "")
            {
                Session["ContextID"] = gc.TryDecrypt(Request.QueryString["cid"].ToString());
                Session["Context"] = Request.QueryString["c"].ToString().Replace("+", "");
            }

            if (Request.Form["txtContextID"] != null && Request.Form["txtContextID"] != "")
            {
                Session["ContextID"] = gc.TryDecrypt(Request.Form["txtContextID"].ToString());
            }

            if (Request.Form["txtContext"] != null && Request.Form["txtContext"] != "")
            {
                Session["Context"] = gc.TryDecrypt(Request.Form["txtContext"].ToString());
            }

            #endregion DefaultForm

            dtc = gc.GetTables("exec app_sp_chat " + Session["ContextID"].ToString() + ",'" + Session["Context"].ToString() + "'");

            string DialogWidth = Request.QueryString["w"].ToString();
            string DialogHeight = Request.QueryString["h"].ToString();
            string DialogTitle = Request.QueryString["t"].ToString();

            Session["ChatHeight"] = Convert.ToInt32(DialogHeight) - 315;

            ChatComment.Title = DialogTitle;
            //ChatComment.Height = Convert.ToInt32(DialogHeight) - 130;
            ChatComment.Width = Convert.ToInt32(DialogWidth);
            ChatComment.EncryptionKey = Application["EncryptionKey"].ToString();
            ChatComment.Hide = "Chat_ID";

            ChatComment.GridTable = dtc[0];

            ChatComment.Widths = "";
            ChatComment.Formats = "";
            ChatComment.AddSQL = "Context|'" + Session["Context"].ToString() + "'|Context_ID|" + Session["ContextID"].ToString() + "|Created_By |'" + Session["UserName"].ToString() + "'";

            if (dtc[1].Rows.Count > 0)
            {
                litChat.Text = dtc[1].Rows[0]["Chat"].ToString();
            }

            gc = null;
        }
    }
}