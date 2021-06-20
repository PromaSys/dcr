using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Xml;
using System.Text.RegularExpressions;


namespace WebApp.App_Code
{
    public class GlobalClass
    {

        public static string ApplicationName;

        public static string ConnectionString;

        public static string FromEmail;
        public static string SMTP;
        public static string SMTPPort;
        public static bool EnableSSL;
        public static string EmailUser;
        public static string EmailPassword;
        public static string EmailCC;

        //public int SendEmail(string NotificationID, string Email, string Subject, string Message, string cc, string bcc)
        public string SendEmail(string Email, string Subject, string Message, string cc = "", string bcc = "", string AttachementPath = "")
        {
            //HttpApplicationState Application = HttpContext.Current.Application;
            try
            {
                MailMessage oMsg = new MailMessage();
                oMsg.From = new MailAddress(FromEmail, ApplicationName + " Alert");

                char[] delimiterChars = { ',', ';' };
                string[] Emails = Email.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                foreach(string EmailAddress in Emails)
                {
                    oMsg.To.Add(EmailAddress.Trim());
                }

                oMsg.Subject = Subject;
                oMsg.IsBodyHtml = true;
                oMsg.Body = Message;

                if (cc != "")
                {
                    oMsg.CC.Add(cc);
                }

                if (bcc != "")
                {
                    oMsg.Bcc.Add(bcc);
                }

                if(AttachementPath != "")
                {
                    oMsg.Attachments.Add(new Attachment(AttachementPath));
                }

                SmtpClient client = new SmtpClient(SMTP);
                client.Port = Convert.ToInt32(SMTPPort);
                client.EnableSsl = EnableSSL;
                client.UseDefaultCredentials = false;

                if (EmailUser != "")
                {
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(EmailUser, EmailPassword);
                    client.Credentials = credentials;
                }

                client.Send(oMsg);

                oMsg = null;
                //GetDataSet("exec app_sp_email_result_update " + NotificationID + ",1", Application["ConnectionString"].ToString());
                return "";
            }
            catch (Exception excp)
            {
                string ErrorMessage = excp.Message;
                //GetDataSet("exec app_sp_email_result_update " + NotificationID + ",0", Application["ConnectionString"].ToString());
                return ErrorMessage;
            }
        }
        public DataSet GetDataSet(string SQL)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlDataAdapter cmd = new SqlDataAdapter(SQL, conn);

            DataSet ds = new DataSet();

            ds.EnforceConstraints = false;

            cmd.FillSchema(ds, SchemaType.Mapped);
            cmd.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            cmd.Fill(ds);

            cmd = null;
            conn.Close();

            return ds;
        }
        public DataSet GetDataSet(string SQL, string ConnectionString)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlDataAdapter cmd = new SqlDataAdapter(SQL, conn);

            DataSet ds = new DataSet();

            ds.EnforceConstraints = false;

            cmd.FillSchema(ds, SchemaType.Mapped);
            cmd.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            cmd.Fill(ds);

            cmd = null;
            conn.Close();

            return ds;
        }
        public DataTable GetTable(string SQL)
        {
            DataTableCollection dtc = GetTables(SQL);
            if (dtc.Count == 1)
            {
                return dtc[0];
            }
            else { 
                return null;
            }
        }
        public DataTableCollection GetTables(string SQL)
        {
            return GetDataSet(SQL).Tables;
        }        
        public bool ExecuteSQL(string SQL)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(SQL, conn);

            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            cmd = null;
            conn.Close();

            return true;
        }
        public string EncryptAES(string toEncrypt)
        {

            HttpApplicationState Application = HttpContext.Current.Application;

            if (Application["EncryptionKey"].ToString() == "") return toEncrypt;

            var key = UTF8Encoding.UTF8.GetBytes(Application["EncryptionKey"].ToString().Replace("-", ""));

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(toEncrypt);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }
        public string DecryptAES(string cipherString)
        {
            if (cipherString == "")
            {
                return cipherString;
            }

            cipherString = cipherString.Replace(" ", "+");

            HttpApplicationState Application = HttpContext.Current.Application;

            if (Application["EncryptionKey"].ToString() == "") return cipherString;

            var key = UTF8Encoding.UTF8.GetBytes(Application["EncryptionKey"].ToString().Replace("-", ""));
            //var key = UTF8Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            var fullCipher = Convert.FromBase64String(cipherString);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            //var key = Encoding.UTF8.GetBytes(key);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
        public string Encrypt(string toEncrypt)
        {
            HttpApplicationState Application = HttpContext.Current.Application;

            if (Application["EncryptionKey"].ToString() == "") return toEncrypt;

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = Application["EncryptionKey"].ToString();
            /*
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            */

            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            keyArray = sha256.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            sha256.Clear();

            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public string Decrypt(string cipherString)
        {
            if (cipherString == "")
            {
                return cipherString;
            }

            cipherString = HttpUtility.UrlDecode(cipherString);
            //cipherString = cipherString.Substring(1, cipherString.Length - 1);
            cipherString = cipherString.Replace(" ", "+");

            HttpApplicationState Application = HttpContext.Current.Application;

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            string key = Application["EncryptionKey"].ToString();

            /*
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            */


            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            keyArray = sha256.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            sha256.Clear();

            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);

        }
        public string TryDecrypt(string cipherString)
        {
            string ocipherString = "";
            try
            {
                cipherString = HttpUtility.UrlDecode(cipherString);
                ocipherString = cipherString;
                //cipherString = cipherString.Substring(1, cipherString.Length - 1);
                cipherString = cipherString.Replace(" ", "+");

                byte[] data = Convert.FromBase64String(cipherString);
                return Decrypt(cipherString);
            }
            catch
            {
                return ocipherString;
            }
        }
        public string TryDecryptAll(string cipherString)
        {
            try
            {
                string DecryptAll = "";

                string delimiter = ",";

                if (cipherString.IndexOf("|") > -1)
                {
                    delimiter = "|";
                }
                if (cipherString.IndexOf(", ") > -1)
                {
                    delimiter = ", ";
                }


                string[] separatingChars = { delimiter };
                string[] cipherSegments = cipherString.Split(separatingChars, System.StringSplitOptions.None);

                foreach (string Segment in cipherSegments)
                {
                    DecryptAll += TryDecrypt(Segment.Trim()) + delimiter;
                }

                if (DecryptAll != "")
                {
                    DecryptAll = DecryptAll.Substring(0, DecryptAll.Length - delimiter.Length);
                }

                return DecryptAll;
            }
            catch
            {
                return cipherString;
            }
        }
        /*
        public string Encrypt(string toEncrypt)
        {
            HttpApplicationState Application = HttpContext.Current.Application;

            if (Application["EncryptionKey"].ToString() == "") return toEncrypt;

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = Application["EncryptionKey"].ToString();

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public string Decrypt(string cipherString)
        {
            if (cipherString == "")
            {
                return cipherString;
            }

            cipherString = HttpUtility.UrlDecode(cipherString);
            //cipherString = cipherString.Substring(1, cipherString.Length - 1);
            cipherString = cipherString.Replace(" ", "+");

            HttpApplicationState Application = HttpContext.Current.Application;

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            string key = Application["EncryptionKey"].ToString();

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);

        }
        
        public string EncryptPassword(string clearText)
        {
            HttpApplicationState Application = HttpContext.Current.Application;
            string EncryptionKey = Application["EncryptionKey"].ToString();
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string DecryptPassword(string cipherText)
        {
            HttpApplicationState Application = HttpContext.Current.Application;
            string EncryptionKey = Application["EncryptionKey"].ToString();
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        */
        public string GetSelect(string id, bool TopBlank, DataTable dsTable)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<select id=\"" + id + "\" template=\"select\" >");

            if (TopBlank)
            {
                sb.Append("<option></option>");
            }

            foreach (DataRow dr in dsTable.Rows)
            {

                string Value = dr["value"].ToString();
                string Label = dr["label"].ToString();
                string OtherFields = " ";
                string ValueLabel = "VALUE, LABEL";
                
                Value = Encrypt(Value);

                foreach (DataColumn dc in dsTable.Columns)
                {
                    if (!ValueLabel.Contains(dc.ColumnName.ToUpper()))
                    {
                        OtherFields += dc.ColumnName + "=\"" + dr[dc.Ordinal].ToString() + "\" ";
                    }
                }

                sb.Append("<option value=\"" + Value + "\"" + OtherFields + " >" + Label + "</option>" );
            }

            sb.Append("</select>");
            return sb.ToString();
        }
        public string GetSelect(string id, bool TopBlank, DataTable dsTable, string SelectedValue)
        {
            string sel = GetSelect(id, TopBlank, dsTable);
            string ESelectedValue = Encrypt(SelectedValue);
            sel = sel.Replace("value=\"" + ESelectedValue + "\"", "value=\"" + ESelectedValue + "\" selected ");
            sel = sel.Replace("value=\"" + SelectedValue + "\"", "value=\"" + SelectedValue + "\" selected ");
            return sel;
        }
        public string GetSelect(string id, bool TopBlank, DataTable dsTable, string Label, string Value)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<SELECT id=\"" + id + "\" >");
            if (TopBlank)
            {
                sb.Append("<OPTION></OPTION>");
            }


            foreach (DataRow dr in dsTable.Rows)
            {
                sb.Append("<OPTION value=\"" + dr[Value].ToString() + "\">" + dr[Label].ToString() + "</OPTION>\n"); 
            }

            sb.Append("</SELECT>");

            return sb.ToString();
        }
        public string GetSelect(string id, bool TopBlank, DataTable dsTable, bool AddNew)
        {
            string sel = GetSelect(id, TopBlank, dsTable);
            sel = sel.Replace("</select>", "<option></option><option>Not in this list</option></select>");
            /*
            if (sel.IndexOf("onchange=") > -1)
            {
                sel = sel.Replace("onchange=", "onchange=\"AddNewItem(this);\"");
            }
            else
            {
                sel = sel.Replace("<select", "<select onchange=\"AddNewItem(this);\"");
            }
            */
            return sel;
        }
        public string GetSelect(string id, bool TopBlank, DataTable dsTable, int size, string SelectedValue)
        {
            string sel = GetSelect(id, TopBlank, dsTable);
            sel = sel.Replace("<select ", "<select size=\"" + size.ToString() + "\" ");

            string ESelectedValue = Encrypt(SelectedValue);
            sel = sel.Replace("value=\"" + ESelectedValue + "\"", "value=\"" + ESelectedValue + "\" selected ");
            sel = sel.Replace("value=\"" + SelectedValue + "\"", "value=\"" + SelectedValue + "\" selected ");

            return sel;
        }
        public string GetMultipleSelect(string id, DataTable dsTable, int Columns, int Height = 0, string MultipleSelectValues = "")
        {

            StringBuilder sb = new StringBuilder();
            int dsRows;
            int dsColumns;
            int selRows;
            int dsRow;

            //sb.Append("<div style=\"height: " + Height + "px; overflow: auto;\"><table id=\"" + id + "\" class=\"hgmultipleselect\" template=\"multipleselect\" >");
            if (Height != 0)
            {
                sb.Append("<table id=\"" + id + "\" class=\"hgmultipleselect\" template=\"multipleselect\" style=\"height: " + Height.ToString() + "px; overflow: auto;\" >");
            }
            else
            {
                sb.Append("<table id=\"" + id + "\" class=\"hgmultipleselect\" template=\"multipleselect\" >");
            }

            if (MultipleSelectValues != "")
            {
                MultipleSelectValues = TryDecryptAll(MultipleSelectValues);
            }

            dsRows = dsTable.Rows.Count;

            if (dsRows == 0)
            {
                sb.Append("<tr><td><input type=checkbox style=\"display: none;\">No records</td></tr>");
            }
            else
            {
                decimal d = Convert.ToDecimal(dsTable.Rows.Count) / Convert.ToDecimal(Columns);
                selRows = Convert.ToInt32(Math.Ceiling(d));
                dsColumns = dsTable.Columns.Count;

                for (int r = 0; r < selRows; r++)
                {
                    sb.Append("<tr >");
                    for (int i = 0; i < Columns; i++)
                    {
                        dsRow = i + Columns * r;
                        if (dsRow > dsRows - 1)
                        {
                            sb.Append("<td></td>");
                        }
                        else
                        {
                            string Value = dsTable.Rows[dsRow]["value"].ToString();
                            string Checked = "";

                            if (MultipleSelectValues.Split(',').Contains(Value))
                            {
                                Checked = "checked";
                            }

                            if (dsColumns == 2)
                            {
                                sb.Append("<td><input id=\"" + id + "r" + r.ToString() + "c" + i.ToString() + "\" type=\"checkbox\" value=\"" + Encrypt(dsTable.Rows[dsRow]["value"].ToString()) + "\" " + Checked + " />" + dsTable.Rows[dsRow]["label"].ToString() + "</td >");
                            }
                            else if (dsColumns == 3)
                            {
                                sb.Append("<td " + dsTable.Columns[2].ColumnName + "=\"" + dsTable.Rows[dsRow][2].ToString() + "\" ><input id=\"" + id + "r" + r.ToString() + "c" + i.ToString() + "\" type=\"checkbox\" value=\"" + Encrypt(dsTable.Rows[dsRow]["value"].ToString()) + "\" " + Checked + " />" + dsTable.Rows[dsRow]["label"].ToString() + "</td >");
                            }
                            else if (dsColumns == 4)
                            {
                                sb.Append("<td " + dsTable.Columns[2].ColumnName + "=\"" + dsTable.Rows[dsRow][2].ToString() + "\" " + dsTable.Columns[3].ColumnName + "=\"" + dsTable.Rows[dsRow][3].ToString() + "\" ><input id=\"" + id + "r" + r.ToString() + "c" + i.ToString() + "\" type=\"checkbox\" value=\"" + Encrypt(dsTable.Rows[dsRow]["value"].ToString()) + "\" " + Checked + " />" + dsTable.Rows[dsRow]["label"].ToString() + "</td >");
                            }
                        }
                    }
                    sb.Append("</tr >");
                }
            }

            //sb.Append("</table ></div >");
            sb.Append("</table >");
            return sb.ToString();
        }
        public string GetMultipleSelect_(string id, DataTable dsTable, int Columns)
        {

            StringBuilder sb = new StringBuilder();
            int dsRows;
            int dsColumns;
            int selRows;
            int dsRow;

            sb.Append("<table id=\"" + id + "\" class=\"hgmultipleselect\" template=\"multipleselect\" >");

            dsRows = dsTable.Rows.Count;

            if (dsRows == 0)
            {
                sb.Append("<tr><td><input type=checkbox style=\"display: none;\">No records</td></tr>");
            }
            else
            {
                decimal d = Convert.ToDecimal(dsTable.Rows.Count) / Convert.ToDecimal(Columns);
                selRows = Convert.ToInt32(Math.Ceiling(d));
                dsColumns = dsTable.Columns.Count;

                for (int r = 0; r < selRows; r++)
                {
                    sb.Append("<tr >");
                    for (int i = 0; i < Columns; i++)
                    {
                        dsRow = i + Columns * r;
                        if (dsRow > dsRows - 1)
                        {
                            sb.Append("<td></td>");
                        }
                        else
                        {
                            if (dsColumns == 2)
                            {
                                sb.Append("<td><input id=\"" + id + "r" + r.ToString() + "c" + i.ToString() + "\" type=\"checkbox\" value=\"" + Encrypt(dsTable.Rows[dsRow]["value"].ToString()) + "\" />" + dsTable.Rows[dsRow]["label"].ToString() + "</td >");
                            }
                            else if (dsColumns == 3)
                            {
                                sb.Append("<td " + dsTable.Columns[2].ColumnName + "=\"" + dsTable.Rows[dsRow][2].ToString() + "\" ><input id=\"" + id + "r" + r.ToString() + "c" + i.ToString() + "\" type=\"checkbox\" value=\"" + Encrypt(dsTable.Rows[dsRow]["value"].ToString()) + "\" />" + dsTable.Rows[dsRow]["label"].ToString() + "</td >");
                            }
                        }
                    }
                    sb.Append("</tr >");
                }
            }

            sb.Append("</table >");
            return sb.ToString();
        }
        public string GetSpreadDropDown(string id, bool TopBlank, DataTable dsTable)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("[ ");

            if (TopBlank)
            {
                //sb.Append("<option></option>");
            }

            foreach (DataRow dr in dsTable.Rows)
            {

                string Value = dr["value"].ToString();
                string Label = dr["label"].ToString();
                string OtherFields = " ";
                string ValueLabel = "VALUE, LABEL";

                Value = Encrypt(Value);


                foreach (DataColumn dc in dsTable.Columns)
                {
                    if (!ValueLabel.Contains(dc.ColumnName.ToUpper()))
                    {
                        if (dc.ColumnName.ToUpper().EndsWith("_VALUE"))
                        {
                            OtherFields += dc.ColumnName + "=\"" + Encrypt(dr[dc.Ordinal].ToString()) + "\" ";
                        }
                        else
                        {
                            OtherFields += dc.ColumnName + "=\"" + dr[dc.Ordinal].ToString() + "\" ";
                        }
                    }
                }

                if (Label.IndexOf("'") > -1)
                {
                    Label = Label.Replace("'", "\\'");
                }

                sb.Append("{ 'id' : '" + Value + "', 'name' : '" + Label + "' } ,");
            }

            sb.Append(" ]");
            return sb.ToString();
        }
        public int CookieIntValue(HttpCookie Cookie, int DefaultValue)
        {
            int CookieIntValue = DefaultValue;
            if (Cookie != null)
            {
                CookieIntValue = Convert.ToInt16(Convert.ToDouble(Cookie.Value));
            }

            return CookieIntValue;
        }
        public string GetSPParameters(string SPName)
        {

            HttpApplicationState Application = HttpContext.Current.Application;
            SqlConnection conn = new SqlConnection(Application["ConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(SPName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection.Open();

            SqlCommandBuilder.DeriveParameters(cmd);

            string Parameters = "";

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                Parameters += cmd.Parameters[i].ParameterName + "|";
            }

            cmd.Connection.Close();

            return Parameters;


        }
        public DataSet GetDataSetRV(string SQL, string ConnectionString)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlDataAdapter cmd = new SqlDataAdapter(SQL, conn);
            DataSet ds = new DataSet();

            cmd.Fill(ds);

            cmd = null;
            conn.Close();

            return ds;

        }
        public string GeneratePassword(int lowercase, int uppercase, int numerics, int symbol)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";
            string symbols = "!@#$%";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                ); ;

            for (int i = 1; i <= symbol; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    symbols[random.Next(symbols.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }
        public string DecodeResult(string Text)
        {
            Text = Text.Replace("+", " ");
            Text = Text.Replace("[PL]", "+");
            Text = Text.Replace("[APO]", "'");
            Text = Text.Replace("[QOT]", "\"");
            Text = Text.Replace("[LT]", "<");
            Text = Text.Replace("[GT]", ">");
            Text = Text.Replace("[AT]", "@");
            Text = Text.Replace("[PIP]", "|");
            Text = Text.Replace("[AMP]", "&");
            Text = Text.Replace("[LB]", "#");
            Text = Text.Replace("[PLUS]", "+");

            Text = Text.Replace("[EG]", "è");
            Text = Text.Replace("[EA]", "é");
            Text = Text.Replace("[EC]", "ê");
            Text = Text.Replace("[AG]", "à");


            return Text;
        }
        public string TraceSQL()
        {
            string CurrentUser = HttpContext.Current.Session["UserName"].ToString();
            //string SQL = "Created_By|case when Created_By is null then '" + CurrentUser + "' else Created_By end|Modified|getdate()|Modified_By|'" + CurrentUser + "'";
            string SQL = "Created_By|'" + CurrentUser + "'|Modified|getdate()|Modified_By|'" + CurrentUser + "'";
            return SQL;
        }
        public bool SaveReport(int UserID, string RDL, string Format, string ReportParameters, string ReportFolderPath, string ReportOutputPath)
        {
            bool Result = false;

            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.AsyncRendering = true;
            ReportViewer1.SizeToReportContent = false;

            LocalReport rep = ReportViewer1.LocalReport;
            rep.ReportPath = ReportFolderPath + "\\" + RDL;
            rep.EnableHyperlinks = true;
            rep.DataSources.Clear();
            rep.EnableExternalImages = true;

            Dictionary<string, string> ReportParam = new Dictionary<string, string>();

            if (ReportParameters != "")
            {
                char[] delimiter = { '|' };
                string[] PP = ReportParameters.Split(delimiter);


                for (int i = 0; i < PP.Length - 1; i = i + 2)
                {

                    if (PP[i + 1] == "")
                    {
                        ReportParam.Add("rqp_" + PP[i], "null");
                    }
                    else
                    {
                        ReportParam.Add("rqp_" + PP[i], TryDecrypt(PP[i + 1]));
                    }
                }
            }

            #region Parameters
            XmlDocument xmlReport = new XmlDocument();
            xmlReport.Load(rep.ReportPath);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlReport.NameTable);
            nsmgr.AddNamespace("a", xmlReport.ChildNodes[1].NamespaceURI);

            XmlNodeList parameters = xmlReport.SelectNodes("a:Report/a:ReportParameters/a:ReportParameter", nsmgr);

            string ParameterName = "";
            string ParameterType = "";
            string StringParameters = "|";


            foreach (XmlNode parameter in parameters)
            {
                string DefaultValue = "";
                ParameterName = parameter.Attributes["Name"].Value;
                ParameterType = parameter.SelectSingleNode("a:DataType", nsmgr).InnerText;

                if (ParameterType == "DateTime" || ParameterType == "String")
                {
                    StringParameters += ParameterName + "|";
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
                        foreach (ReportParameterInfo rpi in rep.GetParameters())
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
                            CommandText = CommandText.Replace("UserID", UserID.ToString());
                        }
                    }
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
                if (ReportParameters.IndexOf(ParameterName + "|") > -1)
                {
                    DefaultValue = ReportParam["rqp_" + ParameterName].ToString();
                }
                else
                {
                    ReportParam["rqp_" + ParameterName] = DefaultValue;
                }

                
                //set report parameters
                if (DefaultValue != "null")
                {
                    ReportParameter param = new ReportParameter(ParameterName, DefaultValue);
                    rep.SetParameters(new ReportParameter[] { param });
                }

            }
            // end parameters
            #endregion


            #region Data Sources
            XmlNodeList datasets = xmlReport.SelectNodes("a:Report/a:DataSets/a:DataSet", nsmgr);

            foreach (XmlNode dataset in datasets)
            {
                DataTable dt = new DataTable();
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

                    if (ReportParam["rqp_" + queryparametername] != null)
                    {
                        queryparametervalue = ReportParam["rqp_" + queryparametername].ToString();
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

                    dt = GetTable("exec " + CommandText);
                }
                else
                {
                    dt = GetTable(CommandText);
                }

                ReportDataSource rdsReport = new ReportDataSource();
                rdsReport.Name = dataset.Attributes["Name"].Value;
                rdsReport.Value = dt;
                rep.DataSources.Add(rdsReport);

                rep.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessingEventHandler);

                dt.Dispose();
            }


            Warning[] warn;
            string[] streamids;
            string mimeType = "application/pdf";
            string encoding = string.Empty;
            string extension = string.Empty;
            byte[] byteViewer;
            byteViewer = rep.Render(Format, null, out mimeType, out encoding, out extension, out streamids, out warn);

            // do not alter document generation during testing
            if (Environment.MachineName.ToUpper() != "WES2222")
            {
                using (FileStream fs = new FileStream(ReportOutputPath, FileMode.Create))
                {
                    fs.Write(byteViewer, 0, byteViewer.Length);
                }

                Result = true;
                /*
                if (FormID != 0)
                {
                    gc.GetDataSet("exec app_sp_Update_Form_Generation_Date " + FormID + ",'" + Environment.MachineName + "'", Application["ConnectionString"].ToString());
                }
                */
            }

            return Result;
            #endregion
        }
        void SubreportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //GlobalClass gc = new GlobalClass();

            string NumberDataTypes = "Integer, Float";
            string SQLAll = "";

            foreach (string DataSourceName in e.DataSourceNames)
            {
                string Parameters = "|" + GetSPParameters(DataSourceName).ToUpper();

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
            ds = GetDataSetRV(SQLAll, ConnectionString);

            int dsi = 0;
            foreach (string DataSourceName in e.DataSourceNames)
            {
                e.DataSources.Add(new ReportDataSource(DataSourceName, ds.Tables[dsi]));
                dsi++;
            }

            ds.Dispose();
        }
        public string StripHTML(string Text)
        {
            string TextOut = Text.Replace("<br>", "[br]");
            TextOut = TextOut.Replace("<br />", "[br]");
            TextOut = TextOut.Replace("<BR>", "[br]");
            TextOut = TextOut.Replace("<BR />", "[br]");

            TextOut = Regex.Replace(TextOut, "<.*?>", String.Empty);
            TextOut = TextOut.Replace( "[br]","<br>");
            return TextOut;
        }
        public void SendAppEmails()
        {
            DataTable dt = new DataTable();

            dt = GetTable("select Email_ID, Email, Subject, Message, cc, bcc from app_Emails where Sent_Date is null");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow r = dt.Rows[i];
                string Result = SendEmail(r["Email"].ToString(), r["Subject"].ToString(), r["Message"].ToString(), r["cc"].ToString(), r["bcc"].ToString(), "");

                // sent
                if (Result == "")
                {
                    ExecuteSQL("update app_Emails set Sent_Date = getdate() where Email_ID = " + r["Email_ID"].ToString());
                }
            }
        }
        public void SetCookieSession(string CookieName, string Default)
        {
            var Context = HttpContext.Current;
            var Cookie = Context.Request.Cookies[CookieName];
            var CookieSession = Context.Session[CookieName];

            if (Cookie != null)
            {
                if (Cookie.Value == "undefined")
                {
                    CookieSession = Default;
                }
                else
                {
                    CookieSession = TryDecrypt(Cookie.Value.ToString());
                }
            }

            if (CookieSession == null)
            {
                CookieSession = Default;
            }

            Context.Session[CookieName] = CookieSession;
        }
        public string GetLeftMenu(string ID, DataTable dt, string Title, int Size, string SelectedValue, string All)
        {
            string LeftMenu = GetSelect("sel" + ID + "_", true, dt);
            LeftMenu = LeftMenu.Replace("id=", "onchange=\"SetLeftMenuValue(this, '" + ID + "'); return false;\" class=\"form-control Left-Menu\" size=\"" + Size.ToString() + "\" id=");

            if (Title != "")
            {
                LeftMenu = "<span>" + Title + "</span><br />" + LeftMenu;
            }

            if (All != "")
            {
                LeftMenu = LeftMenu.Replace("<option></option>", "<option value=\"" + Encrypt(All) + "\">All</option>");
            }
            else
            {
                LeftMenu = LeftMenu.Replace("<option></option>", "");
            }

            SelectedValue = Encrypt(SelectedValue);

            LeftMenu = LeftMenu.Replace("value=\"" + SelectedValue + "\"", "value=\"" + SelectedValue + "\" selected ");

            //LeftMenu = "<br />" + LeftMenu;

            LeftMenu += "<br />";

            return LeftMenu;
        }
        public string GetPairValue(string Pairs, string FirstValue, string Default)
        {
            Pairs = Pairs.Trim();

            if (Pairs == "")
            {
                return Default;
            }

            string Delimiter;
            if (Pairs.IndexOf("|") > -1)
            {
                Delimiter = "|";
            }
            else
            {
                Delimiter = ",";
            }


            Pairs = Pairs.Replace(" " + Delimiter, Delimiter);
            Pairs = Pairs.Replace(Delimiter + " ", Delimiter);

            string PairValue = Default;

            string[] ParsedPairs = Pairs.Split(Delimiter.ToCharArray());

            for (int i = 0; i < ParsedPairs.Length - 1; i++)
            {
                if (ParsedPairs[i].ToUpper() == FirstValue.ToUpper())
                {
                    PairValue = ParsedPairs[i + 1];
                    return PairValue;
                }
            }

            return PairValue;
        }
        public string Req_(string Parameter)
        {
            var CurrentRequest = HttpContext.Current.Request;
            var ParameterValue = "";

            if (CurrentRequest.RequestType == "POST" && CurrentRequest.Form.AllKeys.Contains(Parameter))
            {
                ParameterValue = CurrentRequest.Form[Parameter].ToString();
            }
            else if (CurrentRequest.QueryString[Parameter] != null)
            {
                ParameterValue = CurrentRequest.QueryString[Parameter].ToString();
            }

            if (ParameterValue != "")
            {
                ParameterValue = TryDecryptAll(ParameterValue);
            }

            return ParameterValue;
        }
        public string Req(string Parameter)
        {
            var CurrentRequest = HttpContext.Current.Request;
            var ParameterValue = "";

            if (CurrentRequest.RequestType == "POST" && CurrentRequest.Form.AllKeys.Contains(Parameter))
            {
                ParameterValue = CurrentRequest.Form[Parameter].ToString();
            }
            else if (CurrentRequest.QueryString[Parameter] != null)
            {
                ParameterValue = CurrentRequest.QueryString[Parameter].ToString();
            }

            if (ParameterValue != "")
            {
                ParameterValue = TryDecryptAll(ParameterValue);

                if (ParameterValue.Contains("|"))
                {
                    string SegmentValues = "";

                    foreach (string segment in ParameterValue.Split('|'))
                    {
                        SegmentValues += TryDecryptAll(segment) + "|";
                    }

                    if (SegmentValues != "")
                    {
                        SegmentValues = SegmentValues.Substring(0, SegmentValues.Length - 1);
                    }

                    ParameterValue = SegmentValues;
                }
            }

            return ParameterValue;
        }
        public string RowToHtml(string Query)
        {
            DataTable dt = GetTable(Query);

            string Text = "<table class=\"RowToHTML\" cellpadding=\"8\" cellspacing=\"0\" border=\"0\" >\n";

            foreach (DataRow r in dt.Rows)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    Text += "<tr><td>" + c.ColumnName + "</td><td>" + dt.Rows[0][c.ColumnName].ToString() + "</td></tr>\n";
                }
            }

            Text += "</table>";

            return Text;
        }
        public string Filter(string ID, string Label, bool All, DataTable dt, int Size, string Filters)
        {
            // add All
            if (All)
            {
                DataRow dr;
                dr = dt.NewRow();

                dr["Value"] = DBNull.Value;
                dr["Label"] = "All";

                dt.Rows.InsertAt(dr, 0);
            }
            
            string FilterValue = GetPairValue(Filters, ID, "");
            string FilterHTML = GetSelect(ID, false, dt, Size, FilterValue);
            FilterHTML = Label + "<br />" + FilterHTML.Replace(" id=", " class=\"filter form-control\" onchange=\"gridApplyFilters(this);\" id=");
            return FilterHTML;
        }
        public string MultipleSelectFilter(string ID, string Label, bool All, DataTable dt, int Size, string Filters)
        {
            // add All
            if (All && dt.Rows[0]["Label"].ToString() != "All")
            {
                DataRow dr;
                dr = dt.NewRow();

                dr["Value"] = DBNull.Value;
                dr["Label"] = "All";

                dt.Rows.InsertAt(dr, 0);
            }

            int Height = Size * 18;

            string FilterValue = GetPairValue(Filters, ID, "");
            string FilterHTML = GetMultipleSelect(ID, dt, 1, Height, FilterValue);
            FilterHTML = Label + "<br />" + FilterHTML.Replace("class=\"hgmultipleselect\" ", "class=\"hgResult filter\" style=\"height: " + Height + "px; overflow: auto;\" ").Replace("<input id=", "<input onclick=\"gridApplyFilters(this);\" id=");
            return FilterHTML;
        }
        public string FilterDate(string ID, string Label, string Filters)
        {
            string FilterValue = GetPairValue(Filters, ID, "");
            string FilterHTML = Label + "<br /><input id=\"" + ID + "\" value=\"" + FilterValue + "\" class=\"hgResult datepicker filter form-control\" onchange=\"gridApplyFilters(this);\" template=\"calendar\" />";
            return FilterHTML;
        }
        public void ReplaceXMLNodeValue(string documentPath, string selector, string attribute, string newValue)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(documentPath);
            XmlNode node = doc.SelectSingleNode(selector);
            XmlAttribute attributeVal = node.Attributes[attribute];
            attributeVal.Value = newValue;
            doc.Save(documentPath);
        }

    }
}
