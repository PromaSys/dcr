<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Logout</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body style="padding: 20px; font-family: Arial;">
    <div  >
        <div style="border-radius: 0px;padding: 0px; width: 270px;">
            <img src="<%= ResolveUrl("~/images/" + Application["ApplicationLogo"].ToString()) %>" style="height:100px; "/>
        </div>
        <br /><br />
        <%
            if (Request.Cookies["LogoutReason"] != null)
            {
                if (Request.Cookies["LogoutReason"].Value == "Expiry")
                {
                    Request.Cookies["LogoutReason"].Expires = DateTime.Now.AddDays(-1);
                    %>
                        Your session has expired.
                        <br />
                        This happens after prolonged periods of inactivity.
                    <%
                 }
            }      
            else
            {
                %>
                Good bye!
                <br />
                You have been logged off.
                <%
            }
        %>
        
        <br /><br />
        What do you want to do?
        <br /><br />
        &nbsp;&nbsp;Close This browser
        <br /><br />&nbsp;&nbsp;or<br /><br />
        &nbsp;&nbsp;<a href="<%=ResolveUrl("~/Account/Login") %>">Go back to <%=Application["ApplicationName"].ToString() %></a>
    </div>
</body>
</html>
