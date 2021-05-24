using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Grid_Dialog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Session["UserID"] == null)
            {
                Response.Write("<script type='text/javascript' >window.open('logout.aspx','_top');</script >"); Response.End();
            }

            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            GlobalClass gc = new GlobalClass();
            DataTable dt = new DataTable();
            DataTableCollection dtc = null;

            //Available Client Height
            int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 400);

            string Context = gc.Req("c");
            string DialogWidth = gc.Req("w");
            string DialogHeight = gc.Req("h");
            string DialogTitle = gc.Req("t");

            Grid.Title = DialogTitle;
            Grid.Height = Convert.ToInt32(DialogHeight) - 140;
            Grid.Width = Convert.ToInt32(DialogWidth);
            Grid.EncryptionKey = Application["EncryptionKey"].ToString();

            Grid.ID = "Grid" + Context;
            Session["GridID"] = Grid.ID;

            switch (Context)
            {

                case "Test":

                    string TestID = gc.Req("tid");

                    dtc = gc.GetTables("exec fnd_sp_test " + TestID);

                    Grid.GridTable = dtc[0];
                    Grid.Table = "fnd_test";
                    Grid.KeyField = "Test_ID";
                    Grid.Hide = "Test_ID";
                    Grid.Edit = true;
                    Grid.DoNotEdit = "Modified|Modified_By";
                    Grid.Widths = "Quantity|15%|Note|35%|Active|10%|Modified|30%|Modified_By|30%";
                    Grid.ColumnStyle = "Quantity|text-align: left;|Del|text-align: center;|Active|text-align: center;|Modified|text-align: center;";
                    Grid.Labels = "";
                    Grid.Formats = "Modified|MM/dd/yyy";
                    Grid.Templates = "";
                    Grid.AddSQL = gc.TraceSQL() + "|Test_ID|" + TestID ;
                    Grid.Title = "";
                    Grid.Calendars = "";
                    Grid.TextAreas = "";
                    Grid.Required = "";
                    Grid.NewRecords = 20;
                    Grid.Blocked = "";
                    Grid.Counter = "";
                    //Grid.AutoSave = false;
                    //Grid.DeleteColumn = true;
                    break;
            }

            gc = null;
        }
    }
}