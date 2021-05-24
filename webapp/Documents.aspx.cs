using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Text.RegularExpressions;

public partial class Documents : System.Web.UI.Page
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

            GlobalClass gc = new GlobalClass();
            DataTable dt = new DataTable();

            //Available Client Height
            int AvailableClientHeight = gc.CookieIntValue(Request.Cookies["AvailableClientHeight"], 400);

            #region DefaultForm

            if (Request.QueryString.ToString() != "")
            {
                Session["ContextID"] = Request.QueryString["cid"].ToString();
                Session["Context"] = Request.QueryString["c"].ToString().Replace("+", "");
                Session["DocsReadOnly"] = Request.QueryString["ro"].ToString();

                if (Session["DocsReadOnly"].ToString() == "1")
                {
                    Docs.DeleteColumn = false;
                }
            }

            if (Request.Form["txtContextID"] != null && Request.Form["txtContextID"] != "")
            {
                Session["ContextID"] = gc.TryDecrypt(Request.Form["txtContextID"].ToString());
            }

            if (Request.Form["txtContext"] != null && Request.Form["txtContext"] != "")
            {
                Session["Context"] = gc.TryDecrypt(Request.Form["txtContext"].ToString());
            }

            #endregion DefaultForm

            dt = gc.GetTable("exec app_sp_documents " + Session["ContextID"].ToString() + ",'" + Session["Context"].ToString() + "'");

            string DialogWidth = Request.QueryString["w"].ToString();
            string DialogHeight = Request.QueryString["h"].ToString();
            string DialogTitle = Request.QueryString["t"].ToString();

            Docs.Title = DialogTitle;
            Docs.Height = Convert.ToInt32(DialogHeight) - 140;
            Docs.Width = Convert.ToInt32(DialogWidth);
            Docs.EncryptionKey = Application["EncryptionKey"].ToString();
            Docs.Hide = "Document_ID|Description";

            Docs.GridTable = dt;

            Docs.Widths = "";
            Docs.Formats = "";
            Docs.AddSQL = "Uploaded_By|'" + Session["UserName"].ToString() + "'"; gc = null;
        }
        else
        {
            if (Request.Files.Count > 0)
            {
                UploadFiles();
                Response.Redirect(Request.RawUrl);
                //Response.End();
            }
        }
    }

    protected void UploadFiles()
    {
        string PictureFile = "";
        string PictureFileName = "";
        string ServerPictureFile = "";
        string Type = "";
        string Context = "";
        int ContextID;

        HttpFileCollection uploadedFiles = Request.Files;

        for (int i = 0; i < uploadedFiles.Count; i++)
        {
            HttpPostedFile userPostedFile = uploadedFiles[i];

            try
            {
                if (userPostedFile.ContentLength > 0)
                {

                    //Save at server
                    PictureFile = userPostedFile.FileName;

                    //Span1.InnerText = "Uploading " + PictureFile + "...";

                    PictureFileName = System.IO.Path.GetFileName(PictureFile);

                    ServerPictureFile = Server.MapPath("uploads") + "\\" + PictureFileName;

                    userPostedFile.SaveAs(ServerPictureFile);

                    FileStream stream = new FileStream(ServerPictureFile, FileMode.Open, FileAccess.Read);

                    byte[] buffer = new byte[stream.Length];

                    stream.Read(buffer, 0, (int)stream.Length);

                    stream.Close();

                    Type = userPostedFile.ContentType;

                    ContextID = Convert.ToInt32(Session["ContextID"].ToString());
                    Context = Session["Context"].ToString();

                    InsertImage(ref buffer, PictureFileName, Type, Context, ContextID, Session["UserName"].ToString());
                    buffer = null;

                    if (Request.QueryString["c"].ToString().IndexOf("Eligibility") > -1)
                    {

                        // post processing
                        GlobalClass gc = new GlobalClass();

                        // pre processing
                        gc.ExecuteSQL("delete from app_Eligibility_Import");

                        string Message = ImportExcelEPPlus(ServerPictureFile, PictureFileName, Context, ContextID);

                        if (Message != "")
                        {
                            Response.Cookies["DocumentError"].Value = Message;
                            Response.Cookies["DocumentError"].Expires = DateTime.Now.AddMinutes(1);
                            return;
                        }

                        Message = gc.GetTable("exec app_sp_import_post '" + Context + "'," + ContextID.ToString() + ",'" + Session["UserLogin"].ToString() + "'").Rows[0]["Message"].ToString();
                        gc = null;

                        if (Message != "")
                        {
                            if (Message.StartsWith("Error"))
                            {
                                Response.Cookies["DocumentError"].Value = Message;
                                Response.Cookies["DocumentError"].Expires = DateTime.Now.AddMinutes(1);
                                File.Delete(ServerPictureFile);
                                return;
                            }
                            else
                            {
                                Response.Cookies["DocumentMessage"].Value = Message;
                                Response.Cookies["DocumentMessage"].Expires = DateTime.Now.AddMinutes(1);
                                File.Delete(ServerPictureFile);
                                return;
                            }
                        }
                    }

                    if (Request.QueryString["c"].ToString().IndexOf("Enrollments") > -1)
                    {
                        GlobalClass gc = new GlobalClass();

                        // pre processing
                        gc.ExecuteSQL("delete from app_Enrollments_Import");

                        string Message = ImportExcelEPPlus(ServerPictureFile, PictureFileName, Context, ContextID);
                        //string Message = ImportExcel(ServerPictureFile, PictureFileName, "art_SalesForce_Import");

                        if (Message != "")
                        {
                            Response.Cookies["DocumentError"].Value = Message;
                            Response.Cookies["DocumentError"].Expires = DateTime.Now.AddMinutes(1);
                            return;
                        }

                        // post processing
                        Message = gc.GetTable("exec app_sp_import_post '" + Context + "'," + ContextID.ToString() + ",'" + Session["UserLogin"].ToString() + "'").Rows[0]["Message"].ToString();
                        gc = null;

                        if (Message != "")
                        {
                            if (Message.StartsWith("Error"))
                            {
                                Response.Cookies["DocumentError"].Value = Message;
                                Response.Cookies["DocumentError"].Expires = DateTime.Now.AddMinutes(1);
                                File.Delete(ServerPictureFile);
                                return;
                            }
                            else
                            {
                                Response.Cookies["DocumentMessage"].Value = Message;
                                Response.Cookies["DocumentMessage"].Expires = DateTime.Now.AddMinutes(1);
                                File.Delete(ServerPictureFile);
                                return;
                            }
                        }
                    }

                    //delete file
                    File.Delete(ServerPictureFile);
                }
            }
            catch (Exception Ex)
            {
                //Span1.InnerText = "Error: " + Ex.Message;
                //Response.Write("<script>MessageBox('Import Error', " + Ex.Message.Replace("'","`") + ",'danger');</script>");
                // write error cookie
                Response.Cookies["DocumentError"].Value = Ex.Message;
                Response.Cookies["DocumentError"].Expires = DateTime.Now.AddMinutes(1);

                File.Delete(ServerPictureFile);
                return;
            }
        }
        /*
        if (Page.ClientQueryString == "e")
        {
            Response.Redirect("documents.aspx?e");
        }
        else
        {
            Response.Redirect("documents.aspx");
        }
        */

        // refresh dialog

    }

    private void InsertImage(ref byte[] buffer, string Title, string Type, string Context, int ContextID, string UserName)
    {
        // Create a stored procedure command
        SqlConnection conn = new SqlConnection(GlobalClass.ConnectionString);
        SqlCommand cmd = new SqlCommand("app_sp_upload_file", conn);
        cmd.CommandType = CommandType.StoredProcedure;


        // Add the image paramter and set the value
        cmd.Parameters.Add("@Image", SqlDbType.Image).Value = buffer;

        // Add the name paramter and set the value
        cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = Title;

        cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;

        cmd.Parameters.Add("@Context", SqlDbType.VarChar).Value = Context;

        cmd.Parameters.Add("@ContextID", SqlDbType.Int).Value = ContextID;

        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;

        // Execute the insert
        cmd.Connection.Open();
        cmd.ExecuteNonQuery();
        cmd.Connection.Close();

    }

    private string ImportExcel(string ServerPictureFile, string PictureFileName, string Import)
    {

        // Connection String to Excel Workbook  XLS

        //string excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ServerPictureFile + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";


        //string excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=xxx" + PictureFileName + "_yyy.xls;Extended Properties=\"Excel 8.0;HDR=YES;\"";
        //excelConnectionString = excelConnectionString.Replace("xxx", Server.MapPath("uploads") + "\\");
        //excelConnectionString = excelConnectionString.Replace("yyy", Session["OrganizationID"].ToString());

        //if (ServerPictureFile.ToUpper().EndsWith(".XLSX"))
        //{
        //Connection String to Excel Workbook 2010 (XLSX)

        //string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ServerPictureFile + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"";
        string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ServerPictureFile + ";Extended Properties=\"TEXT;HDR=YES;\"";
        //string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ServerPictureFile + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
        //excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ServerPictureFile + ";Extended Properties=\"Excel 12.0;\"";
        //}

        // Create Connection to Excel Workbook
        using (OleDbConnection connection = new OleDbConnection(excelConnectionString))
        {

            connection.Open();

            string WorkSheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();

            OleDbCommand command = new OleDbCommand("Select * FROM [" + WorkSheet + "]", connection);

            // Create DbDataReader to Data Worksheet
            using (OleDbDataReader dr = command.ExecuteReader())
            {

                string sqlConnectionString = GlobalClass.ConnectionString.ToString();
                SqlConnection conn = new SqlConnection(sqlConnectionString);
                /*
                // get fields
                string Fields = "";


                // create table
                string SQL = "IF OBJECT_ID('" + Import + "', 'U') IS NOT NULL drop table [" + Import + "]; Create table [" + Import + "] (";
                for(int i=0; i < dr.FieldCount; i++)
                {
                    SQL += "[" + dr.GetName(i) + "] varchar(4000),";
                    Fields += dr.GetName(i) + "|";
                }

                if(SQL != "")
                {
                    SQL = SQL.Substring(0, SQL.Length - 1);
                    Fields = Fields.Substring(0, Fields.Length - 1);
                }

                SQL += ")";

                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                */

                // Bulk Copy to SQL Server
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnectionString))
                {
                    bulkCopy.DestinationTableName = "[" + Import + "]";

                    /*
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        bulkCopy.ColumnMappings.Add("[" + dr.GetName(i) + "]", "[" + dr.GetName(i) + "]");
                        //bulkCopy.ColumnMappings.Add(i, i);
                    }
                    */

                    bulkCopy.WriteToServer(dr);
                }

                // execute import script
                //cmd.CommandText = "exec lynx_sp_import '" + Import + "','" + Fields + "','" + Session["UserName"].ToString() + "'";
                //cmd.Connection.Open();
                //cmd.ExecuteNonQuery();


                //cmd.Dispose();
                conn.Dispose();

                /*
                // get SQL to execute
                GlobalClass gc = new GlobalClass();

                string ImportSQL = gc.GetTable("exec lynx_sp_import '" + Import + "','" + Fields + "','" + Session["UserName"].ToString() + "'").Rows[0]["SQL"].ToString();

                gc.GetDataSet(ImportSQL);

                string ImportMessage = gc.GetTable("exec lynx_sp_import_dedupe '" + Import + "'").Rows[0]["Import_Message"].ToString();

                gc = null;
                */

                //Response.Cookies["DocumentMessage"].Value = "Data imported successfully." + ImportMessage;
                //Response.Cookies["DocumentMessage"].Expires = DateTime.Now.AddMinutes(1);
                // process data
                //cmd.CommandText = "exec lynx_sp_process_import '" + Import + "'";
                //cmd.ExecuteNonQuery();

                // delete created table
                //cmd.CommandText = "drop table [" + Import + "]";
                //cmd.ExecuteNonQuery();
                //cmd.Connection.Close();

                return "";

            }
        }
    }

    private string ImportExcelEPPlus(string ServerPictureFile, string PictureFileName, string Context, Int32 ContextID)
    {

        DataTable dt = new DataTable();

        if (PictureFileName.EndsWith(".CSV", StringComparison.OrdinalIgnoreCase))
        {
            dt = GetDataTableFromCSV(ServerPictureFile, true);
        }
        else if (PictureFileName.EndsWith(".XLS", StringComparison.OrdinalIgnoreCase))
        {
            dt = GetDataTableFromHTML(ServerPictureFile);
        }
        else
        {
            dt = GetDataTableFromExcel(ServerPictureFile, true);
        }


        //--- convert empty and blank to null
        foreach (DataRow dtr in dt.Rows)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (string.IsNullOrEmpty(dtr[i].ToString()))
                {
                    dtr[i] = null;
                }
            }
        }


        string DateStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        string ImportFileName = "";

        if (Context == "Enrollments")
        {
            /*
            dt.Columns.Add("Receipt_ID", typeof(Int32), ContextID.ToString());
            dt.Columns.Add("User_ID", typeof(Int32), Session["UserID"].ToString());
            dt.Columns.Add("Sort_Order", typeof(Int32));

            int index = -1;
            DataColumn SortOrder = dt.Columns["Sort_Order"];
            foreach (DataRow row in dt.Rows)
            {
                row.SetField(SortOrder, ++index);
            }
            */


            ImportFileName = "app_Enrollments_Import";
        }

        if (Context == "Eligibility")
        {
            /*
            // separate currency
            dt.Columns.Add("Currency", typeof(String));
            dt.Columns["Currency"].ReadOnly = false;

            foreach (DataRow dtr in dt.Rows)
            {
                string SalesPrice = dtr["Sales Price"].ToString();
                if (SalesPrice != "")
                {
                    //parse for currency
                    var SalesPriceParts = SalesPrice.Split(' ');
                    if (SalesPriceParts.Length == 1)
                    {
                        dtr["Sales Price"] = SalesPrice.Replace("$", "");
                        dtr["Currency"] = "USD";
                    }
                    else
                    {
                        dtr["Sales Price"] = SalesPriceParts[1];
                        dtr["Currency"] = SalesPriceParts[0];
                    }
                }

                string TradePrice = dtr["Trade Price"].ToString();
                if (TradePrice != "")
                {
                    //parse for currency
                    var TradePriceParts = TradePrice.Split(' ');
                    if (TradePriceParts.Length == 1)
                    {
                        dtr["Trade Price"] = TradePrice.Replace("$", "");
                    }
                    else
                    {
                        dtr["Trade Price"] = TradePriceParts[1];
                    }
                }
            }

            dt.Columns.Add("SF_ID", typeof(Int32), ContextID.ToString());
            dt.Columns.Add("User_ID", typeof(Int32), Session["UserID"].ToString());
            */

            // insert in database
            //string cs = GlobalClass.ConnectionString;
            ImportFileName = "app_Eligibility_Import";
        }

        SqlBulkCopy bulkcopy = new SqlBulkCopy(GlobalClass.ConnectionString)
        {
            DestinationTableName = ImportFileName
        };

        try
        {
            bulkcopy.WriteToServer(dt);
            return "";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
    {
        using (var pck = new OfficeOpenXml.ExcelPackage())
        {
            using (var stream = File.OpenRead(path))
            {
                pck.Load(stream);
            }

            var ws = pck.Workbook.Worksheets.First();
            DataTable tbl = new DataTable();

            // for sales force, locate range
            //if (ws.Cells[1, 1, 1, 1].Text == "") // formatted
            if(1 == 2)
            {
                var start = ws.Dimension.Start;
                var end = ws.Dimension.End;

                var RangeAddress = ws.Dimension.Start.Address;
                var FirstRowAddress = ws.Dimension.Start.Address;

                for (int row = start.Row; row <= end.Row; row++)
                {
                    if (RangeAddress != ws.Dimension.Start.Address)
                    {
                        break;
                    }

                    // Row by row...  
                    for (int col = 1; col <= end.Column; col++)
                    { // ... Cell by cell...  
                        if (ws.Cells[row, col].Text == "Last Modified Date")
                        {
                            RangeAddress = ws.Cells[row, col].Address;

                            // look for Total in the same column
                            for (int j = 1; j < 100; j++)
                            {
                                if (ws.Cells[row + j, col].Text == "Total")
                                {
                                    FirstRowAddress = ws.Cells[row, col, row, col + 52].Address;
                                    RangeAddress = ws.Cells[row, col, row + j - 1, col + 52].Address;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                if (RangeAddress != ws.Dimension.Start.Address)
                {
                    foreach (var firstRowCell in ws.Cells[FirstRowAddress])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }

                    for (int rowNum = ws.Cells[RangeAddress].Start.Row + 1; rowNum <= ws.Cells[RangeAddress].End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, ws.Cells[RangeAddress].Start.Column, rowNum, ws.Cells[RangeAddress].End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            string Field = ws.Cells[ws.Cells[RangeAddress].Start.Row, cell.Start.Column].Text;
                            if (Field == "Sales Price" || Field == "Trade Price")
                            {
                                row[cell.Start.Column - ws.Cells[RangeAddress].Start.Column] = cell.Text;
                            }
                            else
                            {
                                row[cell.Start.Column - ws.Cells[RangeAddress].Start.Column] = cell.Value;
                            }
                        }
                    }
                }

                // remove merged cells
                foreach (DataColumn c in tbl.Columns)
                {
                    if (c.ColumnName.StartsWith("Column"))
                    {
                        tbl.Columns.Remove(c);
                        break;
                    }
                }

            }
            else // plain worksheet
            {

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
            }

            return tbl;
        }
    }

    private DataTable GetDataTableFromCSV(string path, bool hasHeader = true)
    {
        StreamReader sr = new StreamReader(path);

        try
        {
            string FileBody = sr.ReadToEnd();
            sr.Close();

            Regex r = new Regex(@"(?m)^[^""\r\n]*(?:(?:""[^""]*"")+[^""\r\n]*)*");
            MatchCollection Lines = r.Matches(FileBody);

            if (Lines.Count > 0)
            {
                //string[] headers = Lines[0].ToString().Split(',');
                string[] headers = Regex.Split(Lines[0].ToString(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataTable dt = new DataTable();
                foreach (string header in headers)
                {
                    dt.Columns.Add(header.Replace("\"", ""));
                }

                for (int l = 1; l < Lines.Count; l++)
                {
                    string Line = Lines[l].ToString();

                    if (Line.Trim() != "")
                    {
                        string[] rows = Regex.Split(Line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Replace("\"", "");
                        }
                        dt.Rows.Add(dr);
                    }
                }

                return dt;
            }
            else
            {
                return null;
            }


        }
        catch (Exception Ex)
        {
            sr.Close();
            throw;
        }
    }

    private DataTable GetDataTableFromHTML(string path)
    {

        string HTML = File.ReadAllText(path);


        DataTable dt = null;
        DataRow dr = null;
        DataColumn dc = null;
        string TableExpression = "<table[^>]*>(.*?)</table>";
        string HeaderExpression = "<th[^>]*>(.*?)</th>";
        string RowExpression = "<tr[^>]*>(.*?)</tr>";
        string ColumnExpression = "<td[^>]*>(.*?)</td>";
        bool HeadersExist = false;
        int iCurrentColumn = 0;
        int iCurrentRow = 0;

        // Get a match for all the tables in the HTML    
        MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // Loop through each table element    
        foreach (Match Table in Tables)
        {

            // Reset the current row counter and the header flag    
            iCurrentRow = 0;
            HeadersExist = false;

            // Add a new table to the DataSet    
            dt = new DataTable();

            // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)    
            if (Table.Value.Contains("<th"))
            {
                // Set the HeadersExist flag    
                HeadersExist = true;

                // Get a match for all the rows in the table    
                MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Loop through each header element    
                foreach (Match Header in Headers)
                {
                    //dt.Columns.Add(Header.Groups(1).ToString);  
                    dt.Columns.Add(Server.HtmlDecode(Header.Groups[1].ToString()));

                }
            }
            else
            {
                for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase)[0].ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                {
                    dt.Columns.Add("Column " + iColumns);
                }
            }

            // Get a match for all the rows in the table    
            MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Loop through each row element    
            foreach (Match Row in Rows)
            {

                // Only loop through the row if it isn't a header row    
                if (!(iCurrentRow == 0 & HeadersExist == true))
                {

                    // Create a new row and reset the current column counter    
                    dr = dt.NewRow();
                    iCurrentColumn = 0;

                    // Get a match for all the columns in the row    
                    MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Loop through each column element    
                    foreach (Match Column in Columns)
                    {

                        DataColumnCollection columns = dt.Columns;

                        /*
                        if (!columns.Contains("Column " + iCurrentColumn))
                        {
                            //Add Columns  
                            dt.Columns.Add("Column " + iCurrentColumn);
                        }
                        */

                        // Add the value to the DataRow    
                        dr[iCurrentColumn] = Server.HtmlDecode(Column.Groups[1].ToString());
                        // Increase the current column    
                        iCurrentColumn += 1;

                    }

                    // Add the DataRow to the DataTable    
                    dt.Rows.Add(dr);

                }

                // Increase the current row counter    
                iCurrentRow += 1;
            }


        }

        return (dt);

    }
}