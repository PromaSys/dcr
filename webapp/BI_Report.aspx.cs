using System;
using WebApp.App_Code;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.IO;

public partial class Subjects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack || Request.Form["filters"] != "")
        //{
        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        string Filters = gc.Req("filters");
        string BIID = gc.Req("BIID");

        if(BIID == "" && Request.Cookies["BIID"] != null)
        {
            BIID = gc.TryDecrypt(Request.Cookies["BIID"].Value);
        }

        if (BIID != "")
        {
            Session["BIID"] = BIID;
        }

        if (Session["BIID"] == null)
        {
            Session["BIID"] = "1";
        }

        dtc = gc.GetTables("exec dcr_sp_rs_form_fields " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + "," + Session["BIID"].ToString() + ",'" + Filters + "'");

        foreach (DataRow dr in dtc[0].Select("Format<>'' and (Field_Type='Money' or Field_Type='Number')")) {
            dr["Value"] = String.Format("{0:" + dr["Format"].ToString() + ",}", Convert.ToDouble(dr["Value"].ToString()));
        }

        foreach (DataRow dr in dtc[0].Select("Format<>'' and Field_Type='Date'"))
        {
            dr["Value"] = String.Format("{0:" + dr["Format"].ToString() + ",}", Convert.ToDouble(dr["Value"].ToString()));
        }

        foreach (DataRow dr in dtc[0].Select("Format<>'' and Field_Type='Date'"))
        {
            dr["Value"] = String.Format("{0:" + dr["Format"].ToString() + ",}", Convert.ToDateTime(dr["Value"].ToString()));
        }

        if (Filters == "")
        {
            // set start and end dates
            Filters = "Start_Date|" + String.Format("{0:M/d/yyyy}", dtc[0].Rows[0]["Start_Date"]) + "|End_Date |" + String.Format("{0:M/d/yyyy}", dtc[0].Rows[0]["End_Date"]);
        }
            
        litLeftMenu.Text = "";
        litLeftMenu.Text += gc.FilterDate("Start_Date", "Start Date", Filters) + "<br />";
        litLeftMenu.Text += gc.FilterDate("End_Date", "End Date", Filters) + "<br />";

        // iterate through field filters
        foreach (DataRow dr in dtc[1].Rows)
        {
            string FilterField = dr["Field"].ToString();
            litLeftMenu.Text += gc.MultipleSelectFilter(FilterField, FilterField, true, dtc[2].Select("Field='" + FilterField + "'").CopyToDataTable(), 5, Filters) + "<br />";
        }

    //litLeftMenu.Text += gc.MultipleSelectFilter("Subject_Category_ID", "Category", true, dtc[2].Select("Field='" + "Subject_Category_ID" + "'").CopyToDataTable(), 5, Filters) + "<br />";

    // report viewer

    ReportViewer1.ProcessingMode = ProcessingMode.Local;
        ReportViewer1.AsyncRendering = false;
        ReportViewer1.SizeToReportContent = false;

        ReportViewer1.Height = AvailableClientHeight;

        //Session["ReportHeight"] = ReportViewer1.Height;

        LocalReport rep = ReportViewer1.LocalReport;
        rep.ReportPath = "reports/BI Report.rdl";
        rep.EnableHyperlinks = true;
        rep.DataSources.Clear();

        rep.DataSources.Add(new ReportDataSource("dcr_sp_rs_form_fields", dtc[0]));
        rep.Refresh();
        //}
    }
}