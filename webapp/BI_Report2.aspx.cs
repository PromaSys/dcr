using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;
using Microsoft.Reporting.WebForms;

public partial class BI_Report2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GlobalClass gc = new GlobalClass();
            DataTableCollection dtc = null;

            //Available Client Height
            int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

            string Filters = gc.Req("filters");
            string BIID = gc.Req("BIID");

            if (BIID != "")
            {
                Session["BIID"] = BIID;
            }

            if (Session["BIID"] == null)
            {
                Session["BIID"] = "1";
            }

            dtc = gc.GetTables("exec dcr_sp_rs_form_fields " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString() + "," + Session["BIID"].ToString() + ",'" + Filters + "'");

            litLeftMenu.Text = "";
            litLeftMenu.Text += gc.FilterDate("Start_Date", "Start Date", Filters) + "<br />";
            litLeftMenu.Text += gc.FilterDate("End_Date", "End Date", Filters) + "<br />";

            // iterate through field filters
            foreach (DataRow dr in dtc[1].Rows)
            {
                string FilterField = dr["Field"].ToString();
                litLeftMenu.Text += gc.MultipleSelectFilter(FilterField, FilterField, true, dtc[2].Select("Field='" + FilterField + "'").CopyToDataTable(), 5, Filters) + "<br />";
            }


            // report viewer

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.AsyncRendering = true;
            ReportViewer1.SizeToReportContent = false;

            ReportViewer1.Height = AvailableClientHeight;

            //Session["ReportHeight"] = ReportViewer1.Height;

            LocalReport rep = ReportViewer1.LocalReport;
            rep.ReportPath = "reports/BI Report.rdl";
            rep.EnableHyperlinks = true;
            rep.DataSources.Clear();

            rep.DataSources.Add(new ReportDataSource("dcr_sp_rs_form_fields", dtc[0]));
            rep.Refresh();
        }
    }
}