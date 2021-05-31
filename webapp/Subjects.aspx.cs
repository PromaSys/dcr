using System;
using WebApp.App_Code;
using System.Data;

public partial class Subjects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        string Filters = gc.Req("filters");
        string Search = gc.Req("search");

        dtc = gc.GetTables("exec dcr_sp_subjects " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + ",'" + Filters + "','" + Search + "'");

        hgSubjects.Height = AvailableClientHeight;
        hgSubjects.GridTable = dtc[0];
        hgSubjects.Templates = "Category_ID|" + gc.GetSelect("selCategory", true, dtc[1]);

        //hgTest.Filters = "Integer|Integer Filter|" + gc.GetSelect("selFilterInteger", false, dtc[2], 3, "123");
        litLeftMenu.Text = "";
        litLeftMenu.Text += gc.Filter("Category_ID", "Categories", true, dtc[1], 7, Filters);
        //litLeftMenu.Text += gc.MultipleSelectFilter("Multiple_Choice_IDs", "Multiple Choice", true, dtc[2], 3, Filters);
    }
}