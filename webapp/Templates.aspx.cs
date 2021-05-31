using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;
public partial class Templates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        string Filters = gc.Req("filters");
        string Search = gc.Req("search");

        dtc = gc.GetTables("exec dcr_sp_templates " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + ",'" + Filters + "','" + Search + "'");

        hgTemplates.Height = AvailableClientHeight;
        hgTemplates.GridTable = dtc[0];
        hgTemplates.Templates = "Subject_Category_IDs|" + gc.GetMultipleSelect("selCategories", dtc[2], 3, 150);
        //hgTemplates.AddSQL = gc.TraceSQL() + "Category_ID";

        //hgTest.Filters = "Integer|Integer Filter|" + gc.GetSelect("selFilterInteger", false, dtc[2], 3, "123");
        litLeftMenu.Text = "";
        litLeftMenu.Text += gc.Filter("Category_ID", "Categories", true, dtc[1], 7, Filters);
        //litLeftMenu.Text += gc.MultipleSelectFilter("Multiple_Choice_IDs", "Multiple Choice", true, dtc[2], 3, Filters);
    }
}