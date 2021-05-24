using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        string Filters = gc.Req("filters");
        string Search = gc.Req("search");

        dtc = gc.GetTables("exec fnd_sp_test " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + ",'" + Filters + "','" + Search + "'");

        hgTest.Height = AvailableClientHeight;
        hgTest.GridTable = dtc[0];
        hgTest.Templates = "Drop_Down_ID|" + gc.GetSelect("selChoices", true, dtc[1]) + "|Multiple_Choice_IDs|" + gc.GetMultipleSelect("selMultipleChoices", dtc[1], 1);

        //hgTest.Filters = "Integer|Integer Filter|" + gc.GetSelect("selFilterInteger", false, dtc[2], 3, "123");
        litLeftMenu.Text = "";
        litLeftMenu.Text += gc.Filter("Drop_Down_ID", "Drop Down", true, dtc[2], 7, Filters);
        litLeftMenu.Text += gc.MultipleSelectFilter("Multiple_Choice_IDs", "Multiple Choice", true, dtc[2], 3, Filters);
    }
}