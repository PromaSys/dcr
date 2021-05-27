using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;

public partial class Dialog : System.Web.UI.Page
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
            string DialogType = gc.Req("type");

            switch (DialogType)
            {
                case "horizontal":
                    HGrid.Title = DialogTitle;
                    HGrid.Height = Convert.ToInt32(DialogHeight) - 140;
                    HGrid.Width = Convert.ToInt32(DialogWidth);
                    HGrid.EncryptionKey = Application["EncryptionKey"].ToString();
                    HGrid.ID = "Grid" + Context;
                    HGrid.Visible = true;
                    Session["GridID"] = HGrid.ID;
                    break;

                case "vertical":
                    VGrid.Title = DialogTitle;
                    VGrid.Height = Convert.ToInt32(DialogHeight) - 140;
                    VGrid.Width = Convert.ToInt32(DialogWidth);
                    VGrid.EncryptionKey = Application["EncryptionKey"].ToString();
                    VGrid.ID = "Grid" + Context;
                    VGrid.Visible = true;
                    Session["GridID"] = VGrid.ID;
                    break;

                case "spread":
                    SGrid.Title = DialogTitle;
                    SGrid.Height = Convert.ToInt32(DialogHeight) - 140;
                    SGrid.Width = Convert.ToInt32(DialogWidth);
                    SGrid.EncryptionKey = Application["EncryptionKey"].ToString();
                    SGrid.ID = "Grid" + Context;
                    SGrid.Visible = true;
                    Session["GridID"] = SGrid.ID;
                    break;

                
                case "chat":
                    CGrid.Title = DialogTitle;
                    CGrid.Height = Convert.ToInt32(DialogHeight) - 140;
                    CGrid.Width = Convert.ToInt32(DialogWidth);
                    CGrid.EncryptionKey = Application["EncryptionKey"].ToString();
                    CGrid.ID = "Grid" + Context;
                    CGrid.Visible = true;
                    Session["GridID"] = SGrid.ID;

                    divChat.Visible = true;
                    Context = "chat";

                    break;
                
            }           

            switch (Context)
            {

                case "HTest":

                    string HTestID = gc.Req("Test_ID");

                    dtc = gc.GetTables("exec fnd_sp_test " + Session["UserID"].ToString() + ", " + Session["UserGroupID"].ToString());

                    HGrid.GridTable = dtc[0];
                    HGrid.Table = "fnd_test";
                    HGrid.KeyField = "Test_ID";
                    HGrid.Hide = "Test_ID";
                    HGrid.Edit = true;
                    HGrid.DoNotEdit = "Test_ID|Docs|Chat";
                    HGrid.Widths = "Quantity|15%|Note|35%|Active|10%|Modified|30%|Modified_By|30%";
                    HGrid.ColumnStyle = "Drop_Down_ID|text-align: left;";
                    HGrid.Labels = "";
                    HGrid.Formats = "Date|MM/dd/yyyy";
                    HGrid.Templates = "Drop_Down_ID|" + gc.GetSelect("selChoices", true, dtc[1]) + "|Multiple_Choice_IDs|" + gc.GetMultipleSelect("selMultipleChoices", dtc[1], 1);
                    HGrid.AddSQL = gc.TraceSQL() + "|Test_ID|" + HTestID ;
                    HGrid.Title = "";
                    HGrid.Calendars = "Date";
                    HGrid.TextAreas = "Long_Text|3";
                    HGrid.Required = "";
                    HGrid.NewRecords = 20;
                    HGrid.Blocked = "";
                    HGrid.Counter = "";
                    HGrid.Widths = "default";
                    HGrid.NumberFormat = "Money|$#,###.00";
                    HGrid.DeleteColumn = true;
                    HGrid.Chat = "Chat|Test|Test_ID|800,600";
                    HGrid.Docs = "Docs|Test|Test_ID|1000,400";
                    HGrid.NewRecords = 5;
                    break;

                case "VTest":

                    string VTestID = gc.Req("Test_ID");

                    dtc = dtc = gc.GetTables("exec fnd_sp_test " + Session["UserID"].ToString() + ", " + Session["UserGroupID"].ToString());
                    VGrid.GridTable = dtc[0];
                    VGrid.Table = "fnd_Test";
                    VGrid.KeyField = "Test_ID";
                    VGrid.Edit = true;
                    VGrid.Labels = "";
                    VGrid.Formats = "";
                    VGrid.Templates = "Drop_Down_ID|" + gc.GetSelect("selChoices", true, dtc[1]) + "|Multiple_Choice_IDs|" + gc.GetMultipleSelect("selMultipleChoices", dtc[1], 1);
                    VGrid.Title = "";
                    VGrid.Calendars = "Date";
                    VGrid.TextAreas = "";
                    VGrid.Signatures = "Signature";
                    //VGrid.AddSQL = gc.TraceSQL() + "|Test_ID|" + VTestID;
                    break;

                case "STest":

                    string STestID = gc.Req("Test_ID");

                    dtc = gc.GetTables("exec fnd_sp_test " + Session["UserID"].ToString() + ", " + Session["UserGroupID"].ToString());
                    SGrid.GridTable = dtc[0];
                    SGrid.Table = "fnd_Test";
                    SGrid.KeyField = "Test_ID";
                    SGrid.Edit = true;
                    SGrid.Labels = "";
                    SGrid.Formats = "Date|MM/dd/yyyy";
                    SGrid.Templates = "Drop_Down_ID|spreaddropdown" + gc.GetSpreadDropDown("selChoices", true, dtc[1]) + "|Multiple_Choice_IDs|multiplespreaddropdown" + gc.GetSpreadDropDown("selMultipleChoices", true, dtc[1]);
                    SGrid.Calendars = "Date";
                    //SGrid.TextAreas = "Long_Text|3";
                    SGrid.NewRecords = 20;
                    SGrid.Widths = "default";
                    SGrid.NumberFormat = "Money|$#,###.00";
                    SGrid.DeleteColumn = true;
                    SGrid.Hide = "Docs|Chat";
                    //SGrid.DoNotEdit = "Test_ID|Docs|Chat";
                    //SGrid.Chat = "Chat|Test|Test_ID|800,600";
                    //SGrid.Docs = "Docs|Test|Test_ID|1000,400";
                    //SGrid.Links = "Money|gridPop({context: 'HTest', type: 'horizontal', title: 'Test Horizontal', w: 1200, h: 600, data: {Test_ID: 1}})|Integer|gridPop({context: 'STest', type: 'spread', title: 'Test Spread', w: 1200, h: 600, data: {Test_ID: 1}});";
                    SGrid.NewRecords = 5;
                    break;

                case "chat":
                    
                    string ChatContext = gc.Req("c");
                    string ChatContextID = gc.Req("cid");

                    dtc = gc.GetTables("exec app_sp_chat " + ChatContextID + ",'" + ChatContext + "'");

                    CGrid.ID = "ChatComment";
                    CGrid.Hide = "Chat_ID";
                    CGrid.GridTable = dtc[0];
                    CGrid.AddSQL = "Context|'" + ChatContext + "'|Context_ID|" + ChatContextID + "|Created_By |'" + Session["UserName"].ToString() + "'";
                    CGrid.Height = 150;
                    CGrid.Title = "";

                    if (dtc[1].Rows.Count > 0)
                    {
                        litChat.Text = dtc[1].Rows[0]["Chat"].ToString();
                    }

                    divChat.Height = Convert.ToInt32(DialogHeight) - 315;
                    break;

                case "TemplateFields":

                    string Template_ID = gc.Req("Template_ID");

                    dtc = gc.GetTables("exec dcr_sp_get_fields " + Template_ID);

                    HGrid.GridTable = dtc[0];
                    HGrid.Table = "dcr_Form_Template_Fields";
                    HGrid.KeyField = "Template_Field_ID";
                    HGrid.Edit = true;
                    HGrid.ColumnStyle = "Required|text-align: center;|Type|text-align: left;";
                    HGrid.Labels = "";
                    HGrid.Formats = "";
                    HGrid.DoNotEdit = "Choices";
                    HGrid.Templates = "Type|" + gc.GetSelect("selFieldType", true, dtc[1]);
                    HGrid.Title = "";
                    HGrid.Hide = "Form_Template_ID";
                    //HGrid.Calendars = "Date";
                    HGrid.TextAreas = "";
                    break;

                case "FieldChoices":

                    string Field_ID = gc.Req("Template_Field_ID");

                    dtc = gc.GetTables("exec dcr_sp_get_choices " + Field_ID);

                    HGrid.GridTable = dtc[0];
                    HGrid.Table = "dcr_Form_Template_Field_Choices";
                    HGrid.KeyField = "Choice_ID";
                    HGrid.Edit = true;
                    HGrid.ColumnStyle = "Choice|text-align: left;|Active|text-align: center;";
                    HGrid.Labels = "Field_Type_ID|Type";
                    HGrid.Formats = "";
                    HGrid.DoNotEdit = "Choices";
                    HGrid.NewRecords = 10;
                    //HGrid.Templates = "Field_Type_ID|" + gc.GetSelect("selFieldType", true, dtc[1]);
                    HGrid.Title = "";
                    //HGrid.Hide = "Form_Template_ID";
                    HGrid.AddSQL = gc.TraceSQL() + "|Template_Field_ID|" + Field_ID;
                    HGrid.TextAreas = "";
                    break;

                case "SelectCategory":

                    dtc = gc.GetTables("exec dcr_sp_select_template_category ");
                    VGrid.GridTable = dtc[0];
                    VGrid.Table = "dcr_Subject_Categories";
                    VGrid.KeyField = "Category_ID";
                    VGrid.Edit = true;
                    VGrid.Labels = "Cat_ID|Category";
                    VGrid.Formats = "";
                    VGrid.Templates = "Cat_ID|" + gc.GetSelect("selChoices", true, dtc[1]);
                    VGrid.Title = "";
                    VGrid.Calendars = "Date";
                    VGrid.TextAreas = "";
                    VGrid.Signatures = "Signature";
                    //VGrid.AddSQL = gc.TraceSQL() + "|Test_ID|" + VTestID;
                    break;

                case "NewTemplate":

                    string Category_ID = gc.Req("categoryId");

                    dtc = gc.GetTables("exec dcr_sp_create_template " + Session["UserID"].ToString() + ", " + Session["UserGroupID"].ToString() + ", '', ''");
                    VGrid.GridTable = dtc[0];
                    VGrid.Table = "dcr_Form_Templates";
                    VGrid.KeyField = "Form_Template_ID";
                    VGrid.DoNotEdit = "Category_ID";
                    VGrid.Edit = true;
                    VGrid.Labels = "Cat_ID|Category";
                    VGrid.Hide = "Category|Field_Count";
                    VGrid.Formats = "";
                    VGrid.Templates = "Category_ID|" + gc.GetSelect("selChoices", true, dtc[1]);
                    VGrid.Title = "";
                    VGrid.Calendars = "Date";
                    VGrid.TextAreas = "";
                    VGrid.Signatures = "Signature";
                    VGrid.AddSQL = gc.TraceSQL() + "|Category_ID|" + Category_ID;
                    break;
            }

            gc = null;
        }
    }
}