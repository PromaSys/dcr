<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Form_Test.aspx.cs" Inherits="Form_Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="Content/bootstrap.css" rel="stylesheet"/>
    <link href="Content/jquery-ui-1.11.4.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />
    <link href="Content/webapp.css" rel="stylesheet" />

    <script src="Scripts/jquery-2.2.3.min.js"></script>
    <script src="Scripts/jquery-ui-1.11.4.min.js"></script>
    <script src="Scripts/webapp.js"></script>
</head>
<body>
    <div id="template-form-wrapper">
        <h1>Competitive Pricing Form Template</h1>
        <form id="template-form">
            <asp:Literal ID="FormContent" runat="server" />
        </form>
    </div>
</body>
</html>
