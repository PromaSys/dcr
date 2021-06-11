using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;

public partial class Form_Save : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null)
        {
            Response.Write("<script type='text/javascript' >window.open('logout.aspx','_top');</script >"); Response.End();
        }

        Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));

        string xmlData = "";

        if (Request.RequestType == "GET")
        {
            xmlData = GetChange(Request.QueryString.ToString());
        }
        else if (Request.RequestType == "POST")
        {
            Request.InputStream.Position = 0;
            StreamReader reader = new StreamReader(Request.InputStream);
            xmlData = reader.ReadToEnd();
            xmlData = GetChange(xmlData);
        }

        // convert to SQL
        string SQL = ConvertToSQL(xmlData);

        if (SQL.StartsWith("Error"))
        {
            Response.Write(SQL);
            SQL = "";
        }

        if (SQL != "")
        {
            SQL = "BEGIN TRAN T100 " + SQL + " COMMIT TRAN T100";

            GlobalClass gc = new GlobalClass();
            DataTable dt = new DataTable();

            try
            {
                dt = gc.GetTable(SQL);
                Response.Write("Status|Success");

            }
            catch (Exception SQLEx)
            {
                if (SQLEx.Message.StartsWith("_"))
                {
                    Response.Write("Error|" + SQLEx.Message.Substring(1));
                }
                else
                {
                    Response.Write("Error|" + SQLEx.Message);
                }
            }

        }
    }

    private string GetChange(string Text)
    {
        Text = Server.UrlDecode(Text);
        Text = Text.Replace("-[_", "<").Replace("_]-", ">");
        return Text;
    }

    private string ConvertToSQL(string Changes)
    {

        GlobalClass gc = new GlobalClass();

        string SQL = "";

        XmlDocument XMLChanges = new XmlDocument();
        try
        {
            XMLChanges.LoadXml(Changes);
        }
        catch (XmlException Xmle)
        {
            /*
            SQL = "Insert into app_data_access_attempts(User_ID, Changes) values(" + Session["UserID"].ToString() + ",'" + Changes.Replace("'", "''") + "') ";
            SQL += "RAISERROR ('Invalid statement of changes: " + Xmle.Message + "' + ,16,1)";
            return SQL;
            */
            Response.Write("Error|" + Xmle.Message);
            return "";
        }

        string Field = "";
        string Value = "";
        string SubjectID = "";
        string FormID = "";
        string FormTemplateID = "";
        string TemplateFieldID = "";
        string FormFieldID = "";

        string FinalFormID = "";

        XmlNode Data = XMLChanges.DocumentElement;

        // iterate through nodes
        XmlNodeList Nodes = XMLChanges.SelectNodes("data/fv");

        foreach (XmlNode Node in Nodes)
        {
            Field = gc.Decrypt(Node.Attributes["f"].Value);
            Value = Node.Attributes["v"].Value;

            SubjectID = Field.Split('_')[2];
            FormID = Field.Split('_')[4];
            FormTemplateID = Field.Split('_')[6];


            if(FinalFormID == "")
            {
                if(FormID != "0")
                {
                    FinalFormID = FormID;
                }
                else
                {
                    FinalFormID = gc.GetTable("exec dcr_sp_form_create " + FormTemplateID + "," + SubjectID + ",'" + Session["UserName"].ToString() + "'").Rows[0]["Form_ID"].ToString();
                }
            }


            TemplateFieldID = Field.Split('_')[8];
            FormFieldID = Field.Split('_')[10];

            SQL += "\r\n exec dcr_sp_form_field_save " + FinalFormID + "," + TemplateFieldID + "," + FormFieldID + ",'" + Value + "','" + Session["UserName"].ToString() + "'; \r\n";

        }

        return SQL;
    }
}