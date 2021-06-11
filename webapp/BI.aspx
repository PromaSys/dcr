<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="BI.aspx.cs" Inherits="BI" %>

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
                </table>
            </td>
            <td class="RightTD">
                    <hg:HGrid
                        ID="hgBI"
                        Table ="dcr_Reports"
                        KeyField ="Report_ID"
                        runat="server"
                        Title ="Reports|<a href='#' onclick='reportNew(this);' class='btn btn-primary'>New Report</a>"
                        Edit = "false"
                        Links = "Report|Editor, RelativePixels('w', 1, 800), RelativePixels('h', .75, 800)"
                        Required ="Report|Template_Field_IDs"
                        Hide="Report_ID"
                        Labels ="Category_IDs|Categories|Subject_IDs|Subjects|Form_Template_IDs|Templates|Template_Field_IDs|Fields"
                        DoNotEdit ="Modified|Modified_By"
                        ColumnStyle ="Active|text-align: center;|Del|text-align: center;"
                        Widths=""
                        Calendars=""
                        TextAreas = ""
                        NumberFormat = ""
                        DeleteColumn="true"
                        CssClass ="table table-hover scrollable fixedheader js-dynamitable hg"

                        />

            </td>
        </tr>
    </table>
</asp:Content>

