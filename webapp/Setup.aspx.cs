using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Setup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));

        GlobalClass gc = new GlobalClass();
        DataTableCollection dtc = null;

        //Available Client Height
        int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 1400);

        // get Setup left menu selects

        Session["SetupTopic"] = "Users";

        if (txtTopic.Value != "")
        {
            Session["SetupTopic"] = txtTopic.Value;
        }

        if (Session["SetupTopic"].ToString() != "")
        {
            Grid.Height = AvailableClientHeight;
            Grid.Title = "Setup - " + Session["SetupTopic"].ToString();
            Grid.EncryptionKey = Application["EncryptionKey"].ToString();
        }

        if (Session["SetupTopic"].ToString() == "Users")
        {

            string SearchUsers = "";
            
            if(Request.Cookies["SearchUsers"] != null)
            {
                SearchUsers = Request.Cookies["SearchUsers"].Value;
            }

            if (Application["AuthenticationMode"].ToString() != "Windows")
            {

                dtc = gc.GetTables("exec app_sp_setup_users '" + SearchUsers + "'");

                Grid.GridTable = dtc[0];
                Grid.Templates = "User_Group_ID|" + gc.GetSelect("selUserGroup", true, dtc[1]);
                Grid.Title = "<span>Users</span>|<a type = 'button' class = 'btn btn-primary btn-sm' onclick='gridEditorFormNew({element: this, w: 500, h: 410});' >Add New User</a>";
                Grid.Labels = "UserName|Login|PhoneNumber|Phone|User_Group_ID|Group|Company_CUSTNMBR|Customer|Language_ID|Language|Login_As|As|Welcome|Hello";
                Grid.ColumnStyle = "Active|text-align: center;|Reset|text-align: center;|Unlock|text-align: center;|Link|text-align: center;|User_Group_ID|text-align: left;|Del|text-align: center;|Login_As|text-align: center;|Welcome|text-align: center;";
                Grid.Required = "Name|UserName|Email|User_Group_ID";
                Grid.Widths = "Name|15%|UserName|10%|Email|20%|PhoneNumber|10%|User_Group_ID|10%|Active|5%|Reset|5%|Unlock|5%|Link|5%|Welcome|5%|Login_As|5%|Del|5%";
                Grid.Hide = "User_ID";
                Grid.Edit = false;
                Grid.Links = "Name|Editor,500,RelativePixels('h',.7, 410)|Reset|ResetPassword(this);|Unlock|UnlockAccount(this);|Link|EmailResetLink(this);|Login_As|LoginAs(this);|Welcome|EmailWelcomeLink(this);";
                Grid.DoNotEdit = "Reset|Link|Unlock|Welcome|Login_As|Name_Index";
                Grid.KeyField = "User_ID";
                Grid.Table = "AspNetUsers";
                Grid.DeleteColumn = true;
                Grid.Calendars = "";
                Grid.Formats = "";
                Grid.Blocked = "";
                Grid.GroupBy = "Name_Index,e,1,1";
                Grid.CssClass = "table table-hover scrollable js-dynamitable hg setup";

            }
            else
            {
                dtc = gc.GetTables("exec app_sp_setup_AD_users '" + SearchUsers + "'");
                Grid.GridTable = dtc[0];
                Grid.Templates = "User_Group_ID|" + gc.GetSelect("selGroups", true, dtc[1]);
                Grid.Title = "Users";
                Grid.Labels = "User_Group_ID|User Group|Login_As|As";
                Grid.ColumnStyle = "User_Group_ID|text-align: left;";
                Grid.Required = "SamAccountName";
                Grid.Widths = "SamAccountName|20%|Name|25%|Email|25%|User_Group_ID|20%|Login_As|5%|Del|5%";
                Grid.Edit = true;
                Grid.DoNotEdit = "Email|Name";
                Grid.NewRecords = 25;
                Grid.KeyField = "AD_User_ID";
                Grid.Table = "app_AD_Users";
                Grid.DeleteColumn = true;
                Grid.GroupBy = "Name_Index,e,1,1";
            }
        }
        else if (Session["SetupTopic"].ToString() == "Groups")
        {

            dtc = gc.GetTables("exec app_sp_setup_user_groups");
            Grid.GridTable = dtc[0];
            Grid.Templates = "Pages|" + gc.GetMultipleSelect("selPages", dtc[1], 3) + "|Start_Page_ID|" + gc.GetSelect("selStartPage", true, dtc[2]);
            Grid.Title = "Groups";
            Grid.Labels = "Group_Name|Group|Start_Page_ID|Start Page";
            Grid.ColumnStyle = "Active|text-align: center;|Start_Page_ID|text-align: left;";
            Grid.Required = "Group_Name|Pages|Start_Page_ID";
            Grid.Widths = "Group_Name|20%|Pages|40%|Start_Page_ID|20%|Active|10%";
            Grid.Edit = true;
            Grid.NewRecords = 25;
            Grid.KeyField = "User_Group_ID";
            Grid.Table = "app_User_Groups";

        }
        else if (Session["SetupTopic"].ToString() == "Templates")
        {

            dtc = gc.GetTables("select Template_ID, Name, Template, Active from app_Templates order by Template_ID");
            Grid.GridTable = dtc[0];
            Grid.Title = "Templates";
            Grid.ColumnStyle = "";
            Grid.Required = "Name|Template";
            Grid.Widths = "Name|10%|Template|80%|Active|10%";
            Grid.Edit = true;
            //Grid.NewRecords = 5;
            Grid.KeyField = "Template_ID";
            Grid.Table = "app_Templates";
            Grid.TextAreas = "Template|30";
            Grid.AddSQL = gc.TraceSQL();
            Grid.Labels = "";
            Grid.DoNotEdit = "Name";

        }
        else if (Session["SetupTopic"].ToString() == "Theme")
        {

            dtc = gc.GetTables("exec app_sp_setup_theme");
            Grid.GridTable = dtc[0];
            Grid.Title = "Themes|<a href='#' onclick='gridEditorFormNew({element: this, w: 500, h: 410});' class='btn btn-info'>New Theme</a>&nbsp;&nbsp;";
            Grid.Hide = "Stylesheet_ID";
            Grid.ColumnStyle = "Preview_theme|text-align:center;|Set_Theme|text-align: center;";

            //Grid.Required = "Name|Template";
            Grid.Widths = "default";
            Grid.DoNotEdit = "Set_Theme|Preview_Theme";
            Grid.Edit = true;
            //Grid.Blocked = "Set_Theme";
            Grid.Labels = "Stylesheet_Name|Theme";
            Grid.KeyField = "Stylesheet_ID";
            Grid.Table = "app_Css_Stylesheets";
            Grid.TextAreas = "Template|30";
            Grid.AddSQL = gc.TraceSQL();
            //Grid.AsIs = "Set_Theme|Preview_Theme";
            //Grid.NewRecords = 25;
            //Grid.DeleteColumn = true;

        }

        dtc = null;
        //dt.Dispose();
        gc = null;

    }
}