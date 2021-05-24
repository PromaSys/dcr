<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <table border="0" style="width: 100%;" >
        <tr>
            <td style="vertical-align: top;">
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%"  BackColor="AliceBlue" PageCountMode="Estimate"  style="background-color: White;"  ViewStateMode="Enabled"></rsweb:ReportViewer>
            </td>
            <td style="vertical-align: top; text-align: left; width: 320px; background-color: #EAF2FA; padding: 10px;">
                <vg:VGrid
                    ID ="vgParameters"
                    runat= "server"
                    Title = "Filters"
                    cssClass ="table scrollable table-borderless"
                    Edit ="True"
                    KeyField="ID"
                    Table ="Parameters"
                    Stacked ="True"
                    Formats ="Date|M/d/yyyy|StartDate|M/d/yyyy|EndDate|M/d/yyyy"
                />
                <div style="text-align: right; padding: 10px;">
                <button type = "button" class="btn btn-primary" onclick="ApplyReportParameters();" >Apply</button>
                    </div>
            </td>
        </tr>
    </table>
    <input type="hidden" id="txtReportParameters" name="txtReportParameters" />
    
    <script type="text/javascript">  
        //alert("<%=Session["ReportHeight"].ToString() %>");
        //document.getElementById("<%= ReportViewer1.ClientID %>").style.height = "<%=Session["ReportHeight"].ToString() %>";
    </script>
</asp:Content>
