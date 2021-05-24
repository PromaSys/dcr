using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using WebApp;
using WebApp.App_Code;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;

public partial class Process_Change : System.Web.UI.Page
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

        //Response.Write(xmlData.ToString());
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
                if (dt != null)
                {
                    string NewID = gc.Encrypt(dt.Rows[0]["New_ID"].ToString());
                    Response.Write("NewID|" + NewID);
                }
                else
                {
                    if (SQL.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
                    {
                        Response.Write("Delete|Success");
                    }
                    else
                    {
                        Response.Write("Status|Success");
                    }

                    //Response.Write("Error|" + SQL);

                }

                /*
                // post save 
                if (SQL.IndexOf("set [Alert_IDs] = '4'") > -1 || SQL.IndexOf("set [Alert_IDs] = '11'") > -1 || SQL.IndexOf("set [Alert_IDs] = '12'") > -1) {

                    gc.SendEmail();
                 }
                 */

                // send pending emails
                gc.SendAppEmails();
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

            gc = null;
        }
    }

    private string GetChange(string Text)
    {
        Text = Server.UrlDecode(Text);
        Text = Text.Replace("-[_", "<").Replace("_]-", ">");
        return Text;
    }

    private string _DecodeResult(string Text)
    {
        Text = Text.Replace("+", " ");
        Text = Text.Replace("[PL]", "+");
        Text = Text.Replace("[APO]", "''");
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

    private string ConvertToSQL(string Changes)
    {

        GlobalClass gc = new GlobalClass();

        string SQL = "";
        string SQLField = "";
        string SQLValue = "";
        string SQLFieldType = "";
        string SQLFieldEncrypted = "";
        string SQLFields = "";
        string SQLValues = "";
        string NewUserEmail = "";
        string SQLPost = "";

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

        string SQLTable = "";
        string SQLKeyField = "";
        string SQLKeyFieldValue = "";
        string AddSQL = "";

        XmlNode Data = XMLChanges.DocumentElement;
        SQLTable = gc.Decrypt(Data.Attributes["tb"].Value);
        SQLKeyField = gc.Decrypt(Data.Attributes["kf"].Value);
        SQLKeyFieldValue = gc.TryDecrypt(Data.Attributes["kfv"].Value);
        AddSQL = gc.TryDecrypt(Data.Attributes["as"].Value);

        if (SQLTable == "AspNetUsers" && SQLKeyFieldValue == "-1")
        {
            SQLKeyFieldValue = "NewUser";
        }

        if (Data.Attributes["de"] != null)
        {
            string sSQLKeyFieldValue = SQLKeyFieldValue;
            int n;
            if (!int.TryParse(SQLKeyFieldValue, out n))
            {
                sSQLKeyFieldValue = "'" + SQLKeyFieldValue + "'";
            }
            SQL = "delete from [" + SQLTable + "] where [" + SQLKeyField + "]=" + SQLKeyFieldValue;
            return SQL;
        }

        // iterate through nodes
        XmlNodeList Nodes = XMLChanges.SelectNodes("data/fv");

        //insert
        if (SQLKeyFieldValue.Substring(0, 1) == "-")
        {
            foreach (XmlNode Node in Nodes)
            {
                SQLField = gc.Decrypt(Node.Attributes["f"].Value);
                SQLFieldEncrypted = Node.Attributes["e"].Value;
                SQLValue = "";

                if (SQLFieldEncrypted == "1")
                {
                    SQLValue = gc.Decrypt(gc.DecodeResult(Node.Attributes["v"].Value));
                }
                else if (SQLFieldEncrypted == "2")
                {
                    char[] delimiter = { ',' };
                    string[] ValueArray = gc.DecodeResult(Node.Attributes["v"].Value).Replace(" ", "").Split(delimiter);

                    foreach (string Value in ValueArray)
                    {
                        SQLValue += gc.Decrypt(Value) + ", ";
                    }

                    SQLValue = SQLValue.Substring(0, SQLValue.Length - 2);
                }
                else
                {
                    SQLValue = gc.DecodeResult(Node.Attributes["v"].Value);
                }

                // strip HTML
                if (Session["UserGroupID"].ToString() != "1")
                {
                    SQLValue = gc.StripHTML(SQLValue);
                }

                SQLFieldType = Node.Attributes["t"].Value;

                SQLFields += "[" + SQLField + "], ";
                if (SQLValue.Trim() == "")
                {
                    SQLValues += "null, ";
                }
                else if (SQLFieldType == "s")
                {
                    SQLValues += "N'" + SQLValue.Replace("'", "''") + "', ";
                }
                else
                {
                    SQLValues += SQLValue + ", ";
                }
            }

            // add SQL
            if (AddSQL != "")
            {
                string[] AS = AddSQL.Split(new char[] { '|' });
                for (int i = 0; i < AS.Length; i = i + 2)
                {
                    SQLFields += "[" + AS[i] + "], ";
                    SQLValues += AS[i + 1] + ", ";
                }
            }

            if (SQLFields != "")
            {
                SQLFields = SQLFields.Substring(0, SQLFields.Length - 2);
            }

            if (SQLValues != "")
            {
                SQLValues = SQLValues.Substring(0, SQLValues.Length - 2);
            }


            SQL += "insert into [" + SQLTable + "]";
            SQL += "(" + SQLFields + ") ";
            SQL += "values (" + SQLValues + ") ";
            SQL += "IF(@@ERROR > 0) BEGIN ROLLBACK TRAN T100 return END ";

            // get identity
            SQL += "select IDENT_CURRENT('" + SQLTable + "') as New_ID";

        }
        //update
        else
        {
            foreach (XmlNode Node in Nodes)
            {
                SQLField = gc.Decrypt(Node.Attributes["f"].Value);
                SQLFieldEncrypted = Node.Attributes["e"].Value;
                SQLValue = "";

                if (SQLFieldEncrypted == "1")
                {
                    SQLValue = gc.Decrypt(gc.DecodeResult(Node.Attributes["v"].Value));
                }
                else if (SQLFieldEncrypted == "2")
                {
                    char[] delimiter = { ',' };
                    string[] ValueArray = gc.DecodeResult(Node.Attributes["v"].Value).Replace(" ", "").Split(delimiter);

                    foreach (string Value in ValueArray)
                    {
                        SQLValue += gc.DecodeResult(gc.Decrypt(Value)) + ", ";
                    }

                    SQLValue = SQLValue.Substring(0, SQLValue.Length - 2);
                }
                else
                {
                    SQLValue = gc.DecodeResult(Node.Attributes["v"].Value);
                }

                // strip HTML
                if (Session["UserGroupID"].ToString() != "1")
                {
                    SQLValue = gc.StripHTML(SQLValue);
                }

                SQLFieldType = Node.Attributes["t"].Value;

                if (SQLValue.Trim() == "")
                {
                    SQL += "[" + SQLField + "]=null, ";
                }
                else if (SQLFieldType == "s")
                {
                    SQL += "[" + SQLField + "]=N'" + SQLValue.Replace("'", "''") + "', ";
                }
                else
                {
                    SQL += "[" + SQLField + "]=" + SQLValue + ", ";
                }

                if (SQLKeyFieldValue == "NewUser" && SQLField == "Email")
                {
                    NewUserEmail = SQLValue;
                }
            }

            // add sql
            if (AddSQL != "")
            {
                string[] AS = AddSQL.Split(new char[] { '|' });
                for (int i = 0; i < AS.Length; i = i + 2)
                {
                    if (AS[i] != "Created_By")
                    {
                        SQL += "[" + AS[i] + "]=" + AS[i + 1] + ", ";
                    }
                }
            }


            if (SQL != "")
            {
                SQL = SQL.Substring(0, SQL.Length - 2);
            }

            if (SQLKeyFieldValue == "NewUser" && NewUserEmail != "")
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = NewUserEmail, Email = NewUserEmail, EmailConfirmed = true, LockoutEnabled = true };
                var Password = gc.GeneratePassword(3, 3, 2, 0); // Membership.GeneratePassword(8, 0);
                IdentityResult result = manager.Create(user, Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    //string code = manager.GenerateEmailConfirmationToken(user.Id);
                    //string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                    //manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                    //signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                    //IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);

                    SQLKeyFieldValue = user.Id.ToString();
                    SQLKeyField = "Id";
                    SQLPost = " select IDENT_CURRENT('" + SQLTable + "') as New_ID";

                }
                else
                {
                    string ErrorMessage = result.Errors.FirstOrDefault();
                    //SQL = " raiserror('" + ErrorMessage + "', 16,1) ";
                    SQL = "Error|" + ErrorMessage;
                    return SQL;
                }
            }


            SQL = "update [" + SQLTable + "] set " + SQL;

            int Numeric;
            bool IsNumeric = int.TryParse(SQLKeyFieldValue, out Numeric);

            if (IsNumeric)
            {
                SQL += " where [" + SQLKeyField + "]=" + SQLKeyFieldValue;
            }
            else
            {
                SQL += " where [" + SQLKeyField + "]='" + SQLKeyFieldValue + "'";
            }

            SQL += SQLPost;
            SQL += " IF(@@ERROR > 0) BEGIN ROLLBACK TRAN T100 return END ";
        }

        gc = null;

        return SQL;
    }

    private string _Encrypt(string toEncrypt)
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
    private string _Decrypt(string cipherString)
    {
        if (cipherString == "")
        {
            return cipherString;
        }

        cipherString = HttpUtility.UrlDecode(cipherString);
        //cipherString = cipherString.Substring(1, cipherString.Length - 1);
        //cipherString = cipherString.Replace(" ", "+");

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
    private string _GenerateRandomString(int length)
    {
        //Removed O, o, 0, l, 1
        string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        string allowedNumberChars = "23456789";
        char[] chars = new char[length];
        Random rd = new Random();

        bool useLetter = true;
        for (int i = 0; i < length; i++)
        {
            if (useLetter)
            {
                chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                useLetter = false;
            }
            else
            {
                chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                useLetter = true;
            }

        }

        return new string(chars);
    }
    private string _GeneratePassword(int lowercase, int uppercase, int numerics, int symbol)
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

    private void _SendAppEmails()
    {
        GlobalClass gc = new GlobalClass();

        DataTable dt = new DataTable();

        dt = gc.GetTable("select Email_ID, Email, Subject, Message, cc, bcc from app_Emails where Sent_Date is null");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataRow r = dt.Rows[i];
            string Result = gc.SendEmail(r["Email"].ToString(), r["Subject"].ToString(), r["Message"].ToString(), r["cc"].ToString(), r["bcc"].ToString(), "");

            // sent
            if (Result == "")
            {
                gc.ExecuteSQL("update app_Emails set Sent_Date = getdate() where Email_ID = " + r["Email_ID"].ToString());
            }
        }

        gc = null;
    }
}