using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data.SqlClient;

public partial class Display_Doc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();

        if (Session["UserID"] == null)
        {
            Response.Write("<script type='text/javascript' >window.open('logout.aspx?e','_top');</script >"); Response.End();
        }

        #region DefaultForm

        if (Request.Form["did"] != null && Request.Form["did"] != "")
        {
            Session["DocumentID"] = gc.TryDecrypt(Request.Form["did"].ToString());
        }

        if (Request.Form["txtDocumentID"] != null && Request.Form["txtDocumentID"] != "")
        {
            Session["DocumentID"] = gc.TryDecrypt(Request.Form["txtDocumentID"].ToString());
        }

        if (Request.QueryString.ToString() != "")
        {
            Session["DocumentID"] = gc.TryDecrypt(Request.QueryString["document_id"].ToString());
        }

        #endregion DefaultForm

        string DocumentType;

        Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));
        Response.Buffer = true;
        Response.Clear();

        SqlConnection conn = new SqlConnection(GlobalClass.ConnectionString);
        SqlCommand cmd = new SqlCommand("", conn);

        cmd.CommandText = "select Contents, Mime_Type, Name from app_Documents where Document_ID = " + Session["DocumentID"].ToString();

        cmd.Connection.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        dr.Read();

        DocumentType = dr["Mime_Type"].ToString();

        if (DocumentType != null)
        {
            Response.ContentType = DocumentType;
        }

        Response.AddHeader("Content-Disposition", "attachment; filename=" + dr["Name"].ToString());

        Response.BinaryWrite((byte[])dr["Contents"]);
        cmd.Connection.Close();
        Response.End();
    }
}