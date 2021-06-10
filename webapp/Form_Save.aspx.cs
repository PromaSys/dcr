using System;
using System.IO;
using System.Xml;
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
        string TemplateFieldID = "";
        string FormFieldID = "";

        XmlNode Data = XMLChanges.DocumentElement;

        // iterate through nodes
        XmlNodeList Nodes = XMLChanges.SelectNodes("data/fv");

        foreach (XmlNode Node in Nodes)
        {
            Field = gc.Decrypt(Node.Attributes["f"].Value);
            Value = Node.Attributes["v"].Value;

            TemplateFieldID = Field.Split('_')[2];
            FormFieldID = Field.Split('_')[4];

            SQL += "exec dcr_sp_uff " + TemplateFieldID + "," + FormFieldID + "','" + Value + "','" + Session["UserName"].ToString() + "'; ";

        }

        return SQL;
    }
}