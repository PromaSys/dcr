<%@ Page Title="Subjects" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="BI_Report.aspx.cs" Inherits="Subjects" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <table class="LeftRightTable">
        <tr>
            <td class="LeftTD" style="width: 240px; _display: none;">
                <table id="tabLeftMenu" class="LeftRightTable">
                    <tr>
                        <td class="LeftMenuTitle">Filters<a class="btn">&nbsp;</a></td>
                    </tr>
                    <tr>
                        <td class="LeftMenuTD Filters">
                            <asp:Literal ID="litLeftMenu" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;"">
                            <a href="#" onclick="gridApplyFilters(this);" class="filterApply btn btn-primary btn-sm" style="width: 80%;">Apply</a>
                        </td>
                    </tr>
                </table>
            </td>
            <td class="RightTD">                
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%"  BackColor="AliceBlue" PageCountMode="Estimate"  style="background-color: White;"  ViewStateMode="Enabled"></rsweb:ReportViewer>
            </td>
        </tr>
    </table>

    <input type="hidden" id="BIID" name="BIID" />

</asp:Content>

