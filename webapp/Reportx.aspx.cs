using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using WebApp.App_Code;
using System.Data;
using System.Xml;
using System.IO;

public partial class Reportx : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.AppendHeader("X-UA-Compatible", "IE=8");

        GlobalClass gc = new GlobalClass();

        //if (IsPostBack) return;

        if (!IsPostBack || Request.Form["txtReportParameters"] != "")
        {
            DataSet dsr = new DataSet();
            DataTable dt = new DataTable();

            Response.Cache.SetExpires(DateTime.Now.AddSeconds(0));

            Session["ShowParameters"] = "No";
            Session["rqp_HideParameters"] = "";
            Session["rqp_ReportFormat"] = "";
            Session["rqp_ReportFileName"] = "";


            if (Request.QueryString.ToString() != "")
            {
                Session["rqp_RDL"] = Request.QueryString["rdl"].ToString().Replace("+", " ");

                dt = gc.GetTable("exec app_sp_get_report '" + Session["rqp_RDL"].ToString() + "'");

                Session["rqp_ForceParameters"] = dt.Rows[0]["Force_Parameters"].ToString();
                Session["rqp_HideParameters"] = dt.Rows[0]["Hide_Parameters"].ToString();

                if (Request.QueryString["par"] != null)
                {
                    Session["rqp_ForceParameters"] = Request.QueryString["par"].ToString();
                }

                if (Request.QueryString["hide"] != null)
                {
                    Session["rqp_HideParameters"] = Request.QueryString["hide"].ToString();
                }

                if (Request.QueryString["format"] != null)
                {
                    Session["rqp_ReportFormat"] = Request.QueryString["format"].ToString();
                }

                if (Request.QueryString["file"] != null)
                {
                    Session["rqp_ReportFileName"] = Request.QueryString["file"].ToString();
                }
            }

            if (Session["rqp_ForceParameters"] != null)
            {
                Session["PostedParameters"] = Session["rqp_ForceParameters"].ToString();
                Session["PostedParameters"] = Session["PostedParameters"].ToString().Replace("UserID|@UserID", "UserID|" + gc.Encrypt(Session["UserID"].ToString()));
                Session["PostedParameters"] = Session["PostedParameters"].ToString().Replace("OrganizationID|@OrganizationID", "OrganizationID|" + gc.Encrypt("1"));
                Session["PostedParameters"] = Session["PostedParameters"].ToString().Replace("UserGroupID|@UserGroupID", "UserGroupID|" + gc.Encrypt(Session["UserGroupID"].ToString()));
                Session["PostedParameters"] = Session["PostedParameters"].ToString().Replace("Date|Today", "Date|" + DateTime.Today.ToString("MM/dd/yyyy"));
            }

            string PostedParameters = "";
            if (Request.Form["txtReportParameters"] != null && Request.Form["txtReportParameters"] != "")
            {
                PostedParameters = Request.Form["txtReportParameters"].ToString();

                // decrypt
                string[] Params = PostedParameters.Split(new char[] { '|' });
                PostedParameters = "";
                for (int i = 0; i < Params.Length; i = i + 2)
                {
                    PostedParameters += gc.TryDecrypt(Params[i]) + "|" + Params[i + 1] + "|";
                }

                if (PostedParameters != "")
                {
                    PostedParameters = PostedParameters.Substring(0, PostedParameters.Length - 1);
                }
            }

            if (PostedParameters == "")
            {
                if (Request.Cookies["ParameterString"] != null)
                {
                    // read cookie
                    PostedParameters = Server.UrlDecode(Request.Cookies["ParameterString"].Value);
                }
            }


            ReportViewer1.Visible = false;

            if (Request.Form["txtReportHandle"] != null && Request.Form["txtReportHandle"] != "")
            {
                if (Request.Form["txtReportHandle"].ToString() == "1")
                {
                    ReportViewer1.Visible = true;
                }
            }


            if (ReportViewer1.Visible == false)
            {
                if (Request.Cookies["ReportHandle"] != null)
                {
                    if (Request.Cookies["ReportHandle"].Value.ToString() == "1")
                    {
                        ReportViewer1.Visible = true;
                    }
                }
            }


            if (PostedParameters != "")
            {
                Session["PostedParameters"] = PostedParameters;
            }

            if (Session["PostedParameters"].ToString() != "")
            {
                char[] delimiter = { '|' };
                string[] PP = Session["PostedParameters"].ToString().Split(delimiter);

                for (int i = 0; i < PP.Length - 1; i = i + 2)
                {
                    if (PP[i + 1] == "")
                    {
                        Session["rqp_" + PP[i]] = "null";
                    }
                    else
                    {
                        Session["rqp_" + PP[i]] = gc.TryDecrypt(PP[i + 1]);
                    }
                }
            }

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.AsyncRendering = true;
            ReportViewer1.SizeToReportContent = false;

            if (Request.Cookies["AvailableClientHeight"] != null)
            {
                ReportViewer1.Height = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 400) + 100;
            }

            LocalReport rep = ReportViewer1.LocalReport;
            rep.ReportPath = "reports/" + Session["rqp_RDL"].ToString();
            rep.EnableHyperlinks = true;
            rep.DataSources.Clear();

            XmlDocument xmlReport = new XmlDocument();
            xmlReport.Load(Request.ServerVariables["APPL_PHYSICAL_PATH"] + "reports\\" + Session["rqp_RDL"].ToString());

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlReport.NameTable);
            //nsmgr.AddNamespace("a", "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition");
            //nsmgr.AddNamespace("a", "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            nsmgr.AddNamespace("a", xmlReport.ChildNodes[1].NamespaceURI);

            // get parameters
            //XmlElement root = xmlReport.DocumentElement;

            XmlNodeList parameters = xmlReport.SelectNodes("a:Report/a:ReportParameters/a:ReportParameter", nsmgr);

            string ParameterName = "";
            string ParameterType = "";
            string ParameterSQL = "";
            string ParameterLabels = "";
            string ParameterCalendars = "";
            string ParameterDropDowns = "";
            string ParameterDropDown = "";
            string StringParameters = "|";


            foreach (XmlNode parameter in parameters)
            {
                string DefaultValue = "";
                ParameterName = parameter.Attributes["Name"].Value;
                ParameterType = parameter.SelectSingleNode("a:DataType", nsmgr).InnerText;
                ParameterDropDown = "";

                if (ParameterType == "DateTime" || ParameterType == "String")
                {
                    StringParameters += ParameterName + "|";
                }

                if (ParameterType == "DateTime")
                {
                    ParameterCalendars += ParameterName + "|";
                }

                // parameter values: data set 
                if (parameter.SelectNodes("a:ValidValues/a:DataSetReference/a:DataSetName", nsmgr).Count == 1)
                {

                    string DataSetName = parameter.SelectSingleNode("a:ValidValues/a:DataSetReference/a:DataSetName", nsmgr).InnerText;
                    string ValueField = parameter.SelectSingleNode("a:ValidValues/a:DataSetReference/a:ValueField", nsmgr).InnerText;
                    string LabelField = parameter.SelectSingleNode("a:ValidValues/a:DataSetReference/a:LabelField", nsmgr).InnerText;

                    string CommandText = xmlReport.SelectSingleNode("a:Report/a:DataSets/a:DataSet[@Name='" + DataSetName + "']/a:Query/a:CommandText", nsmgr).InnerText;
                    string CommandType = xmlReport.SelectSingleNode("a:Report/a:DataSets/a:DataSet[@Name='" + DataSetName + "']/a:Query/a:CommandType", nsmgr).InnerText;

                    string QueryParameters = "";
                    foreach (XmlNode qp in xmlReport.SelectNodes("a:Report/a:DataSets/a:DataSet[@Name='" + DataSetName + "']/a:Query/a:QueryParameters/a:QueryParameter", nsmgr))
                    {
                        foreach (ReportParameterInfo rpi in this.ReportViewer1.LocalReport.GetParameters())
                        {
                            if (rpi.Values.Count > 0)
                            {
                                if (rpi.Name != ParameterName)
                                {
                                    string ipv = "null";
                                    if (rpi.Values[0] != null)
                                    {
                                        ipv = rpi.Values[0].ToString();
                                    }

                                    string qpName = qp.Attributes["Name"].Value.Replace("@", "");

                                    if (rpi.Name == qpName)
                                    {
                                        QueryParameters += qpName.Replace(rpi.Name, ipv) + ",";
                                    }
                                }
                            }
                        }
                    }

                    if (QueryParameters != "")
                    {
                        QueryParameters = QueryParameters.Substring(0, QueryParameters.Length - 1);
                        if (CommandType == "StoredProcedure")
                        {
                            CommandText = CommandText + " " + QueryParameters;
                        }
                        else
                        {
                            CommandText = CommandText.Replace("UserID", Session["UserID"].ToString());
                        }
                    }


                    if (CommandType == "StoredProcedure")
                    {
                        ParameterDropDown = gc.GetSelect("sel" + ParameterName, false, gc.GetTable("exec " + CommandText), LabelField, ValueField);
                    }
                    else
                    {
                        ParameterDropDown = gc.GetSelect("sel" + ParameterName, false, gc.GetTable(CommandText), LabelField, ValueField);
                    }
                }

                //ParameterDropDown = "";
                if (parameter.SelectNodes("a:ValidValues/a:ParameterValues/a:ParameterValue", nsmgr).Count > 0)
                {

                    foreach (XmlNode pv in parameter.SelectNodes("a:ValidValues/a:ParameterValues/a:ParameterValue", nsmgr))
                    {
                        string v = pv.SelectSingleNode("a:Value", nsmgr).InnerText;
                        ParameterDropDown += "<option value=\"" + gc.Encrypt(v) + "\" >" + v + "</option>";
                    }

                    ParameterDropDown = "<select template=\"select\" id=\"sel" + ParameterName + "\" >" + ParameterDropDown + "</select>";
                }

                if (parameter.SelectNodes("a:DefaultValue/a:Values/a:Value", nsmgr).Count == 1)
                {
                    DefaultValue = parameter.SelectSingleNode("a:DefaultValue/a:Values/a:Value", nsmgr).InnerText;

                    if (DefaultValue == "=Today")
                    {
                        DefaultValue = DateTime.Today.ToShortDateString();// ("M/d/yyyy");
                    }

                    if (DefaultValue == "")
                    {
                        DefaultValue = "null";
                    }
                }
                else
                {
                    DefaultValue = "null";
                }

                // update default values from posted parameters
                if (Session["PostedParameters"].ToString().IndexOf(ParameterName + "|") > -1)
                {
                    DefaultValue = Session["rqp_" + ParameterName].ToString();
                }
                else
                {
                    Session["rqp_" + ParameterName] = DefaultValue;
                }

                if (StringParameters.IndexOf("|" + ParameterName + "|") > -1 && DefaultValue != "null")
                {
                    if (ParameterType == "DateTime")
                    {
                        ParameterSQL += "cast('" + DefaultValue + "' as datetime) AS " + ParameterName + ",";
                    }
                    else
                    {
                        ParameterSQL += "cast('" + DefaultValue + "' as nvarchar(150)) AS " + ParameterName + ",";
                    }
                }
                else
                {
                    ParameterSQL += DefaultValue + " AS " + ParameterName + ",";
                }

                ParameterLabels += ParameterName + "|" + parameter.SelectSingleNode("a:Prompt", nsmgr).InnerText + "|";

                if (ParameterDropDown != "")
                {
                    if (ParameterDropDown.IndexOf("<select", StringComparison.OrdinalIgnoreCase) == 0 && ParameterDropDown.IndexOf("template", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        ParameterDropDown = ParameterDropDown.Replace("<select", "<select template=\"select\"");
                        ParameterDropDown = ParameterDropDown.Replace("<SELECT", "<select template=\"select\"");
                    }
                    //ParameterDropDowns += parameter.Attributes["Name"].Value + "|sel" + ParameterName + "|" + ParameterDropDown + "|";
                    ParameterDropDowns += parameter.Attributes["Name"].Value + "|" + ParameterDropDown + "|";
                }

                //set report parameters
                if (DefaultValue != "null")
                {
                    ReportParameter param = new ReportParameter(ParameterName, DefaultValue);
                    this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { param });
                }

                //if (Session["rqp_ForceParameters"].ToString().IndexOf(ParameterName) == -1 && Session["rqp_HideParameters"].ToString().IndexOf(ParameterName) != -1)
                if (Session["rqp_HideParameters"].ToString().IndexOf(ParameterName) == -1)
                {
                    Session["ShowParameters"] = "Yes";
                }

            }

            if (Session["ShowParameters"].ToString() == "No")
            {
                ReportViewer1.Visible = true;
            }

            if (ParameterSQL != "")
            {
                ParameterSQL = ParameterSQL.Substring(0, ParameterSQL.Length - 1);
            }

            if (ParameterDropDowns != "")
            {
                ParameterDropDowns = ParameterDropDowns.Substring(0, ParameterDropDowns.Length - 1);
            }

            ParameterSQL = "Select -1 ID, " + ParameterSQL;

            if (Session["ShowParameters"].ToString() == "Yes")
            {
                vgParameters.GridTable = gc.GetTable(ParameterSQL);
                vgParameters.Labels = ParameterLabels + "Run_Report|&nbsp;";
                vgParameters.Calendars = ParameterCalendars;
                vgParameters.Templates = ParameterDropDowns;
                //vgParameters.Readonly = "Run_Report";
                //vgParameters.Block = Session["rqp_HideParameters"].ToString();
                //vgParameters.Title = "";
            }


            // Data Sources
            XmlNodeList datasets = xmlReport.SelectNodes("a:Report/a:DataSets/a:DataSet", nsmgr);

            foreach (XmlNode dataset in datasets)
            {
                DataTable ds = new DataTable();
                string CommandText = dataset.SelectSingleNode("a:Query/a:CommandText", nsmgr).InnerText;

                string CommandType = "";
                if (dataset.SelectSingleNode("a:Query/a:CommandType", nsmgr) != null)
                {
                    CommandType = dataset.SelectSingleNode("a:Query/a:CommandType", nsmgr).InnerText;
                }

                XmlNodeList queryparameters = dataset.SelectNodes("a:Query/a:QueryParameters/a:QueryParameter", nsmgr);

                foreach (XmlNode queryparameter in queryparameters)
                {
                    string queryparametername = queryparameter.Attributes["Name"].Value.Replace("@", "");
                    string queryparametervalue = "null";

                    if (Session["rqp_" + queryparametername] != null)
                    {
                        queryparametervalue = Session["rqp_" + queryparametername].ToString();
                    }

                    if (CommandType == "StoredProcedure")
                    {
                        if (StringParameters.IndexOf("|" + queryparametername + "|") > -1 && queryparametervalue != "null")
                        {
                            CommandText += " '" + queryparametervalue + "',";
                        }
                        else
                        {
                            CommandText += " " + queryparametervalue + ",";
                        }
                    }
                    else
                    {
                        if (StringParameters.IndexOf("|" + queryparametername + "|") > -1 && queryparametervalue != "null")
                        {
                            CommandText = CommandText.Replace("@" + queryparametername, "'" + queryparametervalue + "'");
                        }
                        else
                        {
                            CommandText = CommandText.Replace("@" + queryparametername, queryparametervalue);
                        }

                    }

                }

                // strip \r\n
                CommandText = CommandText.Replace("\r\n", " ");
                CommandText = CommandText.Replace("\r", " ");
                CommandText = CommandText.Replace("\n", " ");


                if (CommandType == "StoredProcedure")
                {
                    if (CommandText != "")
                    {
                        if (CommandText.EndsWith(","))
                        {
                            CommandText = CommandText.Substring(0, CommandText.Length - 1);
                        }
                    }

                    dt = gc.GetTable("exec " + CommandText);
                }
                else
                {
                    dt = gc.GetTable(CommandText);
                }

                ReportDataSource rdsReport = new ReportDataSource();
                rdsReport.Name = dataset.Attributes["Name"].Value;
                rdsReport.Value = dt;
                rep.DataSources.Add(rdsReport);

                rep.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);

                ds.Dispose();
            }

            if (Session["rqp_ReportFormat"].ToString() == "")
            {
                ReportViewer1.Visible = true;
                rep.Refresh();
            }
            else if (Session["rqp_ReportFormat"].ToString() != "")
            {
                CreateReport(rep, Session["rqp_ReportFormat"].ToString());
            }

            dsr.Dispose();
            gc = null;
        }/*
            else
            {

                
                string PostedParameters = "";
                if (Request.Form["txtReportParameters"] != null && Request.Form["txtReportParameters"] != "")
                {
                    PostedParameters = Request.Form["txtReportParameters"].ToString();
                }

                string[] Params = PostedParameters.Split(new char[] { '|'});
                for(int i=0;i<Params.Length; i=i+2)
                {
                    ReportParameter param = new ReportParameter(gc.TryDecrypt(Params[i]), Params[i+1]);
                    ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { param });
                }
                ReportViewer1.Visible = true;
                ReportViewer1.DataBind();
                ReportViewer1.LocalReport.Refresh();
                
            }*/



        /*
        GlobalClass gc = new GlobalClass();
        DataTable dt = new DataTable();

        dt = gc.GetDataSet("exec app_sp_rs_users_contacts 1", "Data Source=p001,4434;Initial Catalog=demo;Persist Security Info=True;User ID=wjraige;Password=Gaby1Sara").Tables[0];

        ReportViewer1.Width = 800;
        ReportViewer1.Height = 600;
        ReportViewer1.ProcessingMode = ProcessingMode.Local;
        ReportViewer1.BackColor = System.Drawing.Color.AliceBlue;
        ReportViewer1.BorderColor = System.Drawing.Color.Black;
        ReportViewer1.BorderStyle = BorderStyle.Solid;
        ReportViewer1.BorderWidth = 4;
        ReportViewer1.AsyncRendering = true;
        ReportViewer1.SizeToReportContent = false;


        LocalReport rep = ReportViewer1.LocalReport;
        rep.ReportPath = "reports/Phone Directory.rdl";
        rep.EnableHyperlinks = true;
        rep.DataSources.Clear();

        ReportParameter param = new ReportParameter("OrganizationID", "1");
        this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { param });

        ReportDataSource rdsReport = new ReportDataSource();
        rdsReport.Name = "app_sp_rs_users_contacts";
        rdsReport.Value = dt;
        rep.DataSources.Add(rdsReport);

        rep.Refresh();

        gc = null;
        */
        /*

        var serverReport = this.ReportViewer1.ServerReport;
        //serverReport.ReportServerCredentials = new ReportServerCredentials(userName, password, "yourDomain.com");
        serverReport.ReportServerUrl = new Uri("http://p001/reportserver");
        serverReport.ReportPath = "/flo_reports/Request Preparation Time.rdl";
        serverReport.Refresh();
        */
    }
    private void CreateReport(LocalReport rep, string ReportFormat)
    {
        try
        {
            Warning[] warn;
            string[] streamids;
            string mimeType = "application/pdf";
            string encoding = string.Empty;
            string extension = string.Empty;
            byte[] byteViewer;
            byteViewer = rep.Render(ReportFormat, null, out mimeType, out encoding, out extension, out streamids, out warn);
            Response.Buffer = true;
            Response.Clear();

            string ReportName = Session["rqp_ReportFileName"].ToString();

            if (ReportName == "")
            {
                ReportName = rep.ReportPath.Replace("reports/", "").Replace(".rdl", "");
            }

            if (ReportFormat == "PDF")
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "Attachment; filename=" + ReportName + ".pdf");
            }
            else if (ReportFormat == "EXCEL")
            {
                Response.ContentType = "application/vnd.ms-excel"; // application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; 
                Response.AddHeader("content-disposition", "Attachment; filename=" + ReportName + ".xls");
            }

            Response.BinaryWrite(byteViewer);

            Response.Flush();
            Response.End();
        }
        catch (Exception ex)
        {
            if (ReportFormat == "HTML" && ex.Message.IndexOf("Specified argument was out of the range of valid values.") == 0)
            {

            }
            else
            {
                Response.Write(ex.Message);
            }
        }
    }
    void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
    {
        GlobalClass gc = new GlobalClass();

        string NumberDataTypes = "Integer, Float";
        string SQLAll = "";

        foreach (string DataSourceName in e.DataSourceNames)
        {
            string Parameters = "|" + gc.GetSPParameters(DataSourceName).ToUpper();

            string SQL = "exec " + DataSourceName + " ";
            for (int p = 0; p < e.Parameters.Count; p++)
            {
                string ParameterName = "@" + e.Parameters[p].Name;

                if (Parameters.IndexOf("|" + ParameterName.ToUpper() + "|") > 0)
                {

                    if (e.Parameters[p].Values[0] == null)
                    {
                        SQL += ParameterName + "=null, ";
                    }
                    else if (NumberDataTypes.IndexOf(e.Parameters[p].DataType.ToString()) > -1)
                    {
                        SQL += ParameterName + "=" + e.Parameters[p].Values[0].ToString() + ", ";
                    }
                    else
                    {
                        SQL += ParameterName + "='" + e.Parameters[p].Values[0].ToString() + "', ";
                    }
                }
            }

            if (SQL.EndsWith(", "))
            {
                SQL = SQL.Substring(0, SQL.Length - 2);
            }

            SQLAll += SQL + " ";
            //e.DataSources.Add(new ReportDataSource(DataSourceName, gc.GetDataSet(SQL, Application["ConnectionString"].ToString()).Tables[0]));
        }

        DataSet ds = new DataSet();
        ds = gc.GetDataSetRV(SQLAll, Application["ConnectionString"].ToString());

        int dsi = 0;
        foreach (string DataSourceName in e.DataSourceNames)
        {
            e.DataSources.Add(new ReportDataSource(DataSourceName, ds.Tables[dsi]));
            dsi++;
        }

        ds.Dispose();
        gc = null;
    }
}