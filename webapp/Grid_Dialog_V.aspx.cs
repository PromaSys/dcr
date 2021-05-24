using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Grid_Dialog_V : System.Web.UI.Page
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
            Grid.Height = Convert.ToInt32(DialogHeight) - 130;
            Grid.Width = Convert.ToInt32(DialogWidth) - 60;
            Grid.EncryptionKey = Application["EncryptionKey"].ToString();
            Grid.ID = "VGrid" + Context;

            switch (Context)
            {
                case "Test":

                    string TestID = gc.Req("Test_ID");

                    dtc = gc.GetTables("select * from fnd_Test where Test_ID = " + TestID);
                    Grid.GridTable = dtc[0];
                    Grid.Table = "fnd_Tests";
                    Grid.KeyField = "Test_ID";
                    Grid.Edit = true;
                    Grid.Labels = "";
                    Grid.Formats = "";
                    Grid.Templates = "";
                    Grid.Title = "";
                    Grid.Calendars = "";
                    Grid.TextAreas = "";
                    break;

                case "ManageAccount":

                    dtc = gc.GetTables("exec app_sp_manage_account " + Session["UserID"].ToString() + "," + Session["UserGroupID"].ToString());

                    Grid.GridTable = dtc[0];
                    Grid.Table = "AspNetUsers";
                    Grid.KeyField = "User_ID";
                    Grid.Hide = "User_ID";
                    Grid.Edit = true;
                    Grid.DoNotEdit = "User_ID";
                    Grid.Widths = "";
                    Grid.Labels = "Language_ID|Language";
                    Grid.Formats = "";
                    Grid.Templates = "";
                    Grid.AddSQL = gc.TraceSQL();
                    Grid.Title = "";
                    Grid.Calendars = "";
                    Grid.TextAreas = "";
                    Grid.Required = "Name|UserName|Email";
                    break;
            }



            gc = null;
        }
    }
}