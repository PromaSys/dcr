<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site_No_Menu.master" CodeFile="Form.aspx.cs" Inherits="Form" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <vg:VGrid
        ID ="vgForm"
        runat="server" 
        cssClass ="table table-condensed table-borderless vg"        
        Edit =" true"    
        KeyField = "Form_ID"
        Table ="dcr_Form_Fields"
        Stacked="true"
        />

    <!--
    <div id="template-form-wrapper">
        <asp:Literal ID="Literal1" runat="server" ClientIDMode="Static" />
        <asp:TextBox runat="server"></asp:TextBox>
    </div>
    -->
</asp:Content>

