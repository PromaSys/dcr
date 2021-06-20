using System;
using WebApp.App_Code;
using System.Data;

public partial class BI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        string Filters = gc.Req("filters");
        string Search = gc.Req("search");

        dtc = gc.GetTables("exec dcr_sp_bi " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + ",'" + Filters + "','" + Search + "'");

        hgReports.Height = AvailableClientHeight;
        hgReports.GridTable = dtc[0];
        hgReports.Templates = "Category_IDs|" + gc.GetMultipleSelect("selCategories", dtc[1], 1, 150) + "|Subject_IDs|" + gc.GetMultipleSelect("selSubjects", dtc[2], 1, 150) + "|Form_Template_IDs|" + gc.GetMultipleSelect("selTemplates", dtc[3], 1, 150) + "|Template_Field_IDs|" + gc.GetMultipleSelect("selFields", dtc[4], 1, 150) + "|Period|" + gc.GetSelect("selPeriods", true, dtc[5]) + "|Filter_Field_IDs|" + gc.GetMultipleSelect("selFilterFields", dtc[6], 1, 150);
        hgReports.AddSQL = gc.TraceSQL();

        //hgTest.Filters = "Integer|Integer Filter|" + gc.GetSelect("selFilterInteger", false, dtc[2], 3, "123");
        litLeftMenu.Text = "";
        //litLeftMenu.Text += gc.Filter("Category_ID", "Categories", true, dtc[1], 7, Filters);
    }
}