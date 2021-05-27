<%@ Page Title="Forms" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Forms.aspx.cs" Inherits="Forms" %>

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
                        Table ="dcr_Forms"
                        KeyField ="Form_ID"
                        runat="server"
                        Title ="Templates|<a href='#' onclick='gridEditorFormNew({element: this, w:400, h:180});' class='btn btn-primary'>New Form</a>"
                        Edit = "false"
                        Links = "Name|gridEditorForm({element: this, w: RelativePixels('w', 1, 400), h: RelativePixels('h', 0.25, 600)})|Field_Count|displayTemplateFields(this)"
                        _Required ="Text"
                        Hide="Form_ID|Form_Template_ID|Subject_ID"
                        Labels ="Category_ID|Category"
                        DoNotEdit ="Field_Count"
                        _Formats ="Date|MM/dd/yyyy"
                        ColumnStyle ="Field_Count|text-align: center;"
                        Widths="default"
                        Calendars="Date"
                        TextAreas = "Long_Text|3"
                        _NumberFormat = "Money|$#,###.00"
                        DeleteColumn="false"
                        Chat ="Chat|Test|Subject_ID|800,600"
                        Docs="Docs|Test|Subject_ID|1000,400" 
                        NewRecords="1" 
                        Search ="menu"
                        cssClass ="table table-hover scrollable fixedheader js-dynamitable hg"

                        />

            </td>
        </tr>
    </table>

</asp:Content>

