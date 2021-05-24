<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ResetPasswordConfirmation.aspx.cs" Inherits="ResetPasswordConfirmation" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">


    <div style="border-radius: 0px;padding: 12px; width: 270px;">
            <img src="<%= ResolveUrl("~/images/" + Application["ApplicationLogo"].ToString()) %>" style="height:100px; "/>
        <br /><br />
        <p>Your password has been changed.<br />Click <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/Login">here</asp:HyperLink> to login </p>
    </div>

</asp:Content>

