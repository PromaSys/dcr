using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserStartPage"] != null)
        {
            // redirect to user start page
            Response.Redirect("~/" + Session["UserStartPage"].ToString());
        }
        else
        {
            Response.Redirect("~/Account/Login");
        }
    }
}