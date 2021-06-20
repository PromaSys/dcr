<%@ page title="BI" language="C#" masterpagefile="~/Site.master" autoeventwireup="true" inherits="BI, App_Web_hcfhn0u3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <table class="LeftRightTable">
        <tr>
            <!--
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
            -->
            <td class="RightTD">
                <hg:HGrid
                    ID="hgReports"
                    Table="dcr_BI"
                    KeyField="BI_ID"
                    runat="server"
                    Title="Reports|<a href='#' onclick='reportNew(this);' class='btn btn-primary'>New Report</a>"
                    Edit="false"
                    Links="Report|reportEdit(this);"
                    Required="Report|Template_Field_IDs"
                    Hide="BI_ID"
                    Labels="Category_IDs|Categories|Subject_IDs|Subjects|Form_Template_IDs|Templates|Template_Field_IDs|Fields|Period|Default Period|Filter_Field_IDs|Filter Fields"
                    DoNotEdit="Modified|Modified_By| "
                    ColumnStyle="Active|text-align: center;|Del|text-align: center;"
                    Widths=""
                    Calendars=""
                    TextAreas=""
                    Formats="Modified|MMM d, yyyy"
                    DeleteColumn="true"
                    CssClass="table table-hover scrollable fixedheader js-dynamitable hg" />

            </td>
        </tr>
    </table>

    <input type="hidden" name="BIID" id="BIID" />
    
</asp:Content>

