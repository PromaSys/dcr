<%@ Page Title="Templates" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Templates.aspx.cs" Inherits="Templates" %>

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
                        ID="hgTemplates"
                        Table ="dcr_Form_Templates"
                        KeyField ="Form_Template_ID"
                        runat="server"
                        Title ="Templates|<a href='#' onclick='gridEditorFormNew({element: this, w: 800, h: 400});' class='btn btn-primary'>New Template</a>"
                        Edit = "false"
                        Links = "Name|gridEditorForm({element: this, w: RelativePixels('w', 1, 800), h: RelativePixels('h', 0.75, 400), afterLoad: attachOnChangeToMultiselect})|Fields|templateFields(this);"
                        Required ="Name|Subject_Category_IDs"
                        Blocked ="Subject_Category_IDs"
                        Hide="Form_Template_ID"
                        Labels ="Categories|Applies To|Subject_Category_IDs|Categories"
                        DoNotEdit ="Fields|Categories"
                        Formats ="Date|MM/dd/yyyy"
                        ColumnStyle ="Field_Count|text-align: center;|Active|text-align: center;|Category_Count|text-align: center;|Preview|text-align: center;"
                        Widths=""
                        Calendars="Date"
                        TextAreas = "Long_Text|3"
                        _NumberFormat = "Money|$#,###.00"
                        DeleteColumn="false"
                        _Chat ="Chat|Test|Subject_ID|800,600"
                        _Docs="Docs|Test|Subject_ID|1000,400" 
                        _NewRecords="1" 
                        Search ="menu"
                        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"

                        />

            </td>
        </tr>
    </table>

</asp:Content>

