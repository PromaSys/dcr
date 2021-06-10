using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;

public partial class Form : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTable dt = new DataTable();
        DataTableCollection dtc = null;
        StringBuilder sb = new StringBuilder();

        string Context = gc.Req("c");
        string FormTemplateID = gc.Req("ftid");
        string FormID = gc.Req("fid");


        dtc = gc.GetTables($"exec dcr_sp_form {Session["UserID"].ToString()},{Session["UserGroupID"].ToString()},{FormID},{FormTemplateID}");
        dt = dtc[0];

        string FormSQL = dt.Rows[0]["Form_SQL"].ToString();
        string FormLabels = dt.Rows[0]["Form_Labels"].ToString();
        string FormRequired = dt.Rows[0]["Form_Required"].ToString();
        string FormTemplates = dt.Rows[0]["Form_Templates"].ToString();
        string FormNumberFormat = dt.Rows[0]["Form_Number_Format"].ToString();
        string FormTextAreas = dt.Rows[0]["Form_Text_Areas"].ToString();
        string FormCalendars = dt.Rows[0]["Form_Calendars"].ToString();
        string FormDocs = dt.Rows[0]["Form_Docs"].ToString();
        string FormDoNotEdit = dt.Rows[0]["Form_DoNotEdit"].ToString();
        string FormLinks = dt.Rows[0]["Form_Links"].ToString();

        vgForm.Hide = "Template_Form_ID|Form_ID";
        vgForm.ID = Context;

        vgForm.GridTable = gc.GetTable(FormSQL);
        vgForm.Labels = FormLabels;
        vgForm.Required = FormRequired;
        vgForm.Templates = FormTemplates;

        vgForm.NumberFormat = FormNumberFormat;
        vgForm.TextAreas = FormTextAreas;
        vgForm.Calendars = FormCalendars;
        //vgForm.Docs = FormDocs;
        vgForm.DoNotEdit = FormDoNotEdit;
        vgForm.Links = FormLinks;
    }
}